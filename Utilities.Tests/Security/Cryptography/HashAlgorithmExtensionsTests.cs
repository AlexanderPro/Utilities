using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Security.Cryptography.Extensions;

namespace Utilities.Tests.Security.Cryptography
{
    [TestClass]
    public class HashAlgorithmExtensionsTests
    {
        [TestMethod]
        public void TestMd5Algorithm()
        {
            using (var algorithm = MD5.Create())
            {
                TestHashAlgorithm(algorithm);
            }
        }

        [TestMethod]
        public void TestSha1Algorithm()
        {
            using (var algorithm = SHA1.Create())
            {
                TestHashAlgorithm(algorithm);
            }
        }

        [TestMethod]
        public void TestSha256Algorithm()
        {
            using (var algorithm = SHA256.Create())
            {
                TestHashAlgorithm(algorithm);
            }
        }

        [TestMethod]
        public void TestSha384Algorithm()
        {
            using (var algorithm = SHA384.Create())
            {
                TestHashAlgorithm(algorithm);
            }
        }

        [TestMethod]
        public void TestSha512Algorithm()
        {
            using (var algorithm = SHA512.Create())
            {
                TestHashAlgorithm(algorithm);
            }
        }


        private void TestHashAlgorithm(HashAlgorithm algorithm)
        {
            var source = "qwerty12345678";
            var bytes = Encoding.UTF8.GetBytes(source);
            var hash = algorithm.ComputeHash(bytes);
            var result1 = Convert.ToBase64String(hash);
            var result2 = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Assert.AreEqual(result1, algorithm.ComputeHash(source, true));
            Assert.AreEqual(result2, algorithm.ComputeHash(source, false));
            Assert.AreEqual(result2, algorithm.ComputeHash(source));
        }
    }
}