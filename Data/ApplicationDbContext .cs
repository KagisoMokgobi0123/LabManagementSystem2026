using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Technician.Models.ViewModel;
using NMB_HLabSys.Models.Account;
using System.Reflection.Emit;
using StockTransactionModel = NMB_HLabSys.Areas.Manager.Models.StockTransactionModel;

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

            modelBuilder.Entity<TestRequestTestTypeModel>(entity =>
            {
                entity.HasKey(e => e.TestRequestTestTypeId);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Submitted");

                // Other configurations...
            });

            modelBuilder.Entity<ConsumableModel>().HasData(
     new ConsumableModel { ConsumableId = 1, Name = "Test Tubes (EDTA)", QuantityOnHand = 500, ReorderLevel = 100 },
     new ConsumableModel { ConsumableId = 2, Name = "Test Tubes (Sodium Citrate)", QuantityOnHand = 300, ReorderLevel = 75 },      // For PT/INR/aPTT
     new ConsumableModel { ConsumableId = 3, Name = "Syringes 5ml", QuantityOnHand = 300, ReorderLevel = 50 },
     new ConsumableModel { ConsumableId = 4, Name = "Blood Smear Slides", QuantityOnHand = 1000, ReorderLevel = 200 },              // For Blood Film
     new ConsumableModel { ConsumableId = 5, Name = "ESR Racks and Tubes", QuantityOnHand = 50, ReorderLevel = 15 },               // For ESR
     new ConsumableModel { ConsumableId = 6, Name = "Coagulation Test Cartridges", QuantityOnHand = 200, ReorderLevel = 50 },      // For PT/aPTT/INR/Fibrinogen
     new ConsumableModel { ConsumableId = 7, Name = "D-Dimer Test Kits", QuantityOnHand = 100, ReorderLevel = 30 },                 // For D-Dimer
     new ConsumableModel { ConsumableId = 8, Name = "Chemistry Analyzer Reagents", QuantityOnHand = 50, ReorderLevel = 15 },       // For Glucose/HbA1c/Lipid/LFT/RFT/CRP
     new ConsumableModel { ConsumableId = 9, Name = "Urine Dipsticks", QuantityOnHand = 500, ReorderLevel = 100 },                  // For Urinalysis
     new ConsumableModel { ConsumableId = 10, Name = "Microscope Slides (Urine)", QuantityOnHand = 500, ReorderLevel = 100 },      // For Urine Microscopy
     new ConsumableModel { ConsumableId = 11, Name = "Culture Media Plates", QuantityOnHand = 300, ReorderLevel = 80 },             // For Culture tests
     new ConsumableModel { ConsumableId = 12, Name = "Blood Culture Bottles", QuantityOnHand = 150, ReorderLevel = 40 },           // For Blood Culture
     new ConsumableModel { ConsumableId = 13, Name = "Alcohol Swabs", QuantityOnHand = 1000, ReorderLevel = 200 },
     new ConsumableModel { ConsumableId = 14, Name = "Gloves (Medium)", QuantityOnHand = 800, ReorderLevel = 150 },
     new ConsumableModel { ConsumableId = 15, Name = "Gloves (Large)", QuantityOnHand = 600, ReorderLevel = 120 },
     new ConsumableModel { ConsumableId = 16, Name = "Face Masks", QuantityOnHand = 500, ReorderLevel = 100 },
     new ConsumableModel { ConsumableId = 17, Name = "Lab Coats (Disposable)", QuantityOnHand = 100, ReorderLevel = 25 },
     new ConsumableModel { ConsumableId = 18, Name = "Centrifuge Tubes", QuantityOnHand = 1000, ReorderLevel = 200 },
     new ConsumableModel { ConsumableId = 19, Name = "Pipette Tips", QuantityOnHand = 2000, ReorderLevel = 400 },
     new ConsumableModel { ConsumableId = 20, Name = "Quality Control Materials", QuantityOnHand = 50, ReorderLevel = 15 }
 );
            modelBuilder.Entity<MeasurementUnitModel>().HasData(
                new MeasurementUnitModel { MeasurementUnitId = 1, Unit = "x10³/μL", Description = "Thousand cells per microliter" },
                new MeasurementUnitModel { MeasurementUnitId = 2, Unit = "%", Description = "Percentage" },
                new MeasurementUnitModel { MeasurementUnitId = 3, Unit = "Qualitative", Description = "Positive/Negative" },
                new MeasurementUnitModel { MeasurementUnitId = 4, Unit = "mm/hr", Description = "Millimeters per hour" },
                new MeasurementUnitModel { MeasurementUnitId = 5, Unit = "seconds", Description = "Seconds" },
                new MeasurementUnitModel { MeasurementUnitId = 6, Unit = "ratio", Description = "Ratio" },
                new MeasurementUnitModel { MeasurementUnitId = 7, Unit = "ng/mL", Description = "Nanograms per milliliter" },
                new MeasurementUnitModel { MeasurementUnitId = 8, Unit = "mg/dL", Description = "Milligrams per deciliter" },
                new MeasurementUnitModel { MeasurementUnitId = 9, Unit = "mmol/L", Description = "Millimoles per liter" },
                new MeasurementUnitModel { MeasurementUnitId = 10, Unit = "U/L", Description = "Units per liter" },
                new MeasurementUnitModel { MeasurementUnitId = 11, Unit = "CFU/mL", Description = "Colony forming units per milliliter" },
                new MeasurementUnitModel { MeasurementUnitId = 12, Unit = "g/dL", Description = "Grams per deciliter" },
                new MeasurementUnitModel { MeasurementUnitId = 13, Unit = "fL", Description = "Femtoliters" },
                new MeasurementUnitModel { MeasurementUnitId = 14, Unit = "pg", Description = "Picograms" },
                new MeasurementUnitModel { MeasurementUnitId = 15, Unit = "x10⁶/μL", Description = "Million cells per microliter" },
                new MeasurementUnitModel { MeasurementUnitId = 16, Unit = "μmol/L", Description = "Micromoles per liter" }
            );
            modelBuilder.Entity<TestRequestTestTypeModel>().HasData(
    // Test Request 1 (NMB-REQ-000001) - Routine blood tests
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 1,
        TestRequestId = 1,
        TestTypeId = 1, // FBC
        TechnicianId = null,
        Status = "Submitted",
        StartDateTime = DateTime.Now,
        CompletionDateTime = DateTime.Now,
        DueDate = DateTime.Now.AddHours(2)
    },
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 2,
        TestRequestId = 1,
        TestTypeId = 11, // Glucose
        TechnicianId = null,
        Status = "Submitted",
        StartDateTime = DateTime.Now,
        CompletionDateTime = DateTime.Now,
        DueDate = DateTime.Now.AddHours(1)
    },

    // Test Request 2 (NMB-REQ-000002) - Urgent tests
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 3,
        TestRequestId = 2,
        TestTypeId = 1, // FBC
        TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
        Status = "InProgress",
        StartDateTime = DateTime.Now.AddHours(-2),
        CompletionDateTime = DateTime.Now,
        DueDate = DateTime.Now.AddHours(2)
    },
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 4,
        TestRequestId = 2,
        TestTypeId = 6, // PT
        TechnicianId = null,
        Status = "Submitted",
        StartDateTime = DateTime.Now,
        CompletionDateTime = DateTime.Now,
        DueDate = DateTime.Now.AddHours(2)
    },

    // Test Request 3 (NMB-REQ-000003) - STAT emergency tests
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 5,
        TestRequestId = 3,
        TestTypeId = 1, // FBC
        TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
        Status = "Completed",
        StartDateTime = DateTime.Now.AddHours(-3),
        CompletionDateTime = DateTime.Now.AddHours(-1),
        DueDate = DateTime.Now.AddHours(-1),
        ResultValue = "WBC: 11.2, RBC: 4.8, HGB: 14.2, PLT: 250",
        Notes = "Results show elevated white blood cells"
    },
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 6,
        TestRequestId = 3,
        TestTypeId = 11, // Glucose
        TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
        Status = "Verified",
        StartDateTime = DateTime.Now.AddHours(-3),
        CompletionDateTime = DateTime.Now.AddHours(-2),
        DueDate = DateTime.Now.AddHours(-2),
        ResultValue = "5.6 mmol/L",
        Notes = "Normal range",
        VerifiedByTechnicianId = "af381d22-8f2c-4401-ae8b-b31bb60e1c39",
        VerificationNote = "Verified - within normal range",
        VerificationDateTime = DateTime.Now.AddHours(-1.5)
    },
    new TestRequestTestTypeModel
    {
        TestRequestTestTypeId = 7,
        TestRequestId = 3,
        TestTypeId = 6, // PT
        TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
        Status = "ToBeReviewed",
        StartDateTime = DateTime.Now.AddHours(-2),
        CompletionDateTime = DateTime.Now.AddHours(-0.5),
        DueDate = DateTime.Now,
        ResultValue = "INR: 3.5",
        Notes = "Abnormal high INR - needs review",
        VerifiedByTechnicianId = "af381d22-8f2c-4401-ae8b-b31bb60e1c39",
        VerificationNote = "Result appears abnormal. Please review clinical notes.",
        VerificationDateTime = DateTime.Now.AddMinutes(-30)
    }
);
          
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
    // ==================== HEMATOLOGY TESTS (CategoryId = 1) ====================
    new TestTypeModel
    {
        TestTypeId = 1,
        Name = "Full Blood Count (FBC)",
        TestCategoryId = 1,
        SampleTypeId = 1, // Whole Blood
        ConsumableId = 1,
        MeasurementUnit = 1, // x10³/μL
        NormalMin = 4.5m,
        NormalMax = 11.0m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 2,
        Name = "White Blood Cell Differential",
        TestCategoryId = 1,
        SampleTypeId = 1,
        ConsumableId = 1,
        MeasurementUnit = 2, // Percentage
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 3
    },
    new TestTypeModel
    {
        TestTypeId = 3,
        Name = "Peripheral Blood Film",
        TestCategoryId = 1,
        SampleTypeId = 1,
        ConsumableId = 4,
        MeasurementUnit = 3, // Qualitative
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 4
    },
    new TestTypeModel
    {
        TestTypeId = 4,
        Name = "Reticulocyte Count",
        TestCategoryId = 1,
        SampleTypeId = 1,
        ConsumableId = 1,
        MeasurementUnit = 2, // Percentage
        NormalMin = 0.5m,
        NormalMax = 2.5m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 5,
        Name = "Erythrocyte Sedimentation Rate (ESR)",
        TestCategoryId = 1,
        SampleTypeId = 1,
        ConsumableId = 5,
        MeasurementUnit = 4, // mm/hr
        NormalMin = 0m,
        NormalMax = 20m,
        TurnAroundTimeHours = 1
    },

    // ==================== COAGULATION STUDIES (CategoryId = 2) ====================
    new TestTypeModel
    {
        TestTypeId = 6,
        Name = "Prothrombin Time (PT)",
        TestCategoryId = 2,
        SampleTypeId = 4, // Plasma
        ConsumableId = 6,
        MeasurementUnit = 5, // seconds
        NormalMin = 11.0m,
        NormalMax = 13.5m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 7,
        Name = "Activated Partial Thromboplastin Time (aPTT)",
        TestCategoryId = 2,
        SampleTypeId = 4,
        ConsumableId = 6,
        MeasurementUnit = 5, // seconds
        NormalMin = 25.0m,
        NormalMax = 35.0m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 8,
        Name = "International Normalized Ratio (INR)",
        TestCategoryId = 2,
        SampleTypeId = 4,
        ConsumableId = 6,
        MeasurementUnit = 6, // ratio
        NormalMin = 0.8m,
        NormalMax = 1.2m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 9,
        Name = "D-Dimer",
        TestCategoryId = 2,
        SampleTypeId = 4,
        ConsumableId = 7,
        MeasurementUnit = 7, // ng/mL
        NormalMin = 0m,
        NormalMax = 500m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 10,
        Name = "Fibrinogen Level",
        TestCategoryId = 2,
        SampleTypeId = 4,
        ConsumableId = 6,
        MeasurementUnit = 8, // mg/dL
        NormalMin = 200m,
        NormalMax = 400m,
        TurnAroundTimeHours = 3
    },

    // ==================== CLINICAL CHEMISTRY (CategoryId = 3) ====================
    new TestTypeModel
    {
        TestTypeId = 11,
        Name = "Glucose (Fasting)",
        TestCategoryId = 3,
        SampleTypeId = 4, // Plasma
        ConsumableId = 8,
        MeasurementUnit = 9, // mmol/L
        NormalMin = 3.9m,
        NormalMax = 5.5m,
        TurnAroundTimeHours = 1
    },
    new TestTypeModel
    {
        TestTypeId = 12,
        Name = "Hemoglobin A1C (HbA1c)",
        TestCategoryId = 3,
        SampleTypeId = 1, // Whole Blood
        ConsumableId = 1,
        MeasurementUnit = 2, // Percentage
        NormalMin = 4.0m,
        NormalMax = 5.6m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 13,
        Name = "Lipid Profile",
        TestCategoryId = 3,
        SampleTypeId = 4, // Plasma
        ConsumableId = 8,
        MeasurementUnit = 9, // mmol/L
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 4,
      
    },
    new TestTypeModel
    {
        TestTypeId = 14,
        Name = "Liver Function Test (LFT)",
        TestCategoryId = 3,
        SampleTypeId = 4, // Plasma
        ConsumableId = 8,
        MeasurementUnit = 10, // U/L
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 4,
        
    },
    new TestTypeModel
    {
        TestTypeId = 15,
        Name = "Renal Function Test (RFT)",
        TestCategoryId = 3,
        SampleTypeId = 4, // Plasma
        ConsumableId = 8,
        MeasurementUnit = 9, // mmol/L
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 3,
       
    },
    new TestTypeModel
    {
        TestTypeId = 16,
        Name = "C-Reactive Protein (CRP)",
        TestCategoryId = 3,
        SampleTypeId = 4, // Plasma
        ConsumableId = 8,
        MeasurementUnit = 7, // mg/L
        NormalMin = 0m,
        NormalMax = 5m,
        TurnAroundTimeHours = 2
    },

    // ==================== URINALYSIS (CategoryId = 4) ====================
    new TestTypeModel
    {
        TestTypeId = 17,
        Name = "Urinalysis (Dipstick)",
        TestCategoryId = 4,
        SampleTypeId = 2, // Urine
        ConsumableId = 9,
        MeasurementUnit = 3, // Qualitative
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 1,
    
    },
    new TestTypeModel
    {
        TestTypeId = 18,
        Name = "Urine Microscopy",
        TestCategoryId = 4,
        SampleTypeId = 2, // Urine
        ConsumableId = 10,
        MeasurementUnit = 1, // x10³/μL
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 2,
       
    },
    new TestTypeModel
    {
        TestTypeId = 19,
        Name = "Urine Culture and Sensitivity",
        TestCategoryId = 4,
        SampleTypeId = 2, // Urine
        ConsumableId = 11,
        MeasurementUnit = 11, // CFU/mL
        NormalMin = 0m,
        NormalMax = 10000m,
        TurnAroundTimeHours = 48,
       
    },

    // ==================== MICROBIOLOGY (CategoryId = 5) ====================
    new TestTypeModel
    {
        TestTypeId = 20,
        Name = "Blood Culture",
        TestCategoryId = 5,
        SampleTypeId = 1, // Whole Blood
        ConsumableId = 12,
        MeasurementUnit = 3, // Qualitative
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 72,
       
    },
    new TestTypeModel
    {
        TestTypeId = 21,
        Name = "Sputum Culture",
        TestCategoryId = 5,
        SampleTypeId = 9, // Sputum
        ConsumableId = 11,
        MeasurementUnit = 3, // Qualitative
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 48,
       
    },
    new TestTypeModel
    {
        TestTypeId = 22,
        Name = "Wound Swab Culture",
        TestCategoryId = 5,
        SampleTypeId = 10, // Swab
        ConsumableId = 11,
        MeasurementUnit = 3, // Qualitative
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 48,
        
    }
);

            modelBuilder.Entity<SampleTypeModel>().HasData(
    new SampleTypeModel { SampleTypeId = 1, Name = "Blood", IsActive = true },
    new SampleTypeModel { SampleTypeId = 2, Name = "Urine", IsActive = true },
    new SampleTypeModel { SampleTypeId = 3, Name = "Saliva" , IsActive = true },
    new SampleTypeModel { SampleTypeId = 4, Name = "Plasma" , IsActive = true },
    new SampleTypeModel { SampleTypeId = 5, Name = "Serum" , IsActive = true },
    new SampleTypeModel { SampleTypeId = 6, Name = "Bone Marrow" , IsActive = true },
    new SampleTypeModel { SampleTypeId = 7, Name = "CSF" , IsActive = true },
    new SampleTypeModel { SampleTypeId = 8, Name = "Stool" , IsActive = true },
    new SampleTypeModel { SampleTypeId = 9, Name = "Sputum" , IsActive = true },      // Added for TestTypeId 21
    new SampleTypeModel { SampleTypeId = 10, Name = "Swab" , IsActive = true }        // Added for TestTypeId 22
);

            // Seed Samples
            modelBuilder.Entity<SampleModel>().HasData(
            new SampleModel
            {
                SampleId = 1,
                Barcode = "SMP-2024-000001",
                SampleTypeId = 1,
                TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
                DateReceived = new DateTime(2024, 1, 15, 10, 30, 0),
                Status = "Received",
                TestRequestId = 1,
                IsReceived = true
            },
                new SampleModel
                {
                    SampleId = 2,
                    Barcode = "SMP-2024-000002",
                    SampleTypeId = 2,
                    TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
                    DateReceived = new DateTime(2024, 1, 16, 11, 45, 0),
                    Status = "Received",
                    TestRequestId = 1,
                    IsReceived = true
                },
                new SampleModel
                {
                    SampleId = 3,
                    Barcode = "SMP-2024-000003",
                    SampleTypeId = 3,
                    TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
                    DateReceived = new DateTime(2024, 1, 17, 09, 15, 0),
                    Status = "Received",
                    TestRequestId = 2,
                    IsReceived = true
                },
                new SampleModel
                {
                    SampleId = 4,
                    Barcode = "SMP-2024-000004",
                    SampleTypeId = 1,
                    TechnicianId = null,
                    DateReceived = new DateTime(2024, 1, 18, 14, 20, 0),
                    Status = "Pending",
                    TestRequestId = 2,
                    IsReceived = false
                },
                new SampleModel
                {
                    SampleId = 5,
                    Barcode = "SMP-2024-000005",
                    SampleTypeId = 4,
                    TechnicianId = "2ec716df-770e-465e-a7b7-8d3926b560a2",
                    DateReceived = new DateTime(2024, 1, 19, 08, 00, 0),
                    Status = "Received",
                    TestRequestId = 3,
                    IsReceived = true
                },
                new SampleModel
                {
                    SampleId = 6,
                    Barcode = "SMP-2024-000006",
                    SampleTypeId = 5,
                    TechnicianId = null,
                    DateReceived = new DateTime(2024, 1, 20, 13, 30, 0),
                    Status = "Pending",
                    TestRequestId = 3,
                    IsReceived = false
                },
             
                new SampleModel
                {
                    SampleId = 9,
                    Barcode = "SMP-2024-000009",
                    SampleTypeId = 3,
                    TechnicianId = null,
                    DateReceived = new DateTime(2024, 1, 23, 11, 20, 0),
                    Status = "Pending",
                    TestRequestId = 3,
                    IsReceived = false
                }
             
            );
            modelBuilder.Entity<TestTypeConsumableModel>().HasData(
    // Hematology Tests
    new TestTypeConsumableModel { TestTypeConsumableId = 1, TestTypeId = 1, ConsumableId = 1, QuantityRequired = 1 },  // FBC - EDTA Tube
    new TestTypeConsumableModel { TestTypeConsumableId = 2, TestTypeId = 2, ConsumableId = 1, QuantityRequired = 1 },  // WBC Diff - EDTA Tube
    new TestTypeConsumableModel { TestTypeConsumableId = 3, TestTypeId = 3, ConsumableId = 4, QuantityRequired = 2 },  // Blood Film - Slides
    new TestTypeConsumableModel { TestTypeConsumableId = 4, TestTypeId = 4, ConsumableId = 1, QuantityRequired = 1 },  // Reticulocyte - EDTA Tube
    new TestTypeConsumableModel { TestTypeConsumableId = 5, TestTypeId = 5, ConsumableId = 5, QuantityRequired = 1 },  // ESR - ESR Tube

    // Coagulation Tests
    new TestTypeConsumableModel { TestTypeConsumableId = 6, TestTypeId = 6, ConsumableId = 2, QuantityRequired = 1 },  // PT - Citrate Tube
    new TestTypeConsumableModel { TestTypeConsumableId = 7, TestTypeId = 7, ConsumableId = 2, QuantityRequired = 1 },  // aPTT - Citrate Tube
    new TestTypeConsumableModel { TestTypeConsumableId = 8, TestTypeId = 8, ConsumableId = 2, QuantityRequired = 1 },  // INR - Citrate Tube
    new TestTypeConsumableModel { TestTypeConsumableId = 9, TestTypeId = 9, ConsumableId = 7, QuantityRequired = 1 },  // D-Dimer - Test Kit
    new TestTypeConsumableModel { TestTypeConsumableId = 10, TestTypeId = 10, ConsumableId = 6, QuantityRequired = 1 }, // Fibrinogen - Cartridge

    // Chemistry Tests
    new TestTypeConsumableModel { TestTypeConsumableId = 11, TestTypeId = 11, ConsumableId = 8, QuantityRequired = 1 }, // Glucose - Reagents
    new TestTypeConsumableModel { TestTypeConsumableId = 12, TestTypeId = 12, ConsumableId = 8, QuantityRequired = 1 }, // HbA1c - Reagents
    new TestTypeConsumableModel { TestTypeConsumableId = 13, TestTypeId = 13, ConsumableId = 8, QuantityRequired = 2 }, // Lipid Profile - Reagents
    new TestTypeConsumableModel { TestTypeConsumableId = 14, TestTypeId = 14, ConsumableId = 8, QuantityRequired = 2 }, // LFT - Reagents
    new TestTypeConsumableModel { TestTypeConsumableId = 15, TestTypeId = 15, ConsumableId = 8, QuantityRequired = 1 }, // RFT - Reagents
    new TestTypeConsumableModel { TestTypeConsumableId = 16, TestTypeId = 16, ConsumableId = 8, QuantityRequired = 1 }, // CRP - Reagents

    // Urinalysis Tests
    new TestTypeConsumableModel { TestTypeConsumableId = 17, TestTypeId = 17, ConsumableId = 9, QuantityRequired = 1 }, // Urinalysis - Dipstick
    new TestTypeConsumableModel { TestTypeConsumableId = 18, TestTypeId = 18, ConsumableId = 10, QuantityRequired = 1 }, // Urine Microscopy - Slides
    new TestTypeConsumableModel { TestTypeConsumableId = 19, TestTypeId = 19, ConsumableId = 11, QuantityRequired = 1 }, // Urine Culture - Media

    // Microbiology Tests
    new TestTypeConsumableModel { TestTypeConsumableId = 20, TestTypeId = 20, ConsumableId = 12, QuantityRequired = 2 }, // Blood Culture - Bottles
    new TestTypeConsumableModel { TestTypeConsumableId = 21, TestTypeId = 21, ConsumableId = 11, QuantityRequired = 1 }, // Sputum Culture - Media
    new TestTypeConsumableModel { TestTypeConsumableId = 22, TestTypeId = 22, ConsumableId = 11, QuantityRequired = 1 }  // Wound Swab - Media
);




            modelBuilder.Entity<TestRequestsModel>().HasData(
           // Test Request 1 - Routine
           new TestRequestsModel
           {
               TestRequestId = 1,
               RequestDate = new DateTime(2026, 5, 1),
               Status = "Submitted",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Routine,
               ClinicalNotes = "Routine blood test - Annual checkup",
               RequestNumber = "NMB-REQ-000001"
           },
           // Test Request 2 - Urgent
           new TestRequestsModel
           {
               TestRequestId = 2,
               RequestDate = new DateTime(2026, 5, 2),
               Status = "Samples Received",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Urgent,
               ClinicalNotes = "Patient presenting with fever and chills - suspected infection",
               RequestNumber = "NMB-REQ-000002"
           },
           // Test Request 3 - STAT Emergency
           new TestRequestsModel
           {
               TestRequestId = 3,
               RequestDate = new DateTime(2026, 5, 3),
               Status = "InProgress",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Stat,
               ClinicalNotes = "Emergency - Patient collapsed, suspected DVT",
               RequestNumber = "NMB-REQ-000003"
           },
           // Test Request 4 - Routine Diabetes Monitoring
           new TestRequestsModel
           {
               TestRequestId = 4,
               RequestDate = new DateTime(2026, 5, 4),
               Status = "Completed",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Routine,
               ClinicalNotes = "Diabetes monitoring - HbA1c and glucose levels",
               RequestNumber = "NMB-REQ-000004"
           },
           // Test Request 5 - Urgent Cardiac
           new TestRequestsModel
           {
               TestRequestId = 5,
               RequestDate = new DateTime(2026, 5, 5),
               Status = "Submitted",
               PatientId = "7b0c0099-788d-4e4a-94f5-0f07c0b2013c",
               DoctorId = "53d244f0-97dd-4a05-97ea-625f6237a3d7",
               Urgency = Urgency.Urgent,
               ClinicalNotes = "Chest pain, suspected cardiac issue - Lipid Profile and Cardiac enzymes",
               RequestNumber = "NMB-REQ-000005"
           },
           // Test Request 6 - Routine Liver Function
           new TestRequestsModel
           {
               TestRequestId = 6,
               RequestDate = new DateTime(2026, 5, 6),
               Status = "Samples Received",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Routine,
               ClinicalNotes = "Routine liver function test for patient on medication",
               RequestNumber = "NMB-REQ-000006"
           },
           // Test Request 7 - STAT Coagulation
           new TestRequestsModel
           {
               TestRequestId = 7,
               RequestDate = new DateTime(2026, 5, 7),
               Status = "Submitted",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Stat,
               ClinicalNotes = "Pre-surgery coagulation screen - STAT request",
               RequestNumber = "NMB-REQ-000007"
           },
           // Test Request 8 - Routine Urinalysis
           new TestRequestsModel
           {
               TestRequestId = 8,
               RequestDate = new DateTime(2026, 5, 8),
               Status = "Completed",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Routine,
               ClinicalNotes = "Routine urinalysis - suspected UTI",
               RequestNumber = "NMB-REQ-000008"
           },
           // Test Request 9 - Urgent Microbiology
           new TestRequestsModel
           {
               TestRequestId = 9,
               RequestDate = new DateTime(2026, 5, 9),
               Status = "InProgress",
               PatientId = "7b0c0099-788d-4e4a-94f5-0f07c0b2013c",
               DoctorId = "e6eec045-31c8-42c6-b67d-11bb28d980be",
               Urgency = Urgency.Urgent,
               ClinicalNotes = "Suspected blood stream infection - Blood culture",
               RequestNumber = "NMB-REQ-000009"
           },
           // Test Request 10 - Routine Annual Checkup
           new TestRequestsModel
           {
               TestRequestId = 10,
               RequestDate = new DateTime(2026, 5, 10),
               Status = "Submitted",
               PatientId = "fd5dcbfa-644b-438f-9bf4-a7158300009f",
               DoctorId = "53d244f0-97dd-4a05-97ea-625f6237a3d7",
               Urgency = Urgency.Routine,
               ClinicalNotes = "Annual physical examination - Full blood count and chemistry panel",
               RequestNumber = "NMB-REQ-000010"
           }
       );



        }

        
        public DbSet<LoginUser> LoggedInUsers { get; set; }
        public DbSet<RegisterUser> RegisterdUsers { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<StockTransactionModel> StockTransactions { get; set; }
        public DbSet<TechnicianTestTypeModel> TechnicianTestTypes { get; set; }
        public DbSet<TestTypeModel> TestTypes { get; set; }
        public DbSet<SampleModel> Samples { get; set; }
        public DbSet<SampleTypeModel> SampleTypes { get; set; }
        public DbSet<TestRequestsModel> TestRequests { get; set; }
        public DbSet<TestRequestTestTypeModel> TestRequestTests { get; set; }
        public DbSet<ClinicalNotesModel> ClinicalNotes { get; set; }
        public DbSet<ResultModel> Results { get; set; }
        public DbSet<TestRequestTestTypeHistoryModel> TestRequestTestTypeHistories { get; set; }
        public DbSet<ConsumableModel> Consumables { get; set; }
        public DbSet<TestTypeConsumableModel> TestTypeConsumables { get; set; }
        public DbSet<MeasurementUnitModel> MeasurementUnits { get; set; }






    }

}