using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Common.ExtensionMethods
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts string to byte array.
        /// </summary>
        /// <param name="value">The instance of the <see cref="String"/> class.</param>
        /// <param name="hexElement">Does the string contain hexadecimal elements?</param>
        /// <returns>Byte array.</returns>
        public static Byte[] ToByteArray(this String value, Boolean hexElement = true)
        {
            if (String.IsNullOrEmpty(value)) return new Byte[0];

            if (hexElement)
            {
                var data = new Byte[value.Length / 2];
                for (Int32 i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
                }
                return data;
            }
            else
            {
                var data = new Byte[value.Length];
                for (Int32 i = 0; i < data.Length; i++)
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
        public static String ToByteString(this Byte[] value, String delimiter = "")
        {
            if (value == null || value.Length == 0) return String.Empty;
            if (delimiter == null) throw new ArgumentNullException("delimiter");

            var sb = new StringBuilder(value.Length);
            foreach (Byte b in value)
            {
                sb.AppendFormat("{0:X2}{1}", b, delimiter);
            }
            return sb.ToString().TrimEnd(delimiter.ToCharArray());
        }

        /// <summary>
        /// Converts byte array to string.
        /// </summary>
        /// <param name="value">The byte array.</param>
        /// <returns>The string of form 1234567890.</returns>
        public static String ToIntString(this Byte[] value)
        {
            if (value == null || value.Length == 0) return String.Empty;

            var result = String.Concat(value.Select(x => x.ToString()));
            return result;
        }

        /// <summary>
        /// Removes all leading occurrences of byte from the current object.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="element">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static Byte[] TrimStart(this Byte[] value, Byte element)
        {
            if (value == null) throw new ArgumentNullException("value");

            Int32 i = 0;
            while (value[i] == element) ++i;
            Byte[] result = new Byte[value.Length - i];
            Array.Copy(value, i, result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Removes all trailing occurrences of byte from the current object. 
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="element">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static Byte[] TrimEnd(this Byte[] value, Byte element)
        {
            if (value == null) throw new ArgumentNullException("value");

            Int32 i = value.Length - 1;
            while (value[i] == element) --i;
            Byte[] result = new Byte[i + 1];
            Array.Copy(value, result, i + 1);
            return result;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of byte from the current object.
        /// </summary>
        /// <param name="value">The source byte array.</param>
        /// <param name="element">The byte to remove.</param>
        /// <returns>The result byte array.</returns>
        public static Byte[] Trim(this Byte[] value, Byte element)
        {
            return value.TrimStart(element).TrimEnd(element);
        }

        /// <summary>
        /// Concatenates two specified instances of byte array. 
        /// </summary>
        /// <param name="value">The source byte array to concatenate.</param>
        /// <param name="array">The second byte array to concatenate.</param>
        /// <returns>The concatenation of source and second byte arrays.</returns>
        public static Byte[] Concat(this Byte[] value, Byte[] array)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (array == null) throw new ArgumentNullException("array");

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
        public static Boolean IsEqual(this Byte[] value, Byte[] array)
        {
            if (value == null || array == null || value.Length != array.Length) return false;

            for (Int32 i = 0; i < array.Length; i++)
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
        public static Byte[] Remove(this Byte[] value, Byte element)
        {
            if (value == null) throw new ArgumentNullException("value");

            Int32 j = 0;
            var result = new Byte[value.Length];
            for (Int32 i = 0; i < value.Length; i++)
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
        public static Byte[] SubArray(this Byte[] value, Int32 startIndex, Int32 length)
        {
            if (value == null) throw new ArgumentNullException("value");

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
        public static Byte[] PadLeft(this Byte[] value, Int32 totalLength, Byte element)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length >= totalLength) return value;

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
        public static Byte[] PadRight(this Byte[] value, Int32 totalLength, Byte element)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length >= totalLength) return value;

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
        public static Byte[] Xor(this Byte[] value, Byte[] array)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (array == null) throw new ArgumentNullException("array");
            if (value.Length != array.Length) throw new ArgumentException("Both arrays must have equal lengths.");

            var result = new Byte[value.Length];
            for (Int32 i = 0; i < result.Length; i++)
            {
                result[i] = (Byte)(value[i] ^ array[i]);
            }
            return result;
        }
    }
}