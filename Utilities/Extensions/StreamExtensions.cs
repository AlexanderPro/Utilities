using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Stream.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Read all bytes from a stream.
        /// </summary>
        /// <param name="stream">The instance of the <see cref="Stream"/> class.</param>
        /// <returns>Byte array.</returns>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
