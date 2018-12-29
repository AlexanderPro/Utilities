#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public sealed class SimpleLoggerEntry : LoggerEntry
    {
        public SimpleLoggerEntry(String message)
            : base(DateTime.Now, message)
        {
        }

        public SimpleLoggerEntry(DateTime timeStamp, String message)
            : base(timeStamp, message)
        {
        }
    }
}
#endif