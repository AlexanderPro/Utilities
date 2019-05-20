using System;
using System.IO;
using System.Security.Cryptography;

namespace Utilities.Security.Cryptography.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="SymmetricAlgorithm"/>.
    /// </summary>
    public static class SymmetricAlgorithmExtensions
    {
        /// <summary>
        /// Encrypts data using specified cryptographic algorithm.
        /// </summary>
        /// <param name="algorithm">Symmetric algorithm.</param>
        /// <param name="clearText">Clear data.</param>
        /// <returns>Ecrypted data.</returns>
        public static byte[] Encrypt(this SymmetricAlgorithm algorithm, byte[] clearText)
        {
            if (clearText == null || clearText.Length == 0)
            {
                throw new ArgumentException("Clear text must not be empty.", "clearText");
            }

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(clearText, 0, clearText.Length);
                cryptoStream.FlushFinalBlock();
                var resultText = memoryStream.ToArray();
                return resultText;
            }
        }

        /// <summary>
        /// Decrypts data using specified cryptographic algorithm.
        /// </summary>
        /// <param name="algorithm">Symmetric algorithm.</param>
        /// <param name="cipherText">Ecrypted data.</param>
        /// <returns>Clear data.</returns>
        public static byte[] Decrypt(this SymmetricAlgorithm algorithm, byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length == 0)
            {
                throw new ArgumentException("Cipher text must not be empty.", "cipherText");
            }

            using (var memoryStream = new MemoryStream(cipherText))
            using (var cryptoStream = new CryptoStream(memoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
            {

                var buffer = new byte[cipherText.Length];
                var lengthText = cryptoStream.Read(buffer, 0, cipherText.Length);
                var resultText = new byte[lengthText];
                Array.Copy(buffer, 0, resultText, 0, lengthText);
                return resultText;
            }
        }
    }
}