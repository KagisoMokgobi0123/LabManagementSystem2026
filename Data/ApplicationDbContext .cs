using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.LabManager.Models;
using NMB_HLabSys.Areas.Manager.Models;
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

      // modelBuilder.Entity<TestRequestsModel>()
      //.HasMany(t => t.TestTypes)
      //.WithMany(t => t.TestRequests) // Make sure this exists in TestTypeModel
      //.UsingEntity(j => j.ToTable("TestRequestTestTypes"));



            // Seed the Test Types (The Lab Catalog)
            modelBuilder.Entity<TestTypeModel>().HasData(
                new TestTypeModel
                {
                    TestTypeId = 1,
                    Name = "Full Blood Count (FBC)",
                    TurnAroundTime = new DateTime(2026, 01, 01, 04, 00, 0) // 4-hour target
                },
               
                new TestTypeModel
                {
                    TestTypeId = 2,
                    Name = "Liver Function Test (LFT)",
                    TurnAroundTime = new DateTime(2026, 01, 01, 08, 00, 0)
                }
            );
            // 1. Seed Test Requests
            modelBuilder.Entity<TestRequestsModel>().HasData(
                new TestRequestsModel
                {
                    TestRequestId = 1, RequestDate = new DateTime(2026, 04, 10, 10, 30, 0),  Status = "Submitted",Urgency = "Routine",ClinicalNotes = "Patient complaining of persistent fatigue. Check for anemia.",
                    PatientId = "3663096b-0ae2-4ab9-9321-d2a991afef86",
                    DoctorId = "24d75510-b1ad-4803-86e5-8e279b3ddb5a"    // Must match a seeded User Id
                },
                new TestRequestsModel
                {
                    TestRequestId = 2, RequestDate = new DateTime(2026, 04, 11, 09, 15, 0),Status = "Submitted",Urgency = "Stat",ClinicalNotes = "Suspected Malaria. High fever and chills.",
                    PatientId = "8ebb4a85-c399-43a9-a9ed-9d85211bc67d",
                    DoctorId = "ebfc3689-96da-433d-a042-58a90b0da38a"
                }
            );

            // 3. Seed Samples (Barcodes)
            modelBuilder.Entity<SampleModel>().HasData(
                new SampleModel { SampleId = 1, TestRequestId = 1, Barcode = "NMB-2026-0001", SampleTypeId = 1 },
                new SampleModel { SampleId = 2, TestRequestId = 2, Barcode = "NMB-2026-0002", SampleTypeId = 4 }
            );

            //// 4. Seed Test Assignments (Linking Requests to Test Types)
            //modelBuilder.Entity<AssignTestTypesModel>().HasData(
            //    new AssignTestTypesModel { AssignTestId = 1, TestRequestId = 1, TestTypeId = 1, Status = TestStatus.SampleReceived, ManagerId = "7cfd1034-f3a8-42ed-b13e-61f3b19ed8ec", TechnicianId = null, // Not yet claimed
            //        StartDateTime = null,
            //        CompletionDateTime = null
            //    },
            //    new AssignTestTypesModel
            //    {
            //        AssignTestId = 2,TestRequestId = 2, TestTypeId = 2, Status = TestStatus.InProgress, ManagerId = "7cfd1034-f3a8-42ed-b13e-61f3b19ed8ec",
            //        TechnicianId = "bdd52efb-571c-4ea7-9a18-9e9c66c92731", // The tech who assigned themselves
            //        StartDateTime = new DateTime(2026, 04, 18, 08, 00, 0),
            //        CompletionDateTime = null
            //    } // Still working on it }
            //);

       modelBuilder.Entity<SampleTypeModel>().HasData(
       new SampleTypeModel
       {
           SampleTypeId = 1,
           SampleName = "Blood"
       },
       new SampleTypeModel
       {
           SampleTypeId = 2,
           SampleName = "Urine"
       },
          new SampleTypeModel
          {
              SampleTypeId = 3,
              SampleName = "Saliva"
          },
        new SampleTypeModel
        {
            SampleTypeId = 4,
            SampleName = "Swab"
        }
     );
        }

        
        public DbSet<LoginUser> LoggedInUsers { get; set; }
        public DbSet<RegisterUser> RegisterdUsers { get; set; }

        //Lab Technician Tables
        public DbSet<TechnicianTestTypeModel> TechnicianTestTypes { get; set; }
        public DbSet<TestTypeModel> TestTypes { get; set; }
        public DbSet<Areas.Technician.Models.SampleModel> Samples { get; set; }
        public DbSet<SampleTypeModel> SampleTypes { get; set; }
        public DbSet<Areas.Doctor.Models.TestRequestsModel> TestRequests { get; set; }
        public DbSet<Areas.Technician.Models.TestRequestTestTypeModel> TestRequestTests { get; set; }
        public DbSet<ClinicalNotesModel> ClinicalNotes { get; set; }
        public DbSet<Areas.Technician.Models.ResultModel> Results { get; set; }

        public DbSet<Areas.Technician.Models.TestRequestTestTypeModel> TestRequestTestTypes { get; set; }
        public DbSet<Areas.Manager.Models.ConsumableModel> Consumables { get; set; }
        public DbSet<Areas.Technician.Models.TestTypeConsumableModel> TestConsumables { get; set; }







    }

}