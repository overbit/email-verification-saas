using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EmailVerificationService.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public override string Email { get; set; }
    }
}