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

        public ExceptionLoggerEntry(Exception e)
            : base()
        {
            Exception = e;
        }

        private String ExceptionToString(Exception e)
        {
            StringBuilder sb = new StringBuilder();

            if (System.Web.HttpContext.Current != null)
            {
                sb.AppendFormat("URL:            {0}{1}", System.Web.HttpContext.Current.Request.Url, Environment.NewLine);
                sb.AppendFormat("Remote User:    {0}{1}", System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_USER"] ?? String.Empty, Environment.NewLine);
                sb.AppendFormat("Remote Address: {0}{1}", System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? String.Empty, Environment.NewLine);
                sb.AppendFormat("Remote Host:    {0}{1}", System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"] ?? String.Empty, Environment.NewLine);
            }

            for (Exception currEx = e; currEx != null; currEx = currEx.InnerException)
            {
                if (!e.Equals(currEx))
                {
                    sb.AppendLine("InnerException");
                }
                sb.AppendFormat("Type:        {0}{1}", currEx.GetType().FullName, Environment.NewLine);
                sb.AppendFormat("Message:     {0}{1}", currEx.Message, Environment.NewLine);
                sb.AppendFormat("Source:      {0}{1}", currEx.Source ?? String.Empty, Environment.NewLine);
                sb.AppendFormat("Target Site: {0}{1}", (currEx.TargetSite == null) ? String.Empty : currEx.TargetSite.Name, Environment.NewLine);
                sb.AppendFormat("Stack Trace: {0}{1}", currEx.StackTrace ?? String.Empty, Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
