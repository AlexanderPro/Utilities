using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        public void AsEnumerable()
        {
            var trafficLight = TrafficLight.Green;
            var fileAttributes = FileAttributes.System | FileAttributes.Hidden;
            CollectionAssert.AreEqual(new List<TrafficLight> { TrafficLight.Red, TrafficLight.Yellow, TrafficLight.Green, TrafficLight.Unspecified }, EnumExtensions.AsEnumerable<TrafficLight>().ToList());
            CollectionAssert.AreNotEqual(new List<TrafficLight> { TrafficLight.Red, TrafficLight.Yellow, TrafficLight.Green }, EnumExtensions.AsEnumerable<TrafficLight>().ToList());
            CollectionAssert.AreEqual(new List<TrafficLight> { TrafficLight.Red, TrafficLight.Yellow, TrafficLight.Green, TrafficLight.Unspecified }, trafficLight.AsEnumerable(false).ToList());
            CollectionAssert.AreNotEqual(new List<TrafficLight> { TrafficLight.Red, TrafficLight.Yellow, TrafficLight.Green }, trafficLight.AsEnumerable(false).ToList());
            CollectionAssert.AreEqual(new List<FileAttributes> { FileAttributes.Hidden, FileAttributes.System }, fileAttributes.AsEnumerable().ToList());
            CollectionAssert.AreNotEqual(new List<FileAttributes> { FileAttributes.Hidden, FileAttributes.System, FileAttributes.Compressed }, fileAttributes.AsEnumerable().ToList());
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
