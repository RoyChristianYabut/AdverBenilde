using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;


namespace AdverBenilde.App_Code
{
    public class Helper
    {

        /// <summary>
        /// Allows you to get the connection
        /// string from the web.config
        /// </summary>
        /// <returns>connection string</returns>
        public static string GetCon()
        {
            return ConfigurationManager.ConnectionStrings["MyCon2"].ConnectionString;
        }

        public static string Hash(string phrase)
        {
            SHA512Managed HashTool = new SHA512Managed();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(phrase));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }

        public static void SendEmail(string email, string subject, string message)
        {
            MailMessage emailMessage = new MailMessage();
            emailMessage.From = new MailAddress("benilde.web.development@gmail.com", "The Godfather");
            emailMessage.To.Add(new MailAddress(email));
            emailMessage.Subject = subject;
            emailMessage.Body = message;
            emailMessage.IsBodyHtml = true;
            emailMessage.Priority = MailPriority.Normal;
            SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587);
            MailClient.EnableSsl = true;
            MailClient.Credentials = new System.Net.NetworkCredential("benilde.web.development@gmail.com", "!thisisalongpassword1234567890");
            MailClient.Send(emailMessage);
        }
    }
}