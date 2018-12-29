#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Utilities.Logging
{
    public static class Log
    {
        private static IEnumerable<ILogger> _loggers;
        private static readonly Object _lock = new Object();


        public static void Write(String message)
        {
            Initialize();

            SimpleLoggerEntry entry = new SimpleLoggerEntry(message);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    logger.Write(entry);
                }
            }
        }

        public static void Write(Exception ex)
        {
            Initialize();

            ExceptionLoggerEntry entry = new ExceptionLoggerEntry(ex);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    logger.Write(entry);
                }
            }
        }

        public static void Write(ILoggerEntry entry)
        {
            Initialize();

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    logger.Write(entry);
                }
            }
        }


        public static void WriteToFile(String message)
        {
            Initialize();

            SimpleLoggerEntry entry = new SimpleLoggerEntry(message);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is FileLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToFile(Exception ex)
        {
            Initialize();

            ExceptionLoggerEntry entry = new ExceptionLoggerEntry(ex);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is FileLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToFile(ILoggerEntry entry)
        {
            Initialize();

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is FileLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }


        public static void WriteToEventLog(String message)
        {
            Initialize();

            SimpleLoggerEntry entry = new SimpleLoggerEntry(message);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is EventLogLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToEventLog(Exception ex)
        {
            Initialize();

            ExceptionLoggerEntry entry = new ExceptionLoggerEntry(ex);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is EventLogLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToEventLog(ILoggerEntry entry)
        {
            Initialize();

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is EventLogLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }


        public static void WriteToMail(String message)
        {
            Initialize();

            SimpleLoggerEntry entry = new SimpleLoggerEntry(message);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is MailLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToMail(Exception ex)
        {
            Initialize();

            ExceptionLoggerEntry entry = new ExceptionLoggerEntry(ex);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is MailLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToMail(ILoggerEntry entry)
        {
            Initialize();

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is MailLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }


        public static void WriteToConsole(String message)
        {
            Initialize();

            SimpleLoggerEntry entry = new SimpleLoggerEntry(message);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is ConsoleLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToConsole(Exception ex)
        {
            Initialize();

            ExceptionLoggerEntry entry = new ExceptionLoggerEntry(ex);

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is ConsoleLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }

        public static void WriteToConsole(ILoggerEntry entry)
        {
            Initialize();

            lock (_lock)
            {
                foreach (ILogger logger in _loggers)
                {
                    if (logger is ConsoleLogger)
                    {
                        logger.Write(entry);
                    }
                }
            }
        }


        private static void Initialize()
        {
            _loggers = ConfigurationManager.GetSection("utilities/logging") as IEnumerable<ILogger>;

            if (_loggers == null)
            {
                throw new ArgumentNullException("utilities/logging", "Section is not exists!");
            }
        }
    }
}
#endif