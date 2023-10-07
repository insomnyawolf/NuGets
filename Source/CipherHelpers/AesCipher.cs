using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CipherHelpers
{
    public class AesCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KeySize = 256;

        // Size of each data block
        private const int BlockSize = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 128;


        private readonly byte[] Key;
        private readonly byte[] Iv;

        public AesCipher(string Key, string Iv)
        {
            if (string.IsNullOrEmpty(Key))
            {
                throw new InvalidDataException($"{nameof(Key)} can't be null or empty.");
            }
            if (string.IsNullOrEmpty(Iv))
            {
                throw new InvalidDataException($"{nameof(Iv)} can't be null or empty.");
            }

            using var keyManager = new Rfc2898DeriveBytes(Key, Encoding.UTF8.GetBytes(Iv + Key), DerivationIterations);
            this.Key = keyManager.GetBytes(KeySize / 8);

            // I don't understand this, how does it work or why does it take 16
            this.Iv = keyManager.GetBytes(16);
        }

        private void Encryption(Func<Aes, ICryptoTransform> encryptor, Stream input, Stream output)
        {
            using Aes cypher = Aes.Create();
            cypher.Mode = CipherMode.CBC;
            cypher.KeySize = KeySize;
            cypher.BlockSize = BlockSize;
            cypher.Padding = PaddingMode.PKCS7;
            cypher.Key = Key;
            cypher.IV = Iv;

            using var cs = new CryptoStream(output, encryptor(cypher), CryptoStreamMode.Write);
            input.CopyTo(cs);
            input.Flush();
        }

        public void Encrypt(Stream input, Stream output)
        {
            Encryption((encryptor) => encryptor.CreateEncryptor(), input, output);
        }

        public void Decrypt(Stream input, Stream output)
        {
            Encryption((encryptor) => encryptor.CreateDecryptor(), input, output);
        }

        public string Encrypt(string clearInput)
        {
            if (clearInput is null)
            {
                throw new InvalidDataException($"{nameof(clearInput)} can't be null.");
            }

            using var input = new MemoryStream();
            using var writer = new StreamWriter(input);
            writer.Write(clearInput);
            writer.Flush();

            input.Position = 0;

            using var output = new MemoryStream();

            Encrypt(input, output);

            return Convert.ToBase64String(output.ToArray());
        }

        public string Decrypt(string base64cipheredInput)
        {
            if (base64cipheredInput is null)
            {
                throw new InvalidDataException($"{nameof(base64cipheredInput)} can't be null.");
            }

            var inputBytes = Convert.FromBase64String(base64cipheredInput);
            using var input = new MemoryStream(inputBytes);

            using var output = new MemoryStream();

            Decrypt(input, output);

            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
