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

            modelBuilder.Entity<TechnicianTestTypeModel>()
      .HasOne(t => t.Technician)
      .WithMany()
      .HasForeignKey(t => t.TechnicianId)
      .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TechnicianTestTypeModel>()
                .HasOne(t => t.Manager)
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1. Your index for barcodes
            modelBuilder.Entity<SampleModel>()
                .HasIndex(s => s.Barcode)
                .IsUnique();

            modelBuilder.Entity<TestRequestsModel>()
            .Property(t => t.Urgency)
            .HasConversion<string>();

        modelBuilder.Entity<TestCategoryModel>().HasData(
        new TestCategoryModel
        {
            TestCategoryId = 1,
            Description = "Hematology"
        },
        new TestCategoryModel
        {
            TestCategoryId = 2,
            Description = "Biochemistry"
        },
        new TestCategoryModel
        {
            TestCategoryId = 3,
            Description = "Microbiology"
        },
        new TestCategoryModel
        {
            TestCategoryId = 4,
            Description = "Immunology"
        },
        new TestCategoryModel
        {
            TestCategoryId = 5,
            Description = "Blood Banking"
        }
    );

            modelBuilder.Entity<TestTypeModel>().HasData(
            new TestTypeModel
            {
                TestTypeId = 1,
                Name = "Full Blood Count",
                TestCategoryId = 1,
                SampleTypeId = 1,
                ConsumableId = 1,
                MeasurementUnit = 1,
                NormalMin = 4.5m,
                NormalMax = 11.0m,
                TurnAroundTimeHours = 2
            },
            new TestTypeModel
            {
                TestTypeId = 2,
                Name = "Urine Culture",
                TestCategoryId = 2,
                SampleTypeId = 2,
                ConsumableId = 2,
                MeasurementUnit = 1,
                NormalMin = 0,
                NormalMax = 5,
                TurnAroundTimeHours = 4
            },
            new TestTypeModel
            {
                TestTypeId = 3,
                Name = "Glucose Test",
                TestCategoryId = 3,
                SampleTypeId = 1,
                ConsumableId = 3,
                MeasurementUnit = 1,
                NormalMin = 3.9m,
                NormalMax = 5.5m,
                TurnAroundTimeHours = 1
            }
        );

            modelBuilder.Entity<SampleTypeModel>().HasData(
        new SampleTypeModel
        {
            SampleTypeId = 1,
            Name = "Blood"
        },
        new SampleTypeModel
        {
            SampleTypeId = 2,
            Name = "Urine"
        },
        new SampleTypeModel
        {
            SampleTypeId = 3,
            Name = "Saliva"
        },
        new SampleTypeModel
        {
            SampleTypeId = 4,
            Name = "Plasma"
        },
        new SampleTypeModel
        {
            SampleTypeId = 5,
            Name = "Serum"
        }
        );

            modelBuilder.Entity<ConsumableModel>().HasData(
                new ConsumableModel
                {
                    ConsumableId = 1,
                    Name = "Test Tubes (EDTA)",
                    QuantityOnHand = 500,
                    ReorderLevel = 100
                },
                new ConsumableModel
                {
                    ConsumableId = 2,
                    Name = "Syringes 5ml",
                    QuantityOnHand = 300,
                    ReorderLevel = 50
                },
                new ConsumableModel
                {
                    ConsumableId = 3,
                    Name = "Alcohol Swabs",
                    QuantityOnHand = 1000,
                    ReorderLevel = 200
                },
                new ConsumableModel
                {
                    ConsumableId = 4,
                    Name = "Gloves (Medium)",
                    QuantityOnHand = 800,
                    ReorderLevel = 150
                }
            );




    modelBuilder.Entity<TechnicianTestTypeModel>().HasData(
    new TechnicianTestTypeModel
    {
        TechnicianTestTypeId = 1,
        TestTypeId = 1,
        TechnicianId = "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a",
        ManagerId = "9e586fad-975e-44b3-9b44-16bdf420354e",
        Status = "Submitted",
        AssignmentDate = new DateTime(2026, 01, 01)
    },
    new TechnicianTestTypeModel
    {
        TechnicianTestTypeId = 2,
        TestTypeId = 2,
        TechnicianId = "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a",
        ManagerId = "9e586fad-975e-44b3-9b44-16bdf420354e",
        Status = "Submitted",
        AssignmentDate = new DateTime(2026, 01, 02)
    },
    new TechnicianTestTypeModel
    {
        TechnicianTestTypeId = 3,
        TestTypeId = 3,
        TechnicianId = "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a",
        ManagerId = "1ae2c09e-4743-4950-a377-fc9484ea4794",
        Status = "Submitted",
        AssignmentDate = new DateTime(2026, 01, 03)
    }
);


            modelBuilder.Entity<TestRequestsModel>().HasData(
        new TestRequestsModel
        {
            TestRequestId = 1,
            RequestDate = new DateTime(2026, 5, 1),
            Status = "Pending",
            PatientId = "7f8a9985-54df-4e2c-8433-14089d075dbf",
            DoctorId = "cad131b2-791f-4d47-ae17-06f5e3864ef1",
            Urgency = Urgency.Routine,
            ClinicalNotes = "Routine blood test",
            RequestNumber = "NMB-REQ-000001"
        },
        new TestRequestsModel
        {
            TestRequestId = 2,
            RequestDate = new DateTime(2026, 5, 2),
            Status = "Samples Received",
            PatientId = "e6fa143c-9f79-4af2-8b53-97fb8ee94947",
            DoctorId = "cad131b2-791f-4d47-ae17-06f5e3864ef1",
            Urgency = Urgency.Urgent,
            ClinicalNotes = "Suspected infection",
            RequestNumber = "NMB-REQ-000002"
        },
        new TestRequestsModel
        {
            TestRequestId = 3,
            RequestDate = new DateTime(2026, 5, 3),
            Status = "Pending",
            PatientId = "e6fa143c-9f79-4af2-8b53-97fb8ee94947",
            DoctorId = "b5c8ea7a-50fa-4df6-8e93-3b722c5377a2",
            Urgency = Urgency.Stat,
            ClinicalNotes = "Emergency test required",
            RequestNumber = "NMB-REQ-000003"
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
        public DbSet<Areas.Technician.Models.TestRequestTestTypeHistoryModel> TestRequestTestTypeHistories { get; set; }
        public DbSet<Areas.Technician.Models.TestRequestTestTypeModel> TestRequestTestTypes { get; set; }
        public DbSet<Areas.Manager.Models.ConsumableModel> Consumables { get; set; }
        public DbSet<Areas.Technician.Models.TestTypeConsumableModel> TestTypeConsumables { get; set; }







    }

}