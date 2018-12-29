#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public interface ILoggerEntry
    {
        DateTime TimeStamp
        {
            get;
        }

        String Message
        {
            get;
        }
    }
}
#endif