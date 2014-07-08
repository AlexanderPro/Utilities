using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Common.ExtensionMethods;

namespace Utilities.Tests.Common
{
    [TestClass]
    public class EnumExtensionsTests
    {
        [TestMethod]
        public void GetDescriptionOfEnum()
        {
            Assert.AreEqual(TrafficLight.Green.GetDescription(), "Green light.");
        }

        [TestMethod]
        public void EmptyStringIsReturnedIfDescriptionAttributeIsNotSet()
        {
            Assert.AreEqual(TrafficLight.Unspecified.GetDescription(), String.Empty);
        }
    }
}
