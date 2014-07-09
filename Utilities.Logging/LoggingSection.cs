using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Net.Mail;
using Utilities.Logging;

namespace Utilities.Logging
{
    public class LoggingSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            IList<ILogger> _loggers = new List<ILogger>();

            //If section stores in another file.
            XmlAttribute configPath = section.Attributes["configPath"];
            XmlAttribute sectionPath = section.Attributes["sectionPath"];
            if (configPath != null && sectionPath != null)
            {
                XmlDocument document = new XmlDocument();
                document.Load(configPath.Value);
                section = document.SelectSingleNode(sectionPath.Value);
            }

            //Read optional fileLogger section
            XmlNodeList xmlNodes = section.SelectNodes("./fileLogger");
            if (xmlNodes != null)
            {
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    FileLogger fileLogger = ReadFileLoggerSection(xmlNode);
                    _loggers.Add(fileLogger);
                }
            }

            //Read optional eventLogLogger section
            xmlNodes = section.SelectNodes("./eventLogLogger");
            if (xmlNodes != null)
            {
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    EventLogLogger eventLogLogger = ReadEventLogLoggerSection(xmlNode);
                    _loggers.Add(eventLogLogger);
                }
            }

            //Read optional mailLogger section
            xmlNodes = section.SelectNodes("./mailLogger");
            if (xmlNodes != null)
            {
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    MailLogger mailLogger = ReadMailLoggerSection(xmlNode);
                    _loggers.Add(mailLogger);
                }
            }

            //Read optional consoleLogger section
            xmlNodes = section.SelectNodes("./consoleLogger");
            if (xmlNodes != null)
            {
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    ConsoleLogger consoleLogger = ReadConsoleLoggerSection(xmlNode);
                    _loggers.Add(consoleLogger);
                }
            }

            return _loggers;
        }

        #region Methods.Private

        private FileLogger ReadFileLoggerSection(XmlNode xmlNode)
        {
            String fileName;
            Encoding fileEncoding = Encoding.UTF8;
            Boolean enabled = true;

            //Read fileName attribute
            XmlAttribute attribute = xmlNode.Attributes["fileName"];
            if (attribute == null)
            {
                throw new ArgumentNullException("fileName", "fileLogger is not configuried!");
            }
            fileName = attribute.Value;

            //Read optional encoding attribute
            attribute = xmlNode.Attributes["encoding"];
            if (attribute != null)
            {
                fileEncoding = Encoding.GetEncoding(attribute.Value);
            }

            //Read optional enabled attribute
            attribute = xmlNode.Attributes["enabled"];
            if (attribute != null)
            {
                enabled = Boolean.Parse(attribute.Value);
            }

            FileLogger logger = new FileLogger(fileName, fileEncoding);
            logger.Enabled = enabled;
            return logger;
        }

        private EventLogLogger ReadEventLogLoggerSection(XmlNode xmlNode)
        {
            String source, logName;
            Boolean enabled = true;

            //Read source attribute
            XmlAttribute attribute = xmlNode.Attributes["source"];
            if (attribute == null)
            {
                throw new ArgumentNullException("source", "eventLogLogger is not configuried!");
            }
            source = attribute.Value;

            //Read logName attribute
            attribute = xmlNode.Attributes["logName"];
            if (attribute == null)
            {
                throw new ArgumentNullException("logName", "eventLogLogger is not configuried!");
            }
            logName = attribute.Value;

            //Read optional enabled attribute
            attribute = xmlNode.Attributes["enabled"];
            if (attribute != null)
            {
                enabled = Boolean.Parse(attribute.Value);
            }

            EventLogLogger logger = new EventLogLogger(source, logName);
            logger.Enabled = enabled;
            return logger;
        }

        private MailLogger ReadMailLoggerSection(XmlNode xmlNode)
        {
            MailLogger logger = new MailLogger();

            //Read optional enabled attribute
            XmlAttribute attribute = xmlNode.Attributes["enabled"];
            logger.Enabled = attribute == null ? true : Boolean.Parse(attribute.Value);

            //Read server section
            XmlNode innerNode = xmlNode.SelectSingleNode("./server");
            if (innerNode == null)
            {
                throw new ArgumentNullException("server", "mailLogger is not configuried!");
            }

            //Read host attribute
            attribute = innerNode.Attributes["host"];
            if (attribute == null)
            {
                throw new ArgumentNullException("host", "mailLogger is not configuried!");
            }
            logger.ServerName = attribute.Value;

            //Read port attribute
            attribute = innerNode.Attributes["port"];
            logger.Port = attribute == null ? 25 : Int32.Parse(attribute.Value);

            //Read optional userName attribute
            attribute = innerNode.Attributes["userName"];
            if (attribute != null)
            {
                logger.UserName = attribute.Value;
            }

            //Read optional password attribute
            attribute = innerNode.Attributes["password"];
            if (attribute != null)
            {
                logger.Password = attribute.Value;
            }

            //Read optional useSSL attribute
            attribute = innerNode.Attributes["useSSL"];
            if (attribute != null)
            {
                logger.UseSSL = Boolean.Parse(attribute.Value);
            }

            //Read message section
            innerNode = xmlNode.SelectSingleNode("./message");
            if (innerNode == null)
            {
                throw new ArgumentNullException("message", "mailLogger is not configuried!");
            }

            //Read from attribute
            attribute = innerNode.Attributes["from"];
            if (attribute == null)
            {
                throw new ArgumentNullException("from", "mailLogger is not configuried!");
            }

            //Read senderName attribute
            XmlAttribute senderNameAttribute = innerNode.Attributes["senderName"];
            logger.From = senderNameAttribute != null ? new MailAddress(attribute.Value, senderNameAttribute.Value) : new MailAddress(attribute.Value);

            //Read header attribute
            attribute = innerNode.Attributes["header"];
            if (attribute == null)
            {
                throw new ArgumentNullException("header", "mailLogger is not configuried!");
            }
            logger.Subject = attribute.Value;

            //Read optional subjectEncoding attribute
            attribute = innerNode.Attributes["subjectEncoding"];
            logger.SubjectEncoding = attribute == null ? Encoding.UTF8 : Encoding.GetEncoding(attribute.Value);

            //Read optional bodyEncoding attribute
            attribute = innerNode.Attributes["bodyEncoding"];
            logger.BodyEncoding = attribute == null ? Encoding.UTF8 : Encoding.GetEncoding(attribute.Value);

            //Read optional isBodyHtml attribute
            attribute = innerNode.Attributes["isBodyHtml"];
            if (attribute != null)
            {
                logger.IsBodyHtml = Boolean.Parse(attribute.Value);
            }

            //Read TO section
            innerNode = xmlNode.SelectSingleNode("./to");
            if (innerNode == null)
            {
                throw new ArgumentNullException("to", "mailLogger is not configuried!");
            }
            logger.To = ReadMailAddressesSection(innerNode);

            //Read CC section
            innerNode = xmlNode.SelectSingleNode("./cc");
            logger.CC = innerNode == null ? new MailAddressCollection() : ReadMailAddressesSection(innerNode);
            return logger;
        }

        private ConsoleLogger ReadConsoleLoggerSection(XmlNode xmlNode)
        {
            ConsoleLogger logger = new ConsoleLogger();
            Boolean enabled = true;

            //Read optional enabled attribute
            XmlAttribute attribute = xmlNode.Attributes["enabled"];
            if (attribute != null)
            {
                enabled = Boolean.Parse(attribute.Value);
            }
            logger.Enabled = enabled;

            //Read optional encoding attribute
            attribute = xmlNode.Attributes["encoding"];
            logger.Encoding = attribute == null ? Encoding.GetEncoding("cp866") : Encoding.GetEncoding(attribute.Value);
            return logger;
        }

        private MailAddressCollection ReadMailAddressesSection(XmlNode xmlNode)
        {
            MailAddressCollection addresses = new MailAddressCollection();

            //Read add section
            XmlNodeList xmlNodes = xmlNode.SelectNodes("./add");
            if (xmlNodes == null)
            {
                throw new ArgumentNullException("add", "mailLogger is not configuried!");
            }

            foreach (XmlNode innerNode in xmlNodes)
            {
                //Read address attribute
                XmlAttribute addressAttribute = innerNode.Attributes["address"];
                if (addressAttribute == null)
                {
                    throw new ArgumentNullException("address", "mailLogger is not configuried!");
                }

                //Read displayName attribute
                XmlAttribute displayNameAttribute = innerNode.Attributes["displayName"];
                MailAddress address = displayNameAttribute != null ? new MailAddress(addressAttribute.Value, displayNameAttribute.Value) : new MailAddress(addressAttribute.Value);
                addresses.Add(address);
            }
            return addresses;
        }

        #endregion
    }
}