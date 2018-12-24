using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class ComparableExtensionsTests
    {
        [TestMethod]
        public void IsBetween()
        {
            Assert.IsTrue(10.IsBetween(10, 11));
            Assert.IsTrue(11.IsBetween(10, 11));
            Assert.IsFalse(10.IsBetween(11, 15));
            Assert.IsFalse(11.IsBetween(5, 10));
            Assert.IsTrue(0.IsBetween(-1, 1));
        }
    }
}
