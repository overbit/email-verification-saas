using System;
using System.Threading.Tasks;
using EmailVerificationService.Models;

namespace EmailVerificationService.Services
{
    public interface IIdentityMessageService
    {
        Task SendAsync(IdentityMessage message);
    }
}
