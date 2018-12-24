using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
#if NETFX_40 || NETFX_45
        /// <summary>
        /// Returns the number of seconds that have elapsed since 1970-01-01T00:00:00Z.
        /// </summary>
        /// <param name="dateTime">The local DateTime</param>
        /// <returns>The number of seconds</returns>
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var diff = dateTime.ToUniversalTime() - epoch;
            return (long)diff.TotalSeconds;
        }

        /// <summary>
        /// Converts a Unix time expressed as the number of seconds that have elapsed since 1970-01-01T00:00:00Z to a DateTime value.
        /// </summary>
        /// <param name="unixTime">A Unix Time</param>
        /// <returns>A date and time value that represents the same moment in time as the Unix time.</returns>
        public static DateTime FromUnixTimeSeconds(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = TimeSpan.FromSeconds(unixTime);
            return epoch.Add(timeSpan).ToLocalTime();
        }
#endif
        /// <summary>
        /// Gets the value of the end of the day (23:59)
        /// </summary>
        public static DateTime ToEndDay(this DateTime target)
        {
            return target.Date.AddDays(1).AddMilliseconds(-1);
        }
    }
}
