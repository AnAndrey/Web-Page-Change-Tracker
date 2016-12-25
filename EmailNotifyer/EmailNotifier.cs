using System;
using System.Collections.Generic;
using CodeContracts;
using System.Net.Mail;
using System.Configuration;
using NoCompany.Interfaces;
using Newtonsoft.Json;
using log4net;
using NoCompany.EmailNotifier.Properties;

namespace NoCompany.EmailNotifier
{
    public class EmailNotifier : INotificationManager
    {
        public static ILog logger = LogManager.GetLogger(typeof(EmailNotifier));

        private readonly string _sectionName = "EmailNotifierGroup/MessageFields";
        public EmailNotifier() { }
        public EmailNotifier(string sectionName)
        {
            Requires.NotNullOrEmpty(sectionName, "sectionName");
            _sectionName = sectionName;
        }
        public void NotifyAbout<T>(IEnumerable<T> info)
        {
            Requires.NotNullOrEmpty(info, "info");
            try
            {
                using (MailMessage message = CreateMessage(info))
                {
                    using (SmtpClient sc = new SmtpClient())
                    {
                        LogMailStuff(message, sc);

                        sc.Send(message);
                    }
                }
            }
            catch (SmtpException ex)
            {
                logger.ErrorFormat(Resources.Error_SendMailFail, ex.Message);
            } 
        }
        private void LogMailStuff(MailMessage message, SmtpClient sc)
        {
            var specificTypesConverter = new SpecificTypesConverter(new Type[] { typeof(string), typeof(int), typeof(MailAddress) });
            var jsonMessage = JsonConvert.SerializeObject(message,
                                        Formatting.Indented,
                                        specificTypesConverter);
            logger.InfoFormat(Resources.Info_NotificationMessage, jsonMessage);
            var jsonSmtpClient = JsonConvert.SerializeObject(sc, Formatting.Indented, specificTypesConverter);
            logger.InfoFormat(Resources.Info_SmtpSettings, jsonSmtpClient);
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