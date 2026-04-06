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

        //Lab Technician Tables
        public DbSet<Areas.Technician.Models.TechnicianTestTypeModel> TechnicianTestTypes { get; set; }
        public DbSet<Areas.Technician.Models.TestTypeModel> TestTypes { get; set; }
        public DbSet<Areas.Technician.Models.SampleModel> Samples { get; set; }
        public DbSet<Areas.Technician.Models.TestRequestModel> TestRequests { get; set; }
        public DbSet<Areas.Technician.Models.TestRequestTestModel> TestRequestTests { get; set; }
        public DbSet<Areas.Technician.Models.ClinicalNotesModel> ClinicalNotes { get; set; }
        public DbSet<Areas.Technician.Models.ResultModel> Results { get; set; }
        public DbSet<Areas.Technician.Models.VerificationModel> Verifications { get; set; }
        public DbSet<Areas.Technician.Models.ConsumableModel> Consumables { get; set; }
        public DbSet<Areas.Technician.Models.TestTypeConsumableModel> TestConsumables { get; set; }







    }

}