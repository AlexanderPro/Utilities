#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Utilities.Logging
{
    public sealed class EventLogLoggerEntry : LoggerEntry
    {
#region Properties.Public

        public EventLogEntryType EntryType
        {
            get;
            set;
        }

#endregion

#region Methods.Public

        public EventLogLoggerEntry(String message)
            : this(DateTime.Now, message, EventLogEntryType.Information)
        {
        }

        public EventLogLoggerEntry(ILoggerEntry entry)
            : this(entry.TimeStamp, entry.Message, EventLogEntryType.Information)
        {
        }

        public EventLogLoggerEntry(String message, EventLogEntryType entryType)
            : this(DateTime.Now, message, entryType)
        {
        }

        public EventLogLoggerEntry(ILoggerEntry entry, EventLogEntryType entryType)
            : this(entry.TimeStamp, entry.Message, entryType)
        {
        }

        public EventLogLoggerEntry(DateTime timeStamp, String message, EventLogEntryType entryType)
            : base(timeStamp, message)
        {
            EntryType = entryType;
        }

#endregion
    }
}
#endif