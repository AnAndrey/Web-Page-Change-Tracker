using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using SharedInterfaces;

namespace EmailNotifyer
{
    public class EmailNotifyer:INotificationManager
    {

        public static string ToHtmlTable<T>(IEnumerable<T> list)
        {
            var result = new StringBuilder();
            result.Append("<table style  > ");

            result.AppendFormat("<th >{0}</th >", "Description");

            foreach (var item in list)
            {
                result.AppendFormat("<tr >");
                
                result.AppendFormat("<td >{0}</td >", item );
                
                result.AppendLine("</tr >");
            }
        
            result.Append("</table >"); 
            return result.ToString();
        }

        public void NotifyAbout(IEnumerable<string> info)
        {
            var messageFields = (MessageFields)ConfigurationManager.GetSection("EmailNotifierGroup/MessageFields");

            using (MailMessage mm = new MailMessage(messageFields.From, messageFields.To))
            {
                mm.Subject = messageFields.Subject;
                mm.Body = String.Format(messageFields.BodyTemplate, DateTime.UtcNow.ToString(), ToHtmlTable(info));
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

        [ConfigurationProperty("BodyTemplate", DefaultValue = "", IsRequired = false)]
        public string BodyTemplate
        {
            get
            {
                return (string)this["BodyTemplate"];
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
