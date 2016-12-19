using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace EmailNotifyer
{
    public class EmailNotifyer
    {
        public void Notify()
        {
            using (MailMessage mm = new MailMessage("you@yourcompany.com", "you@yourcompany.com"))
            {
                mm.Subject = "Mail Subject";
                mm.Body = "Mail Body";
                mm.IsBodyHtml = false;
                using (SmtpClient sc = new SmtpClient())
                {
                    sc.EnableSsl = true;
                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sc.UseDefaultCredentials = false;
                    sc.Credentials = new NetworkCredential("", "");
                    sc.Send(mm);
                }
            }
        }
    }
}
