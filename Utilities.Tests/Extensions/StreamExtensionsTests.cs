using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class StreamExtensionsTests
    {
        [TestMethod]
        public void ReadAllBytes()
        {
            var source1 = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            var source2 = new Byte[] {  };
            var memoryStream1 = new MemoryStream(source1, false);
            var memoryStream2 = new MemoryStream(source2, false);

            CollectionAssert.AreEqual(memoryStream1.ReadAllBytes(), source1);
            CollectionAssert.AreEqual(memoryStream2.ReadAllBytes(), source2);
            memoryStream1.Position = 0;
            memoryStream2.Position = 0;
            CollectionAssert.AreNotEqual(memoryStream2.ReadAllBytes(), source1);
            CollectionAssert.AreNotEqual(memoryStream1.ReadAllBytes(), source2);
        }
    }
}
