using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void IsNullOrEmpty()
        {
            var source1 = (string)null;
            var source2 = "";
            var source3 = "abcdef";

            Assert.IsTrue(source1.IsNullOrEmpty());
            Assert.IsTrue(source2.IsNullOrEmpty());
            Assert.IsFalse(source3.IsNullOrEmpty());
        }

        [TestMethod]
        public void IsNullOrWhiteSpace()
        {
            var source1 = (string)null;
            var source2 = "";
            var source3 = " ";
            var source4 = "abcdef";

            Assert.IsTrue(source1.IsNullOrWhiteSpace());
            Assert.IsTrue(source2.IsNullOrWhiteSpace());
            Assert.IsTrue(source3.IsNullOrWhiteSpace());
            Assert.IsFalse(source4.IsNullOrWhiteSpace());
        }

        [TestMethod]
        public void IsEqual()
        {
            var source = "abcdef";

            Assert.IsTrue(source.IsEqual("abcdef"));
            Assert.IsFalse(source.IsEqual("abcdef2"));
        }

        [TestMethod]
        public void ToSingleItemArray()
        {
            var source = "abcdef";
            var result1 = new[] { "abcdef" };
            var result2 = new[] { "abcdef2" };
            var result3 = new[] { "abcdef", "abcdef2" };

            CollectionAssert.AreEqual(result1, source.ToSingleItemArray());
            CollectionAssert.AreNotEqual(result2, source.ToSingleItemArray());
            CollectionAssert.AreNotEqual(result3, source.ToSingleItemArray());
        }

        [TestMethod]
        public void NormalizeLineEndings()
        {
            var source = "abcdef\r\nqwerty\r12345678\nABCDEF";
            var expected = "abcdef\r\nqwerty\r\n12345678\r\nABCDEF";

            Assert.AreEqual(expected, source.NormalizeLineEndings());
        }

        [TestMethod]
        public void ToCamelCase()
        {
            var source = "VisualStudio";
            var expected = "visualStudio";

            Assert.AreEqual(expected, source.ToCamelCase());
            Assert.AreNotEqual(source, source.ToCamelCase());
        }

        [TestMethod]
        public void ToPascalCase()
        {
            var source = "visualStudio";
            var expected = "VisualStudio";

            Assert.AreEqual(expected, source.ToPascalCase());
            Assert.AreNotEqual(source, source.ToPascalCase());
        }

        [TestMethod]
        public void ToMd5()
        {
            var source = "12345678";

            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(source);
                var hash = md5.ComputeHash(bytes);
                Assert.AreEqual(BitConverter.ToString(hash).Replace("-", ""), source.ToMd5());
            }
        }

        [TestMethod]
        public void ToEnum()
        {
            var source = "Red";

            Assert.AreEqual(TrafficLight.Red, source.ToEnum<TrafficLight>());
            Assert.AreNotEqual(TrafficLight.Green, source.ToEnum<TrafficLight>());
        }
    }
}