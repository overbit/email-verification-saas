using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EmailVerificationService.Models;

namespace EmailVerificationService.Services
{
    public class ConsoleMessageService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            Debug.WriteLine($"Email: {message.Destination}, Body: {message.Body}");
            return Task.FromResult(0);
        }
    }
}
