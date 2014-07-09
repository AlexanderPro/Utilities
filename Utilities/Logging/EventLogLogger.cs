using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Utilities.Logging
{
    public class EventLogLogger : Logger
    {
        public EventLog EventLog
        {
            get;
            private set;
        }

        public EventLogLogger(String source, String logName)
        {
            CreateLogSource(source, logName);
            EventLog = new EventLog(logName, Environment.MachineName, source);
        }

        private static void CreateLogSource(String source, String logName)
        {
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }
        }

        public override void Write(ILoggerEntry entry)
        {
            try
            {
                if (base._enabled)
                {
                    EventLogEntryType entryType = EventLogEntryType.Information;
                    if (entry is EventLogLoggerEntry)
                    {
                        EventLogLoggerEntry eventLoggerEntry = (EventLogLoggerEntry)entry;
                        entryType = eventLoggerEntry.EntryType;
                    }
                    this.EventLog.WriteEntry(entry.ToString(), entryType);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to write entry!", ex);
            }
        }
    }
}
