using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Data
{
    public class ApplicationDbContext : IdentityDbContext<RegisterUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LoginUser> LoggedInUsers { get; set; }
        public DbSet<RegisterUser> RegisterdUsers { get; set; }

    }

}