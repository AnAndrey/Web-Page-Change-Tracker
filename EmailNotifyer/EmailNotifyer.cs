using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;

namespace EmailNotifyer
{
    public class EmailNotifyer
    {
        public void Notify()
        {
            var messageFields = (MessageFields)ConfigurationManager.GetSection("EmailNotifierGroup/MessageFields");
            
            using (MailMessage mm = new MailMessage(messageFields.From, messageFields.To))
            {
                mm.Subject = messageFields.Subject;
                mm.Body = messageFields.Body;
                mm.IsBodyHtml = messageFields.IsBodyHtml;
                using (SmtpClient sc = new SmtpClient())
                {
                    sc.Send(mm);
                }
            }
        }
    }

    public class MessageFields : ConfigurationSection
    {
        // Create a "remoteOnly" attribute.
        [ConfigurationProperty("Subject", DefaultValue = "false", IsRequired = false)]
        public string Subject
        {
            get
            {
                return (string)this["Subject"];
            }
        }

        [ConfigurationProperty("From", DefaultValue = "", IsRequired = false)]
        public string From
        {
            get
            {
                return (string)this["From"];
            }
        }

        [ConfigurationProperty("To", DefaultValue = "", IsRequired = false)]
        public string To
        {
            get
            {
                return (string)this["To"];
            }
        }

        [ConfigurationProperty("Body", DefaultValue = "", IsRequired = false)]
        public string Body
        {
            get
            {
                return (string)this["Body"];
            }
        }

        [ConfigurationProperty("IsBodyHtml", DefaultValue = "false", IsRequired = false)]
        public bool IsBodyHtml
        {
            get
            {
                return (bool)this["IsBodyHtml"];
            }
        }
    }
}
