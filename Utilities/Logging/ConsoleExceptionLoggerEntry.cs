#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public class ConsoleExceptionLoggerEntry : ExceptionLoggerEntry
    {
#region Properties.Public

        public ConsoleColor Color
        {
            get;
            set;
        }

#endregion

#region Methods.Public

        public ConsoleExceptionLoggerEntry(Exception e)
            : this(e, ConsoleColor.Red)
        {
        }

        public ConsoleExceptionLoggerEntry(Exception e, ConsoleColor color)
            : base(e)
        {
            Color = color;
        }

#endregion
    }
}
#endif