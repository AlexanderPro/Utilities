using System;
using System.Linq;

namespace Utilities.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts string to byte array.
        /// </summary>
        /// <param name="value">The instance of the <see cref="String"/> class.</param>
        /// <param name="hexElement">Does the string contain hexadecimal elements?</param>
        /// <returns>Byte array.</returns>
        public static byte[] ToByteArray(this string value, bool hexElement = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new byte[0];
            }

            if (hexElement)
            {
                var data = new byte[value.Length / 2];
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
                }
                return data;
            }
            else
            {
                var data = new byte[value.Length];
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.ToByte(value.Substring(i, 1), 10);
                }
                return data;
            }
        }

        /// <summary>
        /// Converts byte array to string.
        /// </summary>
        /// <param name="value">The byte array.</param>
        /// <param name="delimiter">The delimiter between the bytes/</param>
        /// <returns>The result string.</returns>
        public static string ToByteString(this byte[] value, string delimiter = "")
        {
            if (value == null || value.Length == 0)
            {
                return string.Empty;
            }

            if (delimiter == null)
            {
                throw new ArgumentNullException("delimiter");
            }

            var result = string.Join(delimiter, value.Select(x => x.ToString("X2")));
            return result;
        }

        /// <summary>
        /// Converts byte array to string.
        /// </summary>
        /// <param name="value">The byte array.</param>
        /// <returns>The string of form 1234567890.</returns>
        public static string ToIntString(this byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                return string.Empty;
            }

            var result = string.Concat(value.Select(x => x.ToString()));
            return result;
        }

        /// <summary>
        /// Removes all leading occurrences of byte from the current object.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="element">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] TrimStart(this byte[] value, byte element)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var i = 0;
            while (value[i] == element) ++i;
            var result = new Byte[value.Length - i];
            Array.Copy(value, i, result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Removes all trailing occurrences of byte from the current object. 
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="element">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] TrimEnd(this byte[] value, byte element)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var i = value.Length - 1;
            while (value[i] == element) --i;
            var result = new Byte[i + 1];
            Array.Copy(value, result, i + 1);
            return result;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of byte from the current object.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="element">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] Trim(this byte[] value, byte element)
        {
            return value.TrimStart(element).TrimEnd(element);
        }

        /// <summary>
        /// Concatenates two specified instances of byte array. 
        /// </summary>
        /// <param name="value">The source byte array to concatenate.</param>
        /// <param name="array">The second byte array to concatenate.</param>
        /// <returns>The concatenation of source and second byte arrays.</returns>
        public static byte[] Concat(this byte[] value, byte[] array)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            var result = new Byte[value.Length + array.Length];
            Array.Copy(value, 0, result, 0, value.Length);
            Array.Copy(array, 0, result, value.Length, array.Length);
            return result;
        }

        /// <summary>
        /// Determines whether two byte arrays are equal.  
        /// </summary>
        /// <param name="value">The source byte array to compare.</param>
        /// <param name="array">The second byte array to compare.</param>
        /// <returns>true if the arrays are considered equal; otherwise, false.</returns>
        public static bool IsEqual(this byte[] value, byte[] array)
        {
            if (value == null || array == null || value.Length != array.Length)
            {
                return false;
            }

            for (var i = 0; i < array.Length; i++)
            {
                if (value[i] != array[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Removes byte from the current object.  
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="array">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] Remove(this byte[] value, byte element)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var j = 0;
            var result = new byte[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != element) Array.Copy(value, i, result, j++, 1);
            }
            Array.Resize(ref result, j);
            return result;
        }

        /// <summary>
        /// Retrieves a subarray from this instance.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="startIndex">The zero-based starting position of a subarray in this instance.</param>
        /// <param name="length">The number of elements in the subarray.</param>
        /// <returns></returns>
        public static byte[] SubArray(this byte[] value, int startIndex, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var result = new Byte[length];
            Array.Copy(value, startIndex, result, 0, length);
            return result;
        }

        /// <summary>
        /// Returns a new array that right-aligns the elements in this instance by padding them on the left with a specified element, for a specified total length.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="totalLength">The number of elements in the resulting array, equal to the number of original elements plus any additional padding elements.</param>
        /// <param name="element">A padding element.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] PadLeft(this byte[] value, int totalLength, byte element)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length >= totalLength)
            {
                return value;
            }

            var result = Enumerable.Repeat(element, totalLength).ToArray();
            Int64 startIndex = totalLength - value.Length;
            Array.Copy(value, 0, result, startIndex, value.Length);
            return result;
        }

        /// <summary>
        /// Returns a new array that left-aligns the elements in this instance by padding them on the right with a specified element, for a specified total length.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="totalLength">The number of elements in the resulting array, equal to the number of original elements plus any additional padding elements.</param>
        /// <param name="element">A padding element.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] PadRight(this byte[] value, int totalLength, byte element)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length >= totalLength)
            {
                return value;
            }

            var result = Enumerable.Repeat(element, totalLength).ToArray();
            Array.Copy(value, 0, result, 0, value.Length);
            return result;
        }

        /// <summary>
        /// Computes the bitwise exclusive-OR of two byte arrays and returns a result.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="array">The second byte array.</param>
        /// <returns>The result byte array.</returns>
        public static byte[] Xor(this byte[] value, byte[] array)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (value.Length != array.Length)
            {
                throw new ArgumentException("Both arrays must have equal lengths.");
            }

            var result = new byte[value.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(value[i] ^ array[i]);
            }
            return result;
        }
    }
}