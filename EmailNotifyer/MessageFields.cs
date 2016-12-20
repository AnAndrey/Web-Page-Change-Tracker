using System.Configuration;

namespace NoCompany.EmailNotifier
{
    internal class MessageFields : ConfigurationSection
    {
        [ConfigurationProperty("Subject", DefaultValue = "false")]
        public string Subject
        {
            get
            {
                return (string)this["Subject"];
            }
        }

        [ConfigurationProperty("From", DefaultValue = "")]
        public string From
        {
            get
            {
                return (string)this["From"];
            }
        }

        [ConfigurationProperty("To", DefaultValue = "")]
        public string To
        {
            get
            {
                return (string)this["To"];
            }
        }

        [ConfigurationProperty("BodyTemplate", DefaultValue = "")]
        public string BodyTemplate
        {
            get
            {
                return (string)this["BodyTemplate"];
            }
        }

        [ConfigurationProperty("IsBodyHtml", DefaultValue = "false")]
        public bool IsBodyHtml
        {
            get
            {
                return (bool)this["IsBodyHtml"];
            }
        }
    }
}
