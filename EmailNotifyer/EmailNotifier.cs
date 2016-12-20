using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using NoCompany.Interfaces;

namespace NoCompany.EmailNotifier
{
    public class EmailNotifier : INotificationManager
    {
        private readonly string _sectionName = "EmailNotifierGroup/MessageFields";
        public EmailNotifier(string sectionName = null)
        {
            if (!String.IsNullOrEmpty(sectionName))
                _sectionName = sectionName;
        }
        public void NotifyAbout<T>(IEnumerable<T> info)
        {
            using (MailMessage mm = CreateMessage(info))
            {
                using (SmtpClient sc = new SmtpClient())
                {
                    sc.Send(mm);
                }
            }
        }

        protected virtual MailMessage CreateMessage<T>(IEnumerable<T> info)
        {
            var messageFields = (MessageFields)ConfigurationManager.GetSection(_sectionName);

            return new MailMessage(messageFields.From, messageFields.To)
            {
                Subject = messageFields.Subject,
                Body = String.Format(messageFields.BodyTemplate, DateTime.UtcNow.ToString(), info.ToHtmlTableString()),
                IsBodyHtml = messageFields.IsBodyHtml
            };
        }

    }
}
