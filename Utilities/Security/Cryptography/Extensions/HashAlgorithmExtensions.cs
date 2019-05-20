using System;
using System.Text;
using System.Security.Cryptography;

namespace Utilities.Security.Cryptography.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HashAlgorithm"/>.
    /// </summary>
    public static class HashAlgorithmExtensions
    {
        /// <summary>
        /// Computes the hash value for the specified string.
        /// </summary>
        /// <param name="algorithm">The instance of the <see cref="HashAlgorithm"/> or its childs.</param>
        /// <param name="value">The instance of the <see cref="String"/> value.</param>
        /// <param name="useBase64">Converts the result to Base64 string.</param>
        /// <returns>The result string.</returns>
        public static string ComputeHash(this HashAlgorithm algorithm, string value, bool useBase64 = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = algorithm.ComputeHash(bytes);
            return useBase64 ? Convert.ToBase64String(hash) : BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
