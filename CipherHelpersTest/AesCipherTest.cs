using CipherHelpers;
using NUnit.Framework;

namespace CipherHelpersTest
{
    public class AesCipherTest
    {
        private readonly AesCipher AesCipher = new(nameof(CipherHelpersTest), nameof(AesCipherTest));

        [SetUp]
        public void Setup()
        {
        }

        [TestCase("Test")]
        [TestCase("SampleText")]
        [TestCase(" ")]
        public void Test1(string input)
        {
            var outputCiphered = AesCipher.Encrypt(input);
            var output = AesCipher.Decrypt(outputCiphered);

            Assert.AreEqual(input, output);
        }
    }
}