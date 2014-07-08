using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Linq;

namespace Utilities.Net.Email.SMTP
{
    public class EmailSender
    {
        #region Fields.Private

        private readonly Char[] SPLITTER = { ',', ';' };

        #endregion

        #region Methods.Public

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EmailSender()
        {
            Attachments = new List<Attachment>();
            EmbeddedResources = new List<LinkedResource>();
            To = new List<MailAddress>();
            CC = new List<MailAddress>();
            Bcc = new List<MailAddress>();
            Priority = MailPriority.Normal;
            IsBodyHtml = true;
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="Message">The body of the message</param>
        public void SendMail(String Message)
        {
            Body = Message;
            SendMail();
        }

        /// <summary>
        /// Sends a piece of mail asynchronous
        /// </summary>
        /// <param name="Message">Message to be sent</param>
        public void SendMailAsync(String Message)
        {
            Body = Message;
            ThreadPool.QueueUserWorkItem(delegate { SendMail(); });
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        public void SendMail()
        {
            using (MailMessage message = new MailMessage())
            {
                foreach (MailAddress address in To)
                {
                    message.To.Add(address);
                }
                foreach (MailAddress address in CC)
                {
                    message.CC.Add(address);
                }
                foreach (MailAddress address in Bcc)
                {
                    message.Bcc.Add(address);
                }
                message.Subject = Subject;
                message.From = String.IsNullOrEmpty(SenderName) ? new MailAddress(From) : new MailAddress(From, SenderName);
                if (UseAlternateView)
                {
                    AlternateView bodyView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
                    foreach (LinkedResource resource in EmbeddedResources)
                    {
                        bodyView.LinkedResources.Add(resource);
                    }
                    message.AlternateViews.Add(bodyView);
                }
                message.Body = Body;
                message.Priority = Priority;
                message.SubjectEncoding = SubjectEncoding ?? Encoding.GetEncoding("ISO-8859-1");
                message.BodyEncoding = BodyEncoding ?? Encoding.GetEncoding("ISO-8859-1");
                message.IsBodyHtml = IsBodyHtml;
                foreach (Attachment attachment in Attachments)
                {
                    message.Attachments.Add(attachment);
                }
                SmtpClient smtp = new SmtpClient(Server, Port);
                if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
                }
                smtp.EnableSsl = UseSSL;
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Sends a piece of mail asynchronous
        /// </summary>
        public void SendMailAsync()
        {
            ThreadPool.QueueUserWorkItem(delegate { SendMail(); });
        }

        #endregion

        #region Properties.Public

        /// <summary>
        /// Whom the message is to
        /// </summary>
        public String StringTo
        {
            get
            {
                return To.Count == 0 ? "" : To.Count == 1 ? To[0].Address : To.Select(x => x.Address).Aggregate((a, b) => a + "," + b);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("StringTo");
                To.Clear();
                To = value.Split(SPLITTER).Where(a => !String.IsNullOrEmpty(a.Trim())).Select(a => new MailAddress(a)).ToList();
            }
        }

        /// <summary>
        /// Carbon copy send (seperate email addresses with a comma)
        /// </summary>
        public String StringCC
        {
            get
            {
                return CC.Count == 0 ? "" : CC.Count == 1 ? CC[0].Address : CC.Select(x => x.Address).Aggregate((a, b) => a + "," + b);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("StringCC");
                CC.Clear();
                CC = value.Split(SPLITTER).Where(a => !String.IsNullOrEmpty(a.Trim())).Select(a => new MailAddress(a)).ToList();
            }
        }

        /// <summary>
        /// Blind carbon copy send (seperate email addresses with a comma)
        /// </summary>
        public String StringBcc
        {
            get
            {
                return Bcc.Count == 0 ? "" : Bcc.Count == 1 ? Bcc[0].Address : Bcc.Select(x => x.Address).Aggregate((a, b) => a + "," + b);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("StringBcc");
                Bcc.Clear();
                Bcc = value.Split(SPLITTER).Where(a => !String.IsNullOrEmpty(a.Trim())).Select(a => new MailAddress(a)).ToList();
            }
        }

        /// <summary>
        /// Whom the message is to
        /// </summary>
        public IList<MailAddress> To { get; set; }

        /// <summary>
        /// Carbon copy send
        /// </summary>
        public IList<MailAddress> CC { get; set; }

        /// <summary>
        /// Blind carbon copy send
        /// </summary>
        public IList<MailAddress> Bcc { get; set; }

        /// <summary>
        /// Any attachments that are included with this
        /// message.
        /// </summary>
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Any attachment (usually images) that need to be embedded in the message
        /// </summary>
        public IList<LinkedResource> EmbeddedResources { get; set; }

        /// <summary>
        /// The priority of this message
        /// </summary>
        public MailPriority Priority { get; set; }

        /// <summary>
        /// Server Location
        /// </summary>
        public String Server { get; set; }

        /// <summary>
        /// User Name for the server
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Password for the server
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Port to send the information on
        /// </summary>
        public Int32 Port { get; set; }

        /// <summary>
        /// Decides whether we are using STARTTLS (SSL) or not
        /// </summary>
        public Boolean UseSSL { get; set; }

        /// <summary>
        /// The subject of the email
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// Whom the message is from
        /// </summary>
        public String From { get; set; }

        /// <summary>
        /// Whom the message is name
        /// </summary>
        public String SenderName { get; set; }

        /// <summary>
        /// Body of the text
        /// </summary>
        public String Body { get; set; }

        /// <summary>
        /// Encoding of subject
        /// </summary>
        public Encoding SubjectEncoding { get; set; }

        /// <summary>
        /// Encoding of body
        /// </summary>
        public Encoding BodyEncoding { get; set; }

        /// <summary>
        /// Is body of message html
        /// </summary>
        public Boolean IsBodyHtml { get; set; }

        /// <summary>
        /// Creates alternate view
        /// </summary>
        public Boolean UseAlternateView { get; set; }

        #endregion
    }
}