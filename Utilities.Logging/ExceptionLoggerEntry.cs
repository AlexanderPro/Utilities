using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public class ExceptionLoggerEntry : LoggerEntry
    {
        public Exception Exception
        {
            get;
            private set;
        }

        public override String Message
        {
            get
            {
                return ExceptionToString(this.Exception);
            }
            set
            {
                base.Message = value;
            }
        }
 
        public ExceptionLoggerEntry(Exception e) : base()
        {
            Exception = e;
        }

        private String ExceptionToString(Exception e)
        {
            var message = new StringBuilder();

            if (HttpContext.GetCurrentHttpContext() != null)
            {
                message.AppendFormat("URL:            {0}{1}", HttpContext.GetRequestUrl(), Environment.NewLine);
                message.AppendFormat("Remote User:    {0}{1}", HttpContext.GetRequestServerVariableValue("REMOTE_USER") ?? String.Empty, Environment.NewLine);
                message.AppendFormat("Remote Address: {0}{1}", HttpContext.GetRequestServerVariableValue("REMOTE_ADDR") ?? String.Empty, Environment.NewLine);
                message.AppendFormat("Remote Host:    {0}{1}", HttpContext.GetRequestServerVariableValue("REMOTE_HOST") ?? String.Empty, Environment.NewLine);
            }

            for (Exception currEx = e; currEx != null; currEx = currEx.InnerException)
            {
                if (!e.Equals(currEx))
                {
                    message.AppendLine("InnerException");
                }
                message.AppendFormat("Type:        {0}{1}", currEx.GetType().FullName, Environment.NewLine);
                message.AppendFormat("Message:     {0}{1}", currEx.Message, Environment.NewLine);
                message.AppendFormat("Source:      {0}{1}", currEx.Source ?? String.Empty, Environment.NewLine);
                message.AppendFormat("Target Site: {0}{1}", (currEx.TargetSite == null) ? String.Empty : currEx.TargetSite.Name, Environment.NewLine);
                message.AppendFormat("Stack Trace: {0}{1}", currEx.StackTrace ?? String.Empty, Environment.NewLine);
            }
            return message.ToString();
        }
    }
}