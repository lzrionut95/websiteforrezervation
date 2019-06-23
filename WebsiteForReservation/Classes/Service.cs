using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebsiteForReservation.Classes
{
    public class Service
    {
        private Entities db = new Entities();
        private  string stringKey = "U2A9/R*41FD412+4";
        public  string Encrypt(string password)
        {
            string finalValue = "";

                byte[] key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
                byte[] initializationVector = Encoding.UTF8.GetBytes(stringKey.Substring(stringKey.Length - 8, 8));
                byte[] word = Encoding.UTF8.GetBytes(password);

                DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, objDES.CreateEncryptor(key, initializationVector), CryptoStreamMode.Write);
                cryptoStream.Write(word, 0, word.Length);
                cryptoStream.FlushFinalBlock();

            finalValue = Convert.ToBase64String(memoryStream.ToArray());
            return finalValue;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                List<User> userList = db.Users.ToList();
                bool emailExist = false;
                foreach (User usr in userList)
                {
                    if (usr.Email.Replace(" ", "") == email.Replace(" ", ""))
                    {
                        emailExist = true;
                    }
                }
                return addr.Address == email && emailExist==true;
            }
            catch
            {
                return false;
            }
        }

        public void sendEmail(string emailTo, string message,string subject)
        {
            string emailFrom = "websiteemail331@gmail.com";
            MailMessage mail = new MailMessage(emailFrom, emailTo);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(emailFrom, "@7Q=cp?`ZJOVQB!2CoSf");
            smtp.Host = "smtp.gmail.com";
            mail.Subject = subject;
            mail.Body = message;
            smtp.Send(mail);
        }

    }
}