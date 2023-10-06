using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace EasySave3Tools
{
    public class EasySave3Helper<TDataStructure>
    {
        const int DerivedKeyLenght = 16;
        const int IvLenght = 16;

        private string _Key;
        private byte[] _KeyBytes;

        public string Key { get => _Key; set { _Key = value; _KeyBytes = Encoding.UTF8.GetBytes(value); } }
        public TDataStructure Value { get; set; }

        public void LoadFrom(Stream inputStream)
        {
            using var aes = Aes.Create();

            var iv = new byte[IvLenght];

            inputStream.Read(iv, 0, IvLenght);

            aes.IV = iv;

            var derivedKey = GetDerivedKey(iv);

            aes.Key = derivedKey;

            using var decryptor = aes.CreateDecryptor();

            using var cs = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);

            Value = JsonSerializer.Deserialize<TDataStructure>(cs);
        }

        public void SaveTo(Stream outputStream)
        {
            using var aes = Aes.Create();

            var iv = new byte[IvLenght];

            Random.Shared.NextBytes(iv);

            aes.IV = iv;

            outputStream.Write(iv, 0, iv.Length);

            var derivedKey = GetDerivedKey(iv);

            aes.Key = derivedKey;

            using var encryptor = aes.CreateEncryptor();

            var cs = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);

            JsonSerializer.Serialize(cs, Value);

            cs.FlushFinalBlock();
        }

        private byte[] GetDerivedKey(byte[] IV)
        {
            using var keyDeriver = new Rfc2898DeriveBytes(_KeyBytes, IV, 100, HashAlgorithmName.SHA1);
            var key = keyDeriver.GetBytes(DerivedKeyLenght);
            return key;
        }
    }
}
