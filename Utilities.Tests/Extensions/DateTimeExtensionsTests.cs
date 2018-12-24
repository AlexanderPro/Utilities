using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;

namespace Utilities.Tests.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void ToUnixTimeSeconds()
        {
            var source = DateTime.Now;
            var result = source.ToUnixTimeSeconds().FromUnixTimeSeconds();
            source = source.AddTicks(-(source.Ticks % TimeSpan.TicksPerSecond));
            result = result.AddTicks(-(result.Ticks % TimeSpan.TicksPerSecond));

            Assert.AreEqual(source, result);
        }

        [TestMethod]
        public void ToEndDay()
        {
            var currentDate = DateTime.Now;
            var source = currentDate.Date.AddDays(1).AddMilliseconds(-1);
            var result = currentDate.ToEndDay();

            Assert.AreEqual(source, result);
        }
    }
}
