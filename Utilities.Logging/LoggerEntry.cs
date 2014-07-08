using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public abstract class LoggerEntry : ILoggerEntry
    {
        public virtual String Message
        {
            get;
            set;
        }

        public virtual DateTime TimeStamp
        {
            get;
            set;
        }

        protected LoggerEntry()
            : this(DateTime.Now, "")
        {
        }

        protected LoggerEntry(DateTime timeStamp, String message)
        {
            TimeStamp = timeStamp;
            Message = message;
        }

        public override String ToString()
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss.ffff} - {1}", TimeStamp, Message);
        }
    }
}