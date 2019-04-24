using EmailVerificationService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EmailVerificationService.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string email, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(VerificationController.ConfirmEmail),
                controller: "Verification",
                values: new { email, code },
                protocol: scheme);
        }
    }
}