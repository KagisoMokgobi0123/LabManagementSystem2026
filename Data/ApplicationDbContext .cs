using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Identity;
using System.Reflection.Emit;

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

        public DbSet<TestRequestsModel> TestRequests { get; set; }

        public DbSet<SampleModel> Samples { get; set; }

        public DbSet<SampleTypeModel> SampleTypes { get; set; }

        public DbSet<TestCategoryModel> TestCategories { get; set; }

        public DbSet<TestTypeModel> TestTypes { get; set; }

        public DbSet<ConsumableModel> Consumables { get; set; }
        
        public DbSet<TestRequestTestTypeModel> TestRequestTestTypes{ get; set; }

        public DbSet<MeasurementUnitModel> MeasurementUnits { get; set; }

        public DbSet<TechnicianTestTypeModel> TechnicianTestTypes { get; set; }

        public DbSet<TestTypeConsumableModel> TestTypeConsumables { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PasswordResetOtp>()
        .HasOne(x => x.User)
        .WithMany(x => x.PasswordResetOtps)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);
            // Additional entity configuration can be added here.
            // ============================================================
            // TEST REQUEST SEED DATA
            // ============================================================

            builder.Entity<TestRequestsModel>().HasData(

                // ========================================================
                // REQUEST 1 - ROUTINE
                // Waiting for samples to be received
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 1,
                    RequestNumber = "NMB-REQ-000001",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 20, 9, 0, 0),

                    Urgency = Urgency.Routine,

                    ClinicalNotes =
                        "Routine blood test - annual checkup.",

                    Status = TestRequestStatus.Submitted.ToString()
                },


                // ========================================================
                // REQUEST 2 - URGENT
                // Samples already received
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 2,
                    RequestNumber = "NMB-REQ-000002",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 21, 10, 30, 0),

                    Urgency = Urgency.Urgent,

                    ClinicalNotes =
                        "Patient presenting with fever and chills. Suspected infection.",

                    Status = TestRequestStatus.SamplesReceived.ToString()
                },


                // ========================================================
                // REQUEST 3 - STAT
                // Laboratory processing already started
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 3,
                    RequestNumber = "NMB-REQ-000003",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 22, 8, 15, 0),

                    Urgency = Urgency.Stat,

                    ClinicalNotes =
                        "Emergency request - patient collapsed. Suspected DVT.",

                    Status = TestRequestStatus.InProgress.ToString()
                },


                // ========================================================
                // REQUEST 4 - ROUTINE
                // Completed request
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 4,
                    RequestNumber = "NMB-REQ-000004",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 22, 14, 0, 0),

                    Urgency = Urgency.Routine,

                    ClinicalNotes =
                        "Diabetes monitoring - HbA1c and glucose levels.",

                    Status = TestRequestStatus.Completed.ToString()
                },


                // ========================================================
                // REQUEST 5 - URGENT
                // Waiting for samples
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 5,
                    RequestNumber = "NMB-REQ-000005",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 23, 11, 20, 0),

                    Urgency = Urgency.Urgent,

                    ClinicalNotes =
                        "Chest pain - urgent laboratory investigation requested.",

                    Status = TestRequestStatus.Submitted.ToString()
                },


                // ========================================================
                // REQUEST 6 - ROUTINE
                // Samples already received
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 6,
                    RequestNumber = "NMB-REQ-000006",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 24, 9, 45, 0),

                    Urgency = Urgency.Routine,

                    ClinicalNotes =
                        "Routine liver function test for patient on medication.",

                    Status = TestRequestStatus.SamplesReceived.ToString()
                },


                // ========================================================
                // REQUEST 7 - STAT
                // Waiting for sample reception
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 7,
                    RequestNumber = "NMB-REQ-000007",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 25, 7, 30, 0),

                    Urgency = Urgency.Stat,

                    ClinicalNotes =
                        "Pre-surgery coagulation screen - STAT request.",

                    Status = TestRequestStatus.Submitted.ToString()
                },


                // ========================================================
                // REQUEST 8 - ROUTINE
                // Completed
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 8,
                    RequestNumber = "NMB-REQ-000008",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 25, 13, 0, 0),

                    Urgency = Urgency.Routine,

                    ClinicalNotes =
                        "Routine urinalysis - suspected UTI.",

                    Status = TestRequestStatus.Completed.ToString()
                },


                // ========================================================
                // REQUEST 9 - URGENT
                // Currently being processed
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 9,
                    RequestNumber = "NMB-REQ-000009",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 26, 8, 45, 0),

                    Urgency = Urgency.Urgent,

                    ClinicalNotes =
                        "Suspected bloodstream infection - blood culture requested.",

                    Status = TestRequestStatus.InProgress.ToString()
                },


                // ========================================================
                // REQUEST 10 - ROUTINE
                // Waiting for sample reception
                // ========================================================
                new TestRequestsModel
                {
                    TestRequestId = 10,
                    RequestNumber = "NMB-REQ-000010",

                    PatientId =
                        "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",

                    DoctorId =
                        "953f333d-2e68-46f6-ad26-0412fe75d908",

                    RequestDate =
                        new DateTime(2026, 9, 26, 14, 30, 0),

                    Urgency = Urgency.Routine,

                    ClinicalNotes =
                        "Annual physical examination - full blood count and chemistry panel.",

                    Status = TestRequestStatus.Submitted.ToString()
                }
            );


            // ============================================================
            // SAMPLE SEED DATA
            // ============================================================
            //
            // Submitted:
            //   TechnicianId = null
            //   DateReceived = null
            //   Status = "Submitted"
            //   IsReceived = false
            //
            // Received:
            //   TechnicianId = receiving technician
            //   DateReceived = populated
            //   Status = "Samples Received"
            //   IsReceived = true
            //
            // ============================================================
            // ============================================================
            // SAMPLE TYPE SEED DATA
            // ============================================================

            builder.Entity<SampleTypeModel>().HasData(

                new SampleTypeModel
                {
                    SampleTypeId = 1,
                    Name = "Blood",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 2,
                    Name = "Urine",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 3,
                    Name = "Saliva",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 4,
                    Name = "Plasma",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 5,
                    Name = "Serum",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 6,
                    Name = "Bone Marrow",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 7,
                    Name = "CSF",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 8,
                    Name = "Stool",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 9,
                    Name = "Sputum",
                    IsActive = true
                },

                new SampleTypeModel
                {
                    SampleTypeId = 10,
                    Name = "Swab",
                    IsActive = true
                }
            );
            builder.Entity<SampleModel>().HasData(

                // ========================================================
                // SAMPLE 1
                // Request 1 - Routine
                // Waiting for laboratory reception
                // ========================================================
                new SampleModel
                {
                    SampleId = 1,
                    TestRequestId = 1,
                    Barcode = "SMP-2026-000001",
                    SampleTypeId = 1, // Blood

                    TechnicianId = null,
                    DateReceived = null,

                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                },


                // ========================================================
                // SAMPLE 2
                // Request 2 - Urgent
                // Sample already received
                // ========================================================
                new SampleModel
                {
                    SampleId = 2,
                    TestRequestId = 2,
                    Barcode = "SMP-2026-000002",
                    SampleTypeId = 1, // Blood

                    TechnicianId =
                        "85a94ac3-e56a-4ae2-bea1-85d398486185",

                    DateReceived =
                        new DateTime(2026, 9, 21, 11, 0, 0),

                    Status = "Samples Received",
                    IsReceived = true,
                    IsDeleted = false
                },


                // ========================================================
                // SAMPLE 3
                // Request 3 - STAT
                // Sample received and request now being processed
                // ========================================================
                new SampleModel
                {
                    SampleId = 3,
                    TestRequestId = 3,
                    Barcode = "SMP-2026-000003",
                    SampleTypeId = 4, // Plasma

                    TechnicianId =
                        "85a94ac3-e56a-4ae2-bea1-85d398486185",

                    DateReceived =
                        new DateTime(2026, 9, 22, 8, 30, 0),

                    Status = "Samples Received",
                    IsReceived = true,
                    IsDeleted = false
                },


                // ========================================================
                // SAMPLE 4
                // Request 5 - Urgent
                // Waiting for reception
                // ========================================================
                new SampleModel
                {
                    SampleId = 4,
                    TestRequestId = 5,
                    Barcode = "SMP-2026-000005",
                    SampleTypeId = 4, // Plasma

                    TechnicianId = null,
                    DateReceived = null,

                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                },


                // ========================================================
                // SAMPLE 5
                // Request 6 - Routine
                // Sample received
                // ========================================================
                new SampleModel
                {
                    SampleId = 5,
                    TestRequestId = 6,
                    Barcode = "SMP-2026-000006",
                    SampleTypeId = 4, // Plasma

                    TechnicianId =
                        "85a94ac3-e56a-4ae2-bea1-85d398486185",

                    DateReceived =
                        new DateTime(2026, 9, 24, 10, 15, 0),

                    Status = "Samples Received",
                    IsReceived = true,
                    IsDeleted = false
                },


                // ========================================================
                // SAMPLE 6
                // Request 7 - STAT
                // Waiting for reception
                // ========================================================
                new SampleModel
                {
                    SampleId = 6,
                    TestRequestId = 7,
                    Barcode = "SMP-2026-000007",
                    SampleTypeId = 4, // Plasma

                    TechnicianId = null,
                    DateReceived = null,

                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                },


                // ========================================================
                // SAMPLE 7
                // Request 10 - Routine
                // Waiting for reception
                // ========================================================
                new SampleModel
                {
                    SampleId = 7,
                    TestRequestId = 10,
                    Barcode = "SMP-2026-000010",
                    SampleTypeId = 1, // Blood

                    TechnicianId = null,
                    DateReceived = null,

                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                }
            );
            // ============================================================
            // SAMPLE SEED DATA
            // ============================================================
            //
            // Submitted request:
            //      IsReceived = false
            //      TechnicianId = null
            //      DateReceived = null
            //
            // Samples Received / later request:
            //      IsReceived = true
            //      TechnicianId = technician
            //      DateReceived = populated
            //
            // ============================================================

 
            builder.Entity<TestCategoryModel>().HasData(
    new TestCategoryModel
    {
        TestCategoryId = 1,
        Description = "Hematology"
    },
    new TestCategoryModel
    {
        TestCategoryId = 2,
        Description = "Coagulation"
    },
    new TestCategoryModel
    {
        TestCategoryId = 3,
        Description = "Clinical Chemistry"
    },
    new TestCategoryModel
    {
        TestCategoryId = 4,
        Description = "Urinalysis"
    },
    new TestCategoryModel
    {
        TestCategoryId = 5,
        Description = "Microbiology"
    }
);

            builder.Entity<MeasurementUnitModel>().HasData(
    new MeasurementUnitModel
    {
        MeasurementUnitId = 1,
        Unit = "x10³/μL",
        Description = "Thousand cells per microliter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 2,
        Unit = "%",
        Description = "Percentage"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 3,
        Unit = "Qualitative",
        Description = "Positive/Negative"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 4,
        Unit = "mm/hr",
        Description = "Millimeters per hour"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 5,
        Unit = "seconds",
        Description = "Seconds"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 6,
        Unit = "ratio",
        Description = "Ratio"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 7,
        Unit = "ng/mL",
        Description = "Nanograms per milliliter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 8,
        Unit = "mg/dL",
        Description = "Milligrams per deciliter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 9,
        Unit = "mmol/L",
        Description = "Millimoles per liter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 10,
        Unit = "U/L",
        Description = "Units per liter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 11,
        Unit = "CFU/mL",
        Description = "Colony forming units per milliliter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 12,
        Unit = "g/dL",
        Description = "Grams per deciliter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 13,
        Unit = "fL",
        Description = "Femtoliters"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 14,
        Unit = "pg",
        Description = "Picograms"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 15,
        Unit = "x10⁶/μL",
        Description = "Million cells per microliter"
    },
    new MeasurementUnitModel
    {
        MeasurementUnitId = 16,
        Unit = "μmol/L",
        Description = "Micromoles per liter"
    }
);

            builder.Entity<ConsumableModel>().HasData(
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
        Name = "Test Tubes (Sodium Citrate)",
        QuantityOnHand = 300,
        ReorderLevel = 75
    },
    new ConsumableModel
    {
        ConsumableId = 3,
        Name = "Syringes 5ml",
        QuantityOnHand = 300,
        ReorderLevel = 50
    },
    new ConsumableModel
    {
        ConsumableId = 4,
        Name = "Blood Smear Slides",
        QuantityOnHand = 1000,
        ReorderLevel = 200
    },
    new ConsumableModel
    {
        ConsumableId = 5,
        Name = "ESR Racks and Tubes",
        QuantityOnHand = 50,
        ReorderLevel = 15
    },
    new ConsumableModel
    {
        ConsumableId = 6,
        Name = "Coagulation Test Cartridges",
        QuantityOnHand = 200,
        ReorderLevel = 50
    },
    new ConsumableModel
    {
        ConsumableId = 7,
        Name = "D-Dimer Test Kits",
        QuantityOnHand = 100,
        ReorderLevel = 30
    },
    new ConsumableModel
    {
        ConsumableId = 8,
        Name = "Chemistry Analyzer Reagents",
        QuantityOnHand = 50,
        ReorderLevel = 15
    },
    new ConsumableModel
    {
        ConsumableId = 9,
        Name = "Urine Dipsticks",
        QuantityOnHand = 500,
        ReorderLevel = 100
    },
    new ConsumableModel
    {
        ConsumableId = 10,
        Name = "Microscope Slides (Urine)",
        QuantityOnHand = 500,
        ReorderLevel = 100
    },
    new ConsumableModel
    {
        ConsumableId = 11,
        Name = "Culture Media Plates",
        QuantityOnHand = 300,
        ReorderLevel = 80
    },
    new ConsumableModel
    {
        ConsumableId = 12,
        Name = "Blood Culture Bottles",
        QuantityOnHand = 150,
        ReorderLevel = 40
    }
);
            builder.Entity<TestTypeModel>().HasData(
    new TestTypeModel
    {
        TestTypeId = 1,
        Name = "Full Blood Count (FBC)",
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
        Name = "White Blood Cell Differential",
        TestCategoryId = 1,
        SampleTypeId = 1,
        ConsumableId = 1,
        MeasurementUnit = 2,
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
        MeasurementUnit = 3,
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 4
    },
    new TestTypeModel
    {
        TestTypeId = 6,
        Name = "Prothrombin Time (PT)",
        TestCategoryId = 2,
        SampleTypeId = 4,
        ConsumableId = 6,
        MeasurementUnit = 5,
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
        MeasurementUnit = 5,
        NormalMin = 25m,
        NormalMax = 35m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 8,
        Name = "International Normalized Ratio (INR)",
        TestCategoryId = 2,
        SampleTypeId = 4,
        ConsumableId = 6,
        MeasurementUnit = 6,
        NormalMin = 0.8m,
        NormalMax = 1.2m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 11,
        Name = "Glucose (Fasting)",
        TestCategoryId = 3,
        SampleTypeId = 4,
        ConsumableId = 8,
        MeasurementUnit = 9,
        NormalMin = 3.9m,
        NormalMax = 5.5m,
        TurnAroundTimeHours = 1
    },
    new TestTypeModel
    {
        TestTypeId = 12,
        Name = "Hemoglobin A1C (HbA1c)",
        TestCategoryId = 3,
        SampleTypeId = 1,
        ConsumableId = 1,
        MeasurementUnit = 2,
        NormalMin = 4.0m,
        NormalMax = 5.6m,
        TurnAroundTimeHours = 2
    },
    new TestTypeModel
    {
        TestTypeId = 17,
        Name = "Urinalysis (Dipstick)",
        TestCategoryId = 4,
        SampleTypeId = 2,
        ConsumableId = 9,
        MeasurementUnit = 3,
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 1
    },
    new TestTypeModel
    {
        TestTypeId = 20,
        Name = "Blood Culture",
        TestCategoryId = 5,
        SampleTypeId = 1,
        ConsumableId = 12,
        MeasurementUnit = 3,
        NormalMin = 0m,
        NormalMax = 0m,
        TurnAroundTimeHours = 72
    }
);
            builder.Entity<TestTypeConsumableModel>().HasData(
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 1,
        TestTypeId = 1,
        ConsumableId = 1,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 2,
        TestTypeId = 2,
        ConsumableId = 1,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 3,
        TestTypeId = 3,
        ConsumableId = 4,
        QuantityRequired = 2
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 4,
        TestTypeId = 6,
        ConsumableId = 6,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 5,
        TestTypeId = 7,
        ConsumableId = 6,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 6,
        TestTypeId = 8,
        ConsumableId = 6,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 7,
        TestTypeId = 11,
        ConsumableId = 8,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 8,
        TestTypeId = 12,
        ConsumableId = 1,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 9,
        TestTypeId = 17,
        ConsumableId = 9,
        QuantityRequired = 1
    },
    new TestTypeConsumableModel
    {
        TestTypeConsumableId = 10,
        TestTypeId = 20,
        ConsumableId = 12,
        QuantityRequired = 2
    }
);

        }
    }
}
