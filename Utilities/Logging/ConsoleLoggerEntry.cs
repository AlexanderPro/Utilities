using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public sealed class ConsoleLoggerEntry : LoggerEntry
    {
        #region Properties.Public

        public ConsoleColor Color
        {
            get;
            set;
        }

        #endregion

        #region Methods.Public

        public ConsoleLoggerEntry(String message)
            : this(DateTime.Now, message, ConsoleColor.Gray)
        {
        }

        public ConsoleLoggerEntry(ILoggerEntry entry)
            : this(entry.TimeStamp, entry.Message, ConsoleColor.Gray)
        {
        }

        public ConsoleLoggerEntry(String message, ConsoleColor color)
            : this(DateTime.Now, message, color)
        {
        }

        public ConsoleLoggerEntry(ILoggerEntry entry, ConsoleColor color)
            : this(entry.TimeStamp, entry.Message, color)
        {
        }

        public ConsoleLoggerEntry(DateTime timeStamp, String message, ConsoleColor color)
            : base(timeStamp, message)
        {
            Color = color;
        }

        #endregion
    }
}