using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using System.Reflection.Emit;

namespace NMB_HLabSys.Data
{
    public class ApplicationDbContext : IdentityDbContext<RegisterUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call the base method first when using Identity
            base.OnModelCreating(modelBuilder);
            // 1. Your unique index for barcodes
            modelBuilder.Entity<SampleModel>()
                .HasIndex(s => s.Barcode)
                .IsUnique();
            //
 
        }
        public DbSet<AssignTestTypes> AssignTestTypes { get; set; }
        public DbSet<LoginUser> LoggedInUsers { get; set; }
        public DbSet<RegisterUser> RegisterdUsers { get; set; }

        //Lab Technician Tables
        public DbSet<Areas.Technician.Models.TechnicianTestTypeModel> TechnicianTestTypes { get; set; }
        public DbSet<Areas.Technician.Models.TestTypeModel> TestTypes { get; set; }
        public DbSet<Areas.Technician.Models.SampleModel> Samples { get; set; }
        public DbSet<Areas.Doctor.Models.TestRequestsModel> TestRequests { get; set; }
        public DbSet<Areas.Technician.Models.TestRequestTestModel> TestRequestTests { get; set; }
        public DbSet<Areas.Technician.Models.ClinicalNotesModel> ClinicalNotes { get; set; }
        public DbSet<Areas.Technician.Models.ResultModel> Results { get; set; }
        public DbSet<Areas.Technician.Models.ConsumableModel> Consumables { get; set; }
        public DbSet<Areas.Technician.Models.TestTypeConsumableModel> TestConsumables { get; set; }







    }

}