using System;
using System.Net;
using System.Threading.Tasks;
using EmailVerificationService.ConfigBinders;
using EmailVerificationService.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailVerificationService.Services
{
    public class EmailService : IIdentityMessageService
    {
        private EmailConfigurationSettings EmailConfigurationSettings { get; }

        public EmailService(EmailConfigurationSettings emailConfigurationSettings)
        {
            EmailConfigurationSettings = emailConfigurationSettings;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            var apiKey = EmailConfigurationSettings.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(EmailConfigurationSettings.FromAccount, "Abcam validator");
            var to = new EmailAddress(message.Destination);
            var msg = MailHelper.CreateSingleEmail(from, to, message.Subject, message.Body, message.Body);
            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Error: {response.StatusCode} - Description: ${response.Body.ReadAsStringAsync().Result}");
            }
        }
    }
}