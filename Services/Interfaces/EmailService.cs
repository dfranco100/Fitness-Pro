using FitnessPro.ViewModel;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

using MailKit.Net.Smtp;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace FitnessPro.Services.Interfaces
{
    public class EmailService : IFitnessEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        
        
        public Task sendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }


        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email;

            MimeMessage newEmail = new();

            //newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.From.Add(MailboxAddress.Parse(_mailSettings.Email)); // Set the sender email
            newEmail.To.Add(MailboxAddress.Parse(emailTo)); // Set the recipient email

           /* foreach (var emailAddress in emailSender.Split(";"))
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            } */
            
            newEmail.Subject= subject;


            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;

            newEmail.Body=emailBody.ToMessageBody();

            using SmtpClient smtpClient = new SmtpClient();

            try
            {
                var host = _mailSettings.Host;
                var port = _mailSettings.Port;
                var password = _mailSettings.Password;


                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                //await smtpClient.AuthenticateAsync(emailSender, password);
                await smtpClient.AuthenticateAsync(_mailSettings.Email, password); // Use sender's email for authentication

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
               
            }
            catch { }
        }
    }
}
