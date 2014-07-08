using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Common.ExtensionMethods;

namespace Utilities.Tests.Common
{
    [TestClass]
    public class TypeConversionExtensionsTests
    {
        [TestMethod]
        public void TestObjectIsNull()
        {
            Object obj1 = (String)null;
            Object obj2 = (StringBuilder)null;
            Object obj3 = null;
            Object obj4 = new List<String>();
            Object obj5 = DBNull.Value;

            Assert.IsTrue(obj1.IsNull());
            Assert.IsTrue(obj2.IsNull());
            Assert.IsTrue(obj3.IsNull());
            Assert.IsFalse(obj4.IsNull());
            Assert.IsFalse(obj5.IsNull());
        }

        [TestMethod]
        public void TestObjectIsNotNull()
        {
            Object obj1 = (String)null;
            Object obj2 = (StringBuilder)null;
            Object obj3 = null;
            Object obj4 = new List<String>();
            Object obj5 = DBNull.Value;

            Assert.IsFalse(obj1.IsNotNull());
            Assert.IsFalse(obj2.IsNotNull());
            Assert.IsFalse(obj3.IsNotNull());
            Assert.IsTrue(obj4.IsNotNull());
            Assert.IsTrue(obj5.IsNotNull());
        }

        [TestMethod]
        public void TestObjectIsNullOrDbNull()
        {
            Object obj1 = (String)null;
            Object obj2 = (StringBuilder)null;
            Object obj3 = null;
            Object obj4 = new List<String>();
            Object obj5 = DBNull.Value;

            Assert.IsTrue(obj1.IsNullOrDBNull());
            Assert.IsTrue(obj2.IsNullOrDBNull());
            Assert.IsTrue(obj3.IsNullOrDBNull());
            Assert.IsFalse(obj4.IsNullOrDBNull());
            Assert.IsTrue(obj5.IsNullOrDBNull());
        }

        [TestMethod]
        public void TestObjectIsNotNullOrDbNull()
        {
            Object obj1 = (String)null;
            Object obj2 = (StringBuilder)null;
            Object obj3 = null;
            Object obj4 = new List<String>();
            Object obj5 = DBNull.Value;

            Assert.IsFalse(obj1.IsNotNullOrDBNull());
            Assert.IsFalse(obj2.IsNotNullOrDBNull());
            Assert.IsFalse(obj3.IsNotNullOrDBNull());
            Assert.IsTrue(obj4.IsNotNullOrDBNull());
            Assert.IsFalse(obj5.IsNotNullOrDBNull());
        }

        [TestMethod]
        public void ConvertObjectToAnotherType()
        {
            Decimal obj1 = 5m;
            Double obj2 = 5f;

            Assert.AreEqual(5, obj1.ConvertTo<Decimal, Int32>());
            Assert.AreEqual(5, obj2.ConvertTo<Double, Int32>());
        }

        [TestMethod]
        public void ConvertObjectToNullableType()
        {
            Object source1 = null;
            Object source2 = DBNull.Value;
            Int32? result1 = null;

            Assert.AreEqual(result1, source1.ConvertTo<Int32>());
            Assert.AreEqual(result1, source2.ConvertTo<Int32>());
        }
    }
}
