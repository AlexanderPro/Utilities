#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public interface ILogger
    {
        void Write(ILoggerEntry entry);
    }
}
#endif