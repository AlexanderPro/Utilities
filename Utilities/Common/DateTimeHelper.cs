using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Common
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Returns all days of the month.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        public static IEnumerable<DateTime> DaysOfMonth(int year, int month)
        {
            return Enumerable.Range(0, DateTime.DaysInMonth(year, month)).Select(day => new DateTime(year, month, day + 1));
        }
    }
}
