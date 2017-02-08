using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mail;

namespace youviame.API.Controllers
{
    public class Email
    {
        public Email()
        {
        }



        //method to send email
        public Boolean SendEmail()
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("rana_jabareen@hotmail.com","rjabreen@asaltech.com");
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

    }
}
        