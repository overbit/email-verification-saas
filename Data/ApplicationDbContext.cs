using EmailVerificationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailVerificationService.Data
{
    public class ApplicationDbContext : IdentityUserContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Entries");
                entity.HasKey(e => e.Id);
                entity.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex").IsUnique();

                entity.Ignore(e => e.AccessFailedCount);
                entity.Ignore(e => e.ConcurrencyStamp);
                entity.Ignore(e => e.PhoneNumber);
                entity.Ignore(e => e.PhoneNumberConfirmed);
//                entity.Ignore(e => e.UserName);
                entity.Ignore(e => e.NormalizedUserName);
                entity.Ignore(e => e.LockoutEnabled);
                entity.Ignore(e => e.LockoutEnd);
                entity.Ignore(e => e.PasswordHash);
                entity.Ignore(e => e.TwoFactorEnabled);
            });

            builder.Ignore<IdentityUserClaim<string>>();
                
            builder.Ignore<IdentityUserToken<string>>();

            builder.Ignore<IdentityUserLogin<string>>();
        }
    }
}
