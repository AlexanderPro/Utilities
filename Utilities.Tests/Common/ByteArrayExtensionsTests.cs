using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Common.ExtensionMethods;

namespace Utilities.Tests.Common
{
    [TestClass]
    public class ByteArrayExtensionsTests
    {
        [TestMethod]
        public void ConvertStringToByteArray()
        {
            var source1 = "000102030405060708090A0B0C0D0E0F";
            var result1 = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            var source2 = "0123456789";
            var result2 = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };

            CollectionAssert.AreEqual(result1, source1.ToByteArray());
            CollectionAssert.AreEqual(result1, source1.ToByteArray(true));
            CollectionAssert.AreEqual(result2, source2.ToByteArray(false));
        }

        [TestMethod]
        public void ConvertByteArrayToByteString()
        {
            var source = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            var result1 = "000102030405060708090A0B0C0D0E0F";
            var result2 = "00-01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F";

            Assert.AreEqual(result1, source.ToByteString());
            Assert.AreEqual(result1, source.ToByteString(""));
            Assert.AreEqual(result2, source.ToByteString("-"));
        }

        [TestMethod]
        public void ConvertByteArrayToIntString()
        {
            var source = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            var result = "0123456789101112131415";

            Assert.AreEqual(result, source.ToIntString());
        }

        [TestMethod]
        public void TrimStartByteArray()
        {
            var source = new Byte[] { 0x05, 0x05, 0x05, 0x05, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var result = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00 };

            CollectionAssert.AreEqual(result, source.TrimStart(0x05));
            CollectionAssert.AreEqual(source, source.TrimStart(0x07));
        }

        [TestMethod]
        public void TrimEndByteArray()
        {
            var source = new Byte[] { 0x05, 0x05, 0x05, 0x05, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var result = new Byte[] { 0x05, 0x05, 0x05, 0x05, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05 };

            CollectionAssert.AreEqual(result, source.TrimEnd(0x00));
            CollectionAssert.AreEqual(source, source.TrimEnd(0x01));
        }

        [TestMethod]
        public void TrimByteArray()
        {
            var source = new Byte[] { 0x05, 0x05, 0x05, 0x05, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05 };
            var result = new Byte[] { 0x01, 0x02, 0x03, 0x04 };

            CollectionAssert.AreEqual(result, source.Trim(0x05));
            CollectionAssert.AreEqual(source, source.Trim(0x07));
        }

        [TestMethod]
        public void ConcatByteArrays()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var source2 = new Byte[] { 0x06, 0x07, 0x08, 0x09, 0x0A };
            var source3 = new Byte[] { };

            var result1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var result2 = new Byte[] { 0x06, 0x07, 0x08, 0x09, 0x0A };
            var result3 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };
            var result4 = new Byte[] { };

            CollectionAssert.AreEqual(result1, source1.Concat(source3));
            CollectionAssert.AreEqual(result2, source2.Concat(source3));
            CollectionAssert.AreEqual(result3, source1.Concat(source2));
            CollectionAssert.AreEqual(result4, source3.Concat(source3));
        }

        [TestMethod]
        public void CompareByteArrays()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var source2 = new Byte[] { 0x06, 0x07, 0x08, 0x09, 0x0A };
            var source3 = new Byte[] { 0x06, 0x07, 0x08, 0x09, 0x0A };

            Assert.IsFalse(source1.IsEqual(source2));
            Assert.IsTrue(source2.IsEqual(source3));
        }

        [TestMethod]
        public void RemoveElementFromByteArray()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x02, 0x06, 0x07, 0x02, 0x02, 0x01 };
            var result1 = new Byte[] { 0x01, 0x03, 0x04, 0x05, 0x06, 0x07, 0x01 };
            var result2 = new Byte[] { 0x02, 0x03, 0x04, 0x05, 0x02, 0x06, 0x07, 0x02, 0x02 };

            CollectionAssert.AreEqual(result1, source1.Remove(0x02));
            CollectionAssert.AreEqual(result2, source1.Remove(0x01));
            CollectionAssert.AreNotEqual(result1, source1.Remove(0x00));
            CollectionAssert.AreNotEqual(result2, source1.Remove(0x00));
        }

        [TestMethod]
        public void GetSubArrayFromByteArray()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
            var result1 = new Byte[] { 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
            var result2 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            var result3 = new Byte[] { 0x08, 0x09 };

            CollectionAssert.AreEqual(result1, source1.SubArray(2, 7));
            CollectionAssert.AreEqual(result2, source1.SubArray(0, 7));
            CollectionAssert.AreEqual(result3, source1.SubArray(7, 2));
        }

        [TestMethod]
        public void PadLeftByteArray()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
            var result1 = new Byte[] { 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };

            CollectionAssert.AreEqual(result1, source1.PadLeft(11, 0x00));
        }

        [TestMethod]
        public void PadRightByteArray()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
            var result1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00 };

            CollectionAssert.AreEqual(result1, source1.PadRight(11, 0x00));
        }

        [TestMethod]
        public void XorByteArrays()
        {
            var source1 = new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            var source2 = new Byte[] { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
            var result = new Byte[] { 0x03, 0x01, 0x07, 0x01, 0x03, 0x01, 0x0F, 0x01 };

            CollectionAssert.AreEqual(result, source1.Xor(source2));
        }
    }
}
