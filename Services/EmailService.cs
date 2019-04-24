using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailVerificationService.ConfigBinders;
using EmailVerificationService.Models;
using Microsoft.Extensions.Configuration;

namespace EmailVerificationService.Services
{
    public class EmailService : IIdentityMessageService
    {
        private readonly IConfiguration config;
        private EmailSettings emailSettings;

        public EmailService(IConfiguration configuration)
        {
            config = configuration;
            config.GetSection("Mail").Bind(emailSettings);
            if (string.IsNullOrEmpty(emailSettings.Username) || string.IsNullOrEmpty(emailSettings.Password))
            {
                throw new Exception("You must set the Mail.Username and Mail.Password in the appsettings.json");
            }
        }

        public async Task SendAsync(IdentityMessage message)
        {
            var client = new SmtpClient(emailSettings.SmtpClient)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password)
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSettings.FromAccount);
            mailMessage.To.Add(message.Destination);
            mailMessage.Body = message.Body;
            mailMessage.Subject = message.Subject;

            client.SendAsync(mailMessage, message.Destination);
        }

        private void SmtpClientSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var smtpClient = (SmtpClient)sender;
            var userAsyncState = (string)e.UserState;
            smtpClient.SendCompleted -= SmtpClientSendCompleted;

//            if (e.Error != null)
//            {
//                Log.error(
//                    e.Error,
//                    string.Format("Message sending for \"{0}\" failed.", userAsyncState)
//                );
//            }

            // Cleaning up resources
        }
    }
}