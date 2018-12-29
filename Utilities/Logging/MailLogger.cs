#if NETFULL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Utilities.Net.Email.SMTP;

namespace Utilities.Logging
{
    public class MailLogger : Logger
    {
#region Properties.Public

        public Encoding SubjectEncoding { get; set; }       

        public Encoding BodyEncoding { get; set; }       

        public String Subject { get; set; }

        public String ServerName { get; set; }

        public Int32 Port { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }

        public Boolean UseSSL { get; set; }

        public Boolean IsBodyHtml { get; set; }

        public MailAddress From { get; set; }

        public MailAddressCollection To { get; set; }

        public MailAddressCollection CC { get; set; }

#endregion

#region Methods.Private

        private void SendMessage(String text)
        {
            try
            {
                EmailSender emailSender = new EmailSender()
                {
                    SubjectEncoding = SubjectEncoding,
                    BodyEncoding = BodyEncoding,
                    Subject = Subject,
                    Server = ServerName,
                    Port = Port,
                    UserName = UserName,
                    Password = Password,
                    UseSSL = UseSSL,
                    IsBodyHtml = IsBodyHtml,
                    From = From.Address,
                    To = To.ToList(),
                    CC = CC.ToList(),
                    Body = text
                };
                emailSender.SendMail();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to send the mail!", ex);
            }
        }

#endregion

#region Methods.Public

        public override void Write(ILoggerEntry entry)
        {
            try
            {
                if (base._enabled)
                {
                    this.SendMessage(entry.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to write entry!", ex);
            }
        }

#endregion
    }
}
#endif