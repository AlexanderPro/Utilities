#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Utilities.Logging
{
    public class ConsoleLogger : Logger
    {
        private Encoding _encoding;

        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                SetConsoleEncoding(value);
                _encoding = value;
            }
        }

#region Methods.Public

        public ConsoleLogger()
            : this(Encoding.GetEncoding("cp866"))
        {

        }

        public ConsoleLogger(Encoding encoding)
            : base()
        {
            SetConsoleEncoding(encoding);
            this._encoding = encoding;
        }

        public override void Write(ILoggerEntry entry)
        {
            try
            {
                if (base._enabled)
                {
                    ConsoleColor foregroundColor = ConsoleColor.Gray;

                    if (entry is ConsoleLoggerEntry)
                    {
                        ConsoleLoggerEntry consoleEntry = (ConsoleLoggerEntry)entry;
                        foregroundColor = consoleEntry.Color;
                    }
                    else
                        if (entry is ConsoleExceptionLoggerEntry)
                        {
                            ConsoleExceptionLoggerEntry consoleEntry = (ConsoleExceptionLoggerEntry)entry;
                            foregroundColor = consoleEntry.Color;
                        }
                        else
                            if (entry is ExceptionLoggerEntry)
                            {
                                ExceptionLoggerEntry consoleEntry = (ExceptionLoggerEntry)entry;
                                foregroundColor = ConsoleColor.Red;
                            }
                            else
                                if (entry is LoggerEntry)
                                {
                                    LoggerEntry consoleEntry = (LoggerEntry)entry;
                                    foregroundColor = ConsoleColor.Gray;
                                }

                    Console.ForegroundColor = foregroundColor;
                    Console.WriteLine(entry.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to write entry!", ex);
            }
        }

        private void SetConsoleEncoding(Encoding encoding)
        {
            Console.OutputEncoding = encoding;
        }

#endregion
    }
}
#endif