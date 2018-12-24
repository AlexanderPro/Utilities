using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class CollectionExtensionsTests
    {
        [TestMethod]
        public void IsNullOrEmpty()
        {
            ICollection<string> source1 = null;
            ICollection<string> source2 = new List<string>();
            ICollection<string> source3 = new List<string> { "A", "B" };

            Assert.IsTrue(source1.IsNullOrEmpty());
            Assert.IsTrue(source2.IsNullOrEmpty());
            Assert.IsFalse(source3.IsNullOrEmpty());
        }

        [TestMethod]
        public void SplitToParts()
        {
            var source = Enumerable.Range(1, 8).ToList();
            var split = source.SplitToParts(3);

            Assert.AreEqual(3, split.Count);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, (List<int>)split[0]);
            CollectionAssert.AreEqual(new List<int> { 4, 5, 6 }, (List<int>)split[1]);
            CollectionAssert.AreEqual(new List<int> { 7, 8 }, (List<int>)split[2]);
        }

        [TestMethod]
        public void AddIfNotContains()
        {
            var source = Enumerable.Range(1, 8).ToList();

            Assert.IsTrue(source.AddIfNotContains(9));
            Assert.IsFalse(source.AddIfNotContains(8));
            Assert.AreEqual(9, source.Count);
        }
    }
}
