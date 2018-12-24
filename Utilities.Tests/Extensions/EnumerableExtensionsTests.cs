using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void IsNullOrEmpty()
        {
            var source1 = (IEnumerable)null;
            var source2 = (IEnumerable<string>)null;
            var source3 = Enumerable.Empty<string>();
            var source4 = Enumerable.Range(1, 8);

            Assert.IsTrue(source1.IsNullOrEmpty());
            Assert.IsTrue(source2.IsNullOrEmpty());
            Assert.IsTrue(source3.IsNullOrEmpty());
            Assert.IsFalse(source4.IsNullOrEmpty());
        }

        [TestMethod]
        public void IsNotNullAndIsNotEmpty()
        {
            var source1 = (IEnumerable)null;
            var source2 = (IEnumerable<string>)null;
            var source3 = Enumerable.Empty<string>();
            var source4 = Enumerable.Range(1, 8);

            Assert.IsFalse(source1.IsNotNullAndIsNotEmpty());
            Assert.IsFalse(source2.IsNotNullAndIsNotEmpty());
            Assert.IsFalse(source3.IsNotNullAndIsNotEmpty());
            Assert.IsTrue(source4.IsNotNullAndIsNotEmpty());
        }

        [TestMethod]
        public void WhereIf()
        {
            var source = Enumerable.Range(1, 8);
            var isOdd = true;
            CollectionAssert.AreEqual(new List<int>() { 1, 3, 5, 7 }, source.WhereIf(isOdd, x => x % 2 != 0).ToList());
            CollectionAssert.AreEqual(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 }, source.WhereIf(!isOdd, x => x % 2 != 0).ToList());
        }

        [TestMethod]
        public void WhereIfElse()
        {
            var source = Enumerable.Range(1, 8);
            var isOdd = true;
            CollectionAssert.AreEqual(new List<int>() { 1, 3, 5, 7 }, source.WhereIfElse(isOdd, x => x % 2 != 0, x => x % 2 == 0).ToList());
            CollectionAssert.AreEqual(new List<int>() { 2, 4, 6, 8 }, source.WhereIfElse(!isOdd, x => x % 2 != 0, x => x % 2 == 0).ToList());
        }

    }
}
