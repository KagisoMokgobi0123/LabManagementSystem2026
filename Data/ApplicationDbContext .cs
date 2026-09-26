using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PasswordResetOtp>()
        .HasOne(x => x.User)
        .WithMany(x => x.PasswordResetOtps)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);
            // Additional entity configuration can be added here.
        }
    }
}
