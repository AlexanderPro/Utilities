using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class EnumExtensionsTests
    {
        [TestMethod]
        public void GetDescription()
        {
            Assert.AreEqual(TrafficLight.Red.GetDescription(), "Red light");
            Assert.AreEqual(TrafficLight.Yellow.GetDescription(), "Yellow light");
            Assert.AreEqual(TrafficLight.Green.GetDescription(), "Green light");
        }

        [TestMethod]
        public void GetDisplayName()
        {
            Assert.AreEqual(TrafficLight.Red.GetDisplayName(), "Red");
            Assert.AreEqual(TrafficLight.Yellow.GetDisplayName(), "Yellow");
            Assert.AreEqual(TrafficLight.Green.GetDisplayName(), "Green");
        }

        [TestMethod]
        public void GetDisplayShortName()
        {
            Assert.AreEqual(TrafficLight.Red.GetDisplayShortName(), "R");
            Assert.AreEqual(TrafficLight.Yellow.GetDisplayShortName(), "Y");
            Assert.AreEqual(TrafficLight.Green.GetDisplayShortName(), "G");
        }

        [TestMethod]
        public void GetDisplayDescription()
        {
            Assert.AreEqual(TrafficLight.Red.GetDisplayDescription(), "Red light");
            Assert.AreEqual(TrafficLight.Yellow.GetDisplayDescription(), "Yellow light");
            Assert.AreEqual(TrafficLight.Green.GetDisplayDescription(), "Green light");
        }

        [TestMethod]
        public void EmptyStringIsReturnedIfDescriptionAttributeIsNotSet()
        {
            Assert.AreEqual(TrafficLight.Unspecified.GetDescription(), String.Empty);
            Assert.AreEqual(TrafficLight.Unspecified.GetDisplayName(), String.Empty);
            Assert.AreEqual(TrafficLight.Unspecified.GetDisplayShortName(), String.Empty);
            Assert.AreEqual(TrafficLight.Unspecified.GetDisplayDescription(), String.Empty);
        }
    }
}
