using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace WebsiteForReservation.Classes
{
    public class Email
    {
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void sendEmail(string emailTo, string newPassword)
        {
            string emailFrom = "websiteemail331@gmail.com";
            MailMessage mail = new MailMessage(emailFrom,emailTo);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; 
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(emailFrom, "@7Q=cp?`ZJOVQB!2CoSf");
            smtp.Host = "smtp.gmail.com";
            mail.Subject = "New Password";
            mail.Body = "Your new password is: " + newPassword;
            smtp.Send(mail);
        }
    }
}