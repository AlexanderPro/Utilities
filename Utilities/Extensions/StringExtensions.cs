using System;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Indicates whether this string is null or an System.String.Empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Indicates whether this string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        
        /// <summary>
        /// Compares two strings.
        /// </summary>
        public static bool IsEqual(this string first, string second)
        {
            return string.IsNullOrEmpty(first) ? string.IsNullOrEmpty(second) : string.Equals(first, second);
        }

        /// <summary>
        /// Converts string to a single item array.
        /// </summary>
        public static string[] ToSingleItemArray(this string value)
        {
            return new[] { value };
        }

        /// <summary>
        /// Converts line endings in the string to <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string NormalizeLineEndings(this string value)
        {
            return value.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Converts PascalCase string to camelCase string.
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string value, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return invariantCulture ? value.ToLowerInvariant() : value.ToLower();
            }

            return (invariantCulture ? char.ToLowerInvariant(value[0]) : char.ToLower(value[0])) + value.Substring(1);
        }

        /// <summary>
        /// Converts PascalCase string to camelCase string in specified culture.
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string value, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return value.ToLower(culture);
            }

            return char.ToLower(value[0], culture) + value.Substring(1);
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string.
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string value, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return invariantCulture ? value.ToUpperInvariant() : value.ToUpper();
            }

            return (invariantCulture ? char.ToUpperInvariant(value[0]) : char.ToUpper(value[0])) + value.Substring(1);
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string in specified culture.
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string value, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return value.ToUpper(culture);
            }

            return char.ToUpper(value[0], culture) + value.Substring(1);
        }

        /// <summary>
        /// Computes the md5 hash value for the specified string.
        /// </summary>
        public static string ToMd5(this string value)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(value);
                var hashBytes = md5.ComputeHash(inputBytes);

                var builder = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    builder.Append(hashByte.ToString("X2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}
