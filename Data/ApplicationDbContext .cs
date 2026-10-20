using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;
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

        // ============================================================
        // IDENTITY / SECURITY
        // ============================================================

        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }


        // ============================================================
        // USER PROFILES
        // ============================================================

        public DbSet<DoctorProfile> DoctorProfiles { get; set; }

        public DbSet<TechnicianProfile> TechnicianProfiles { get; set; }
        public DbSet<TechnicianSentEmailModel> TechnicianSentEmails
        { get; set; }
        public DbSet<TechnicianNotificationModel> TechnicianNotifications
        { get; set; }
        // ============================================================
        // TEST REQUESTS
        // ============================================================

        public DbSet<TestRequestsModel> TestRequests { get; set; }

        public DbSet<SampleModel> Samples { get; set; }

        public DbSet<SampleTypeModel> SampleTypes { get; set; }

        public DbSet<TestCategoryModel> TestCategories { get; set; }

        public DbSet<TestTypeModel> TestTypes { get; set; }

        public DbSet<TestRequestTestTypeModel> TestRequestTestTypes { get; set; }

        public DbSet<MeasurementUnitModel> MeasurementUnits { get; set; }

        // ============================================================
        // AT LEAST ONE TEST TYPE SHOULD BE ASSIGNED TO EACH TECHNICIAN
        // MULTIPLE TECHNICIANS COULD BE ASSIGNED THE SAME TEST TYPE
        // TEST  TYPES THAT THE TECHNICIAN CAN PERFORM SHOULD BE INDICATED FOR EACH
        // TECHNICIAN
        // ============================================================
        public DbSet<TechnicianTestTypeModel> TechnicianTestTypes { get; set; }

        public DbSet<TestTypeConsumableModel> TestTypeConsumables { get; set; }


        // ============================================================
        // MANAGER - CONSUMABLES & ORDERS
        // ============================================================

        public DbSet<ConsumableModel> Consumables { get; set; }
        public DbSet<TestResultModel> TestResults { get; set; }

        public DbSet<StockStatusHistoryModel> StockStatusHistories { get; set; }

        public DbSet<SupplierModel> Suppliers { get; set; }

        public DbSet<OrderModel> Orders { get; set; }

        public DbSet<OrderItemModel> OrderItems { get; set; }

        public DbSet<StockAdjustmentModel> StockAdjustments { get; set; }


        // ============================================================
        // PATIENT MODELS
        // ============================================================

        public DbSet<PatientModel> Patients { get; set; }

        public DbSet<PatientConditionModel> PatientConditions { get; set; }

        public DbSet<PatientAllergyModel> PatientAllergies { get; set; }

        public DbSet<PatientMedicationModel> PatientMedications { get; set; }

        public DbSet<PatientDoctorConsentModel> PatientDoctorConsents { get; set; }

        public DbSet<ConsentRequestItemModel> ConsentRequestItems { get; set; }


        // ============================================================
        // ADMINISTRATOR
        // ============================================================

        public DbSet<MedicalConditionCategory> MedicalConditionCategories { get; set; }

        public DbSet<MedicalCondition> MedicalConditions { get; set; }

        public DbSet<AllergyCategory> AllergyCategories { get; set; }

        public DbSet<Allergy> Allergies { get; set; }


        // ============================================================
        // TECHNICIAN
        // ============================================================


        public DbSet<TestResultVerificationHistoryModel> TestResultVerificationHistories
        { get; set; }

        public DbSet<MedicationCategory> MedicationCategories { get; set; }
        public DbSet<TestTypeMeasurementModel> TestTypeMeasurements { get; set; }
        public DbSet<Medication> Medications { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TestResultModel>(entity =>
            {
                entity.Property(x => x.ReferenceMin)
                    .HasPrecision(18, 4);

                entity.Property(x => x.ReferenceMax)
                    .HasPrecision(18, 4);

                entity.Property(x => x.PreviousReferenceMin)
                    .HasPrecision(18, 4);

                entity.Property(x => x.PreviousReferenceMax)
                    .HasPrecision(18, 4);

                entity.Property(x => x.NewReferenceMin)
                    .HasPrecision(18, 4);

                entity.Property(x => x.NewReferenceMax)
                    .HasPrecision(18, 4);
            });
            builder.Entity<TechnicianSentEmailModel>()
    .HasIndex(x => x.TestRequestId)
    .IsUnique();
            builder.Entity<TestResultVerificationHistoryModel>(entity =>
            {
                entity.Property(x => x.PreviousReferenceMin)
                    .HasPrecision(18, 4);

                entity.Property(x => x.PreviousReferenceMax)
                    .HasPrecision(18, 4);

                entity.Property(x => x.NewReferenceMin)
                    .HasPrecision(18, 4);

                entity.Property(x => x.NewReferenceMax)
                    .HasPrecision(18, 4);
            });



            builder.Entity<TestResultVerificationHistoryModel>(entity =>
            {

                entity.HasOne(x => x.TestResult)
                    .WithMany()
                    .HasForeignKey(x => x.TestResultId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ActionByTechnician)
                    .WithMany()
                    .HasForeignKey(x => x.ActionByTechnicianId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<TestResultModel>(entity =>
            {
                entity.HasOne(x => x.TestRequestTestType)
                    .WithMany()
                    .HasForeignKey(x => x.TestRequestTestTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Technician)
                    .WithMany()
                    .HasForeignKey(x => x.TechnicianId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.VerifiedByTechnician)
                    .WithMany()
                    .HasForeignKey(x => x.VerifiedByTechnicianId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.ReferenceMin)
                    .HasPrecision(18, 4);

                entity.Property(x => x.ReferenceMax)
                    .HasPrecision(18, 4);
            });

            // ============================================================
            // USER PROFILE RELATIONSHIPS
            // ============================================================

            // ------------------------------------------------------------
            // APPLICATION USER -> DOCTOR PROFILE
            // ------------------------------------------------------------

            builder.Entity<DoctorProfile>()
                .HasOne(x => x.User)
                .WithOne(x => x.DoctorProfile)
                .HasForeignKey<DoctorProfile>(
                    x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ------------------------------------------------------------
            // APPLICATION USER -> TECHNICIAN PROFILE
            // ------------------------------------------------------------

            builder.Entity<TechnicianProfile>()
                .HasOne(x => x.User)
                .WithOne(x => x.TechnicianProfile)
                .HasForeignKey<TechnicianProfile>(
                    x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================================
            // USER PROFILE UNIQUE CONSTRAINTS
            // ============================================================

            // ------------------------------------------------------------
            // HPCSA NUMBER
            // ------------------------------------------------------------

            builder.Entity<DoctorProfile>()
                .HasIndex(x => x.HPCSANumber)
                .IsUnique();


            // ------------------------------------------------------------
            // SOUTH AFRICAN ID NUMBER
            // ------------------------------------------------------------

            builder.Entity<TechnicianProfile>()
                .HasIndex(x => x.IdNumber)
                .IsUnique();


            // ------------------------------------------------------------
            // EMPLOYEE NUMBER
            // ------------------------------------------------------------

            builder.Entity<TechnicianProfile>()
                .HasIndex(x => x.EmployeeNumber)
                .IsUnique();


            // ============================================================
            // TEST CATEGORY STATUS
            // ============================================================

            builder.Entity<TestCategoryModel>()
                .Property(x => x.IsActive)
                .HasConversion<int>()
                .HasDefaultValue(
                    TestCategoryModel.RecordStatus.Active);


            // ============================================================
            // ADMINISTRATOR MODEL RELATIONSHIPS
            // ============================================================

            builder.Entity<MedicalCondition>()
                .HasOne(x => x.Category)
                .WithMany(x => x.MedicalConditions)
                .HasForeignKey(x => x.MedicalConditionCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Allergy>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Allergies)
                .HasForeignKey(x => x.AllergyCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Medication>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Medications)
                .HasForeignKey(x => x.MedicationCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // CONSUMABLE -> SUPPLIER
            // ============================================================

            builder.Entity<ConsumableModel>()
                .HasOne(c => c.Supplier)
                .WithMany(s => s.Consumables)
                .HasForeignKey(c => c.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // CONSUMABLE -> STOCK STATUS HISTORY
            // ============================================================

            builder.Entity<StockStatusHistoryModel>()
                .HasOne(x => x.Consumable)
                .WithMany(c => c.StockStatusHistories)
                .HasForeignKey(x => x.ConsumableId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================================
            // TEST TYPE -> TEST CATEGORY
            // ============================================================

            builder.Entity<TestTypeModel>()
                .HasOne(x => x.TestCategory)
                .WithMany()
                .HasForeignKey(x => x.TestCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // TEST TYPE -> SAMPLE TYPE
            // ============================================================

            builder.Entity<TestTypeModel>()
                .HasOne(x => x.SampleType)
                .WithMany()
                .HasForeignKey(x => x.SampleTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // TEST TYPE -> MEASUREMENT UNIT
            // ============================================================

            builder.Entity<TestTypeModel>()
                .HasOne(x => x.MeasurementUnitDetails)
                .WithMany()
                .HasForeignKey(x => x.MeasurementUnit)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // TEST TYPE -> TECHNICIAN ASSIGNMENTS
            // ============================================================

            builder.Entity<TechnicianTestTypeModel>()
                .HasOne(x => x.TestType)
                .WithMany(x => x.TechnicianTestTypes)
                .HasForeignKey(x => x.TestTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // TEST TYPE -> CONSUMABLE
            // THROUGH TestTypeConsumable
            // ============================================================

            builder.Entity<TestTypeConsumableModel>()
                .HasKey(x => x.TestTypeConsumableId);


            builder.Entity<TestTypeConsumableModel>()
                .HasOne(x => x.TestType)
                .WithMany(x => x.TestTypeConsumables)
                .HasForeignKey(x => x.TestTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<TestTypeConsumableModel>()
                .HasOne(x => x.Consumable)
                .WithMany(x => x.TestTypeConsumables)
                .HasForeignKey(x => x.ConsumableId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // ORDER -> SUPPLIER
            // ============================================================

            builder.Entity<OrderModel>()
                .HasOne(o => o.Supplier)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // ORDER -> ORDER ITEMS
            // ============================================================

            builder.Entity<OrderItemModel>()
                .HasOne(i => i.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================================
            // ORDER ITEM -> CONSUMABLE
            // ============================================================

            builder.Entity<OrderItemModel>()
                .HasOne(i => i.Consumable)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(i => i.ConsumableId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // CONSUMABLE -> STOCK ADJUSTMENTS
            // ============================================================

            builder.Entity<StockAdjustmentModel>()
                .HasOne(x => x.Consumable)
                .WithMany(c => c.StockAdjustments)
                .HasForeignKey(x => x.ConsumableId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================================
            // UNIQUE CONSTRAINTS
            // ============================================================

            builder.Entity<SupplierModel>()
                .HasIndex(s => s.SupplierName)
                .IsUnique();

            builder.Entity<ConsumableModel>()
                .HasIndex(c => c.Name)
                .IsUnique();

            builder.Entity<OrderModel>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();


            // ============================================================
            // PASSWORD RESET OTP
            // ============================================================

            builder.Entity<PasswordResetOtp>()
                .HasOne(x => x.User)
                .WithMany(x => x.PasswordResetOtps)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================================
            // DEVELOPMENT DEMO: LERATO PATIENT PROFILE
            // PatientId is distinct from AspNetUsers.Id (UserId).
            // All demographic values below are fictional test data.
            // ============================================================
            builder.Entity<PatientModel>().HasData(
                new PatientModel
                {
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    UserId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    Name = "Lerato",
                    Surname = "Patient",
                    IDNumber = "0001015009087",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    ContactNumber = "0710000000",
                    Email = "lerato.demo@example.com",
                    Address = "Demo Address",
                    Suburb = "Central",
                    City = "Gqeberha",
                    Province = "Eastern Cape",
                    PostalCode = "6001",
                    NoKnownMedicalConditions = false,
                    NoKnownAllergies = false,
                    NoCurrentMedications = false,
                    CreatedDate = new DateTime(2026, 10, 17)
                }
            );

            // ============================================================
            // TEST REQUEST SEED DATA
            // ============================================================

            builder.Entity<TestRequestsModel>().HasData(

                new TestRequestsModel
                {
                    TestRequestId = 1,
                    RequestNumber = "NMB-REQ-000001",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 20, 9, 0, 0),
                    Urgency = Urgency.Routine,
                    ClinicalNotes =
                        "Routine blood test - annual checkup.",
                    Status =
                        TestRequestStatus.Submitted.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 2,
                    RequestNumber = "NMB-REQ-000002",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 21, 10, 30, 0),
                    Urgency = Urgency.Urgent,
                    ClinicalNotes =
                        "Patient presenting with fever and chills. Suspected infection.",
                    Status =
                        TestRequestStatus.SamplesReceived.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 3,
                    RequestNumber = "NMB-REQ-000003",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 22, 8, 15, 0),
                    Urgency = Urgency.Stat,
                    ClinicalNotes =
                        "Emergency request - patient collapsed. Suspected DVT.",
                    Status =
                        TestRequestStatus.InProgress.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 4,
                    RequestNumber = "NMB-REQ-000004",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 22, 14, 0, 0),
                    Urgency = Urgency.Routine,
                    ClinicalNotes =
                        "Diabetes monitoring - HbA1c and glucose levels.",
                    Status =
                        TestRequestStatus.Completed.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 5,
                    RequestNumber = "NMB-REQ-000005",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 23, 11, 20, 0),
                    Urgency = Urgency.Urgent,
                    ClinicalNotes =
                        "Chest pain - urgent laboratory investigation requested.",
                    Status =
                        TestRequestStatus.Submitted.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 6,
                    RequestNumber = "NMB-REQ-000006",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 24, 9, 45, 0),
                    Urgency = Urgency.Routine,
                    ClinicalNotes =
                        "Routine liver function test for patient on medication.",
                    Status =
                        TestRequestStatus.SamplesReceived.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 7,
                    RequestNumber = "NMB-REQ-000007",
                    PatientId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 25, 7, 30, 0),
                    Urgency = Urgency.Stat,
                    ClinicalNotes =
                        "Pre-surgery coagulation screen - STAT request.",
                    Status = TestRequestStatus.Submitted.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 8,
                    RequestNumber = "NMB-REQ-000008",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 25, 13, 0, 0),
                    Urgency = Urgency.Routine,
                    ClinicalNotes =
                        "Routine urinalysis - suspected UTI.",
                    Status =
                        TestRequestStatus.Completed.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 9,
                    RequestNumber = "NMB-REQ-000009",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 26, 8, 45, 0),
                    Urgency = Urgency.Urgent,
                    ClinicalNotes =
                        "Suspected bloodstream infection - blood culture requested.",
                    Status =
                        TestRequestStatus.InProgress.ToString()
                },

                new TestRequestsModel
                {
                    TestRequestId = 10,
                    RequestNumber = "NMB-REQ-000010",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(
                        2026, 9, 26, 14, 30, 0),
                    Urgency = Urgency.Routine,
                    ClinicalNotes =
                        "Annual physical examination - full blood count and chemistry panel.",
                    Status =
                        TestRequestStatus.Submitted.ToString()
                }
            );


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


            // ============================================================
            // SAMPLE SEED DATA
            // ============================================================

            builder.Entity<SampleModel>().HasData(

                new SampleModel
                {
                    SampleId = 1,
                    TestRequestId = 1,
                    Barcode = "SMP-2026-000001",
                    SampleTypeId = 1,
                    TechnicianId = null,
                    DateReceived = null,
                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                },

                new SampleModel
                {
                    SampleId = 2,
                    TestRequestId = 2,
                    Barcode = "SMP-2026-000002",
                    SampleTypeId = 1,
                    TechnicianId =
                        "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    DateReceived =
                        new DateTime(2026, 9, 21, 11, 0, 0),
                    Status = "Samples Received",
                    IsReceived = true,
                    IsDeleted = false
                },

                new SampleModel
                {
                    SampleId = 3,
                    TestRequestId = 3,
                    Barcode = "SMP-2026-000003",
                    SampleTypeId = 4,
                    TechnicianId =
                        "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    DateReceived =
                        new DateTime(2026, 9, 22, 8, 30, 0),
                    Status = "Samples Received",
                    IsReceived = true,
                    IsDeleted = false
                },

                new SampleModel
                {
                    SampleId = 4,
                    TestRequestId = 5,
                    Barcode = "SMP-2026-000005",
                    SampleTypeId = 4,
                    TechnicianId = null,
                    DateReceived = null,
                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                },

                new SampleModel
                {
                    SampleId = 5,
                    TestRequestId = 6,
                    Barcode = "SMP-2026-000006",
                    SampleTypeId = 4,
                    TechnicianId =
                        "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    DateReceived =
                        new DateTime(2026, 9, 24, 10, 15, 0),
                    Status = "Samples Received",
                    IsReceived = true,
                    IsDeleted = false
                },

                new SampleModel
                {
                    SampleId = 6,
                    TestRequestId = 7,
                    Barcode = "SMP-2026-000007",
                    SampleTypeId = 4,
                    TechnicianId = null,
                    DateReceived = null,
                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                },

                new SampleModel
                {
                    SampleId = 7,
                    TestRequestId = 10,
                    Barcode = "SMP-2026-000010",
                    SampleTypeId = 1,
                    TechnicianId = null,
                    DateReceived = null,
                    Status = "Submitted",
                    IsReceived = false,
                    IsDeleted = false
                }
            );


            // ============================================================
            // TEST CATEGORIES
            // ============================================================

            builder.Entity<TestCategoryModel>().HasData(

                new TestCategoryModel
                {
                    TestCategoryId = 1,
                    Description = "Hematology",
                    IsActive =
                        TestCategoryModel.RecordStatus.Active
                },

                new TestCategoryModel
                {
                    TestCategoryId = 2,
                    Description = "Coagulation",
                    IsActive =
                        TestCategoryModel.RecordStatus.Active
                },

                new TestCategoryModel
                {
                    TestCategoryId = 3,
                    Description = "Clinical Chemistry",
                    IsActive =
                        TestCategoryModel.RecordStatus.Active
                },

                new TestCategoryModel
                {
                    TestCategoryId = 4,
                    Description = "Urinalysis",
                    IsActive =
                        TestCategoryModel.RecordStatus.Active
                },

                new TestCategoryModel
                {
                    TestCategoryId = 5,
                    Description = "Microbiology",
                    IsActive =
                        TestCategoryModel.RecordStatus.Active
                }
            );


            // ============================================================
            // MEASUREMENT UNITS
            // ============================================================

            builder.Entity<MeasurementUnitModel>().HasData(

                new MeasurementUnitModel
                {
                    MeasurementUnitId = 1,
                    Unit = "x10³/μL",
                    Description =
                        "Thousand cells per microliter"
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
                    Description =
                        "Millimeters per hour"
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
                    Description =
                        "Nanograms per milliliter"
                },

                new MeasurementUnitModel
                {
                    MeasurementUnitId = 8,
                    Unit = "mg/dL",
                    Description =
                        "Milligrams per deciliter"
                },

                new MeasurementUnitModel
                {
                    MeasurementUnitId = 9,
                    Unit = "mmol/L",
                    Description =
                        "Millimoles per liter"
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
                    Description =
                        "Colony forming units per milliliter"
                },

                new MeasurementUnitModel
                {
                    MeasurementUnitId = 12,
                    Unit = "g/dL",
                    Description =
                        "Grams per deciliter"
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
                    Description =
                        "Million cells per microliter"
                },

                new MeasurementUnitModel
                {
                    MeasurementUnitId = 16,
                    Unit = "μmol/L",
                    Description =
                        "Micromoles per liter"
                }
            );


            // ============================================================
            // SUPPLIER SEED DATA
            // ============================================================

            builder.Entity<SupplierModel>().HasData(

                new SupplierModel
                {
                    SupplierId = 1,
                    SupplierName = "MedLab Supplies",
                    ContactPerson =
                        "Laboratory Supplies Department",
                    Email =
                        "supplies@medlabsupplies.example",
                    IsDeleted = false
                },

                new SupplierModel
                {
                    SupplierId = 2,
                    SupplierName =
                        "BioMedical Distributors",
                    ContactPerson =
                        "Sales Department",
                    Email =
                        "sales@biomedical.example",
                    IsDeleted = false
                },

                new SupplierModel
                {
                    SupplierId = 3,
                    SupplierName =
                        "Clinical Diagnostics Ltd",
                    ContactPerson =
                        "Diagnostics Sales",
                    Email =
                        "orders@clinicaldiagnostics.example",
                    IsDeleted = false
                }
            );


            // ============================================================
            // CONSUMABLE SEED DATA
            // ============================================================

            builder.Entity<ConsumableModel>().HasData(

                new ConsumableModel
                {
                    ConsumableId = 1,
                    Name = "Test Tubes (EDTA)",
                    QuantityOnHand = 500,
                    ReorderLevel = 100,
                    SupplierId = 1,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 2,
                    Name = "Test Tubes (Sodium Citrate)",
                    QuantityOnHand = 300,
                    ReorderLevel = 75,
                    SupplierId = 1,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 3,
                    Name = "Syringes 5ml",
                    QuantityOnHand = 300,
                    ReorderLevel = 50,
                    SupplierId = 2,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 4,
                    Name = "Blood Smear Slides",
                    QuantityOnHand = 1000,
                    ReorderLevel = 200,
                    SupplierId = 2,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 5,
                    Name = "ESR Racks and Tubes",
                    QuantityOnHand = 50,
                    ReorderLevel = 15,
                    SupplierId = 2,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 6,
                    Name = "Coagulation Test Cartridges",
                    QuantityOnHand = 200,
                    ReorderLevel = 50,
                    SupplierId = 3,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 7,
                    Name = "D-Dimer Test Kits",
                    QuantityOnHand = 100,
                    ReorderLevel = 30,
                    SupplierId = 3,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 8,
                    Name = "Chemistry Analyzer Reagents",
                    QuantityOnHand = 50,
                    ReorderLevel = 15,
                    SupplierId = 3,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 9,
                    Name = "Urine Dipsticks",
                    QuantityOnHand = 500,
                    ReorderLevel = 100,
                    SupplierId = 2,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 10,
                    Name = "Microscope Slides (Urine)",
                    QuantityOnHand = 500,
                    ReorderLevel = 100,
                    SupplierId = 2,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 11,
                    Name = "Culture Media Plates",
                    QuantityOnHand = 300,
                    ReorderLevel = 80,
                    SupplierId = 3,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                },

                new ConsumableModel
                {
                    ConsumableId = 12,
                    Name = "Blood Culture Bottles",
                    QuantityOnHand = 150,
                    ReorderLevel = 40,
                    SupplierId = 3,
                    StockStatus = StockStatus.Sufficient,
                    IsDeleted = false
                }
            );


            // ============================================================
            // TEST TYPES
            // ============================================================

            builder.Entity<TestTypeModel>().HasData(

                new TestTypeModel
                {
                    TestTypeId = 1,
                    Name = "Full Blood Count (FBC)",
                    TestCategoryId = 1,
                    SampleTypeId = 1,
                    MeasurementUnit = 1,
                    NormalMin = 4.5m,
                    NormalMax = 11.0m,
                    TurnAroundTimeHours = 120, // 120 minutes
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 2,
                    Name = "White Blood Cell Differential",
                    TestCategoryId = 1,
                    SampleTypeId = 1,
                    MeasurementUnit = 2, // %
                    NormalMin = 0m,
                    NormalMax = 0m, // Individual cell types need separate ranges
                    TurnAroundTimeHours = 180,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 3,
                    Name = "Peripheral Blood Film",
                    TestCategoryId = 1,
                    SampleTypeId = 1,
                    MeasurementUnit = 3, // Qualitative
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 240,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 6,
                    Name = "Prothrombin Time (PT)",
                    TestCategoryId = 2,
                    SampleTypeId = 4,
                    MeasurementUnit = 5, // seconds
                    NormalMin = 11.0m,
                    NormalMax = 13.5m,
                    TurnAroundTimeHours = 120,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 7,
                    Name = "Activated Partial Thromboplastin Time (aPTT)",
                    TestCategoryId = 2,
                    SampleTypeId = 4,
                    MeasurementUnit = 5, // seconds
                    NormalMin = 25m,
                    NormalMax = 35m,
                    TurnAroundTimeHours = 120,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 8,
                    Name = "International Normalized Ratio (INR)",
                    TestCategoryId = 2,
                    SampleTypeId = 4,
                    MeasurementUnit = 6, // ratio
                    NormalMin = 0.8m,
                    NormalMax = 1.2m,
                    TurnAroundTimeHours = 120,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 11,
                    Name = "Glucose (Fasting)",
                    TestCategoryId = 3,
                    SampleTypeId = 4,
                    MeasurementUnit = 9, // mmol/L
                    NormalMin = 3.9m,
                    NormalMax = 5.5m,
                    TurnAroundTimeHours = 60,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 12,
                    Name = "Hemoglobin A1C (HbA1c)",
                    TestCategoryId = 3,
                    SampleTypeId = 1,
                    MeasurementUnit = 2, // %
                    NormalMin = 4.0m,
                    NormalMax = 5.6m,
                    TurnAroundTimeHours = 120,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 17,
                    Name = "Urinalysis (Dipstick)",
                    TestCategoryId = 4,
                    SampleTypeId = 2,
                    MeasurementUnit = 3, // Qualitative
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 60,
                    IsActive = TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 20,
                    Name = "Blood Culture",
                    TestCategoryId = 5,
                    SampleTypeId = 1,
                    MeasurementUnit = 3, // Qualitative
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 4320, // 72 hours
                    IsActive = TestTypeModel.RecordStatus.Active
                }
            );

            builder.Entity<TechnicianTestTypeModel>().HasData(
                new TechnicianTestTypeModel
                {
                    TechnicianTestTypeId = 20,
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    TestTypeId = 1,
                    ManagerId = "1f4afa33-baa8-4f46-8143-2e2740c68fe8",
                    AssignedDate = new DateTime(2026, 1, 10),
                    IsActive = true,
                    Notes = "Assigned to perform routine laboratory tests."
                },
                new TechnicianTestTypeModel
                {
                    TechnicianTestTypeId = 30,
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    TestTypeId = 2,
                    ManagerId = "1f4afa33-baa8-4f46-8143-2e2740c68fe8",
                    AssignedDate = new DateTime(2026, 1, 11),
                    IsActive = true,
                    Notes = "Responsible for processing and analysing test samples."
                },
                new TechnicianTestTypeModel
                {
                    TechnicianTestTypeId = 40,
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    TestTypeId = 3,
                    ManagerId = "1f4afa33-baa8-4f46-8143-2e2740c68fe8",
                    AssignedDate = new DateTime(2026, 1, 12),
                    IsActive = true,
                    Notes = "Authorised to conduct specialised laboratory tests."
                },
                new TechnicianTestTypeModel
                {
                    TechnicianTestTypeId = 4,
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    TestTypeId = 1,
                    ManagerId = "1f4afa33-baa8-4f46-8143-2e2740c68fe8",
                    AssignedDate = new DateTime(2026, 1, 13),
                    IsActive = false,
                    Notes = "Assignment temporarily deactivated pending review."
                },
                new TechnicianTestTypeModel
                {
                    TechnicianTestTypeId = 5,
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    TestTypeId = 3,
                    ManagerId = "1f4afa33-baa8-4f46-8143-2e2740c68fe8",
                    AssignedDate = new DateTime(2026, 1, 14),
                    IsActive = true,
                    Notes = "Assigned additional testing responsibilities."
                }
            );


            // ============================================================
            // TEST REQUEST TEST TYPES SEED DATA
            // Individual tests requested by doctors
            // ============================================================


            builder.Entity<TestRequestTestTypeModel>().HasData(

                // Request 1 - Awaiting samples
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 1,
                    TestRequestId = 1,
                    TestTypeId = 1,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // Request 2 - Samples received
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 2,
                    TestRequestId = 2,
                    TestTypeId = 1,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 3,
                    TestRequestId = 2,
                    TestTypeId = 2,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // Request 3 - Samples received, request in progress
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 4,
                    TestRequestId = 3,
                    TestTypeId = 3,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // Request 5 - Awaiting samples
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 5,
                    TestRequestId = 5,
                    TestTypeId = 6,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // Request 6 - Samples received
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 6,
                    TestRequestId = 6,
                    TestTypeId = 3,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // Request 9 - Samples received, request in progress
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 7,
                    TestRequestId = 9,
                    TestTypeId = 3,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // Request 10 - Awaiting samples
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 8,
                    TestRequestId = 10,
                    TestTypeId = 1,
                    TechnicianId = null,
                    Status = "Submitted",
                    StartDateTime = null,
                    CompletionDateTime = null
                },

                // DEVELOPMENT DEMO: Technician 1 completed this test.
                // Technician 2 has an active assignment for TestTypeId 3
                // and can independently verify the result.
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 1001,
                    TestRequestId = 4,
                    TestTypeId = 3,
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    Status = "Completed",
                    StartDateTime = new DateTime(2026, 10, 14, 8, 30, 0),
                    CompletionDateTime = new DateTime(2026, 10, 14, 10, 0, 0)
                },

                // DEVELOPMENT DEMO: Technician 2 completed this test.
                // Technician 1 can independently verify it.
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 1002,
                    TestRequestId = 8,
                    TestTypeId = 3,
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    Status = "Completed",
                    StartDateTime = new DateTime(2026, 10, 14, 9, 0, 0),
                    CompletionDateTime = new DateTime(2026, 10, 14, 11, 15, 0)
                },

                // DEVELOPMENT DEMO: Returned to Technician 1 for correction.
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 1003,
                    TestRequestId = 3,
                    TestTypeId = 3,
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    Status = "To Be Reviewed",
                    StartDateTime = new DateTime(2026, 10, 13, 8, 0, 0),
                    CompletionDateTime = new DateTime(2026, 10, 13, 9, 30, 0)
                },

                // DEVELOPMENT DEMO: Returned to Technician 2 for correction.
                new TestRequestTestTypeModel
                {
                    TestRequestTestTypeId = 1004,
                    TestRequestId = 6,
                    TestTypeId = 3,
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    Status = "To Be Reviewed",
                    StartDateTime = new DateTime(2026, 10, 13, 10, 0, 0),
                    CompletionDateTime = new DateTime(2026, 10, 13, 12, 0, 0)
                }
            );

            // ============================================================
            // DEVELOPMENT-ONLY REVIEW QUEUE RESULTS
            // Fictional values for UI/workflow testing; not clinical records.
            // ============================================================
            builder.Entity<TestResultModel>().HasData(
                new TestResultModel
                {
                    TestResultId = 2001,
                    TestRequestTestTypeId = 1001,
                    ResultName = "Peripheral Blood Film",
                    ResultValue = "No abnormal cells seen",
                    MeasurementUnit = "Qualitative",
                    IsAbnormal = false,
                    ResultFlag = "Not Evaluated",
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    Notes = "DEMO DATA ONLY - not for clinical use.",
                    CapturedDateTime = new DateTime(2026, 10, 14, 9, 50, 0, DateTimeKind.Utc),
                    RequiresReview = false,
                    ReviewNotes = null,
                    VerifiedByTechnicianId = null,
                    VerifiedDateTime = null
                },
                new TestResultModel
                {
                    TestResultId = 2002,
                    TestRequestTestTypeId = 1002,
                    ResultName = "Peripheral Blood Film",
                    ResultValue = "No abnormal cells seen",
                    MeasurementUnit = "Qualitative",
                    IsAbnormal = false,
                    ResultFlag = "Not Evaluated",
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    Notes = "DEMO DATA ONLY - not for clinical use.",
                    CapturedDateTime = new DateTime(2026, 10, 14, 11, 0, 0, DateTimeKind.Utc),
                    RequiresReview = false,
                    ReviewNotes = null,
                    VerifiedByTechnicianId = null,
                    VerifiedDateTime = null
                },
                new TestResultModel
                {
                    TestResultId = 2003,
                    TestRequestTestTypeId = 1003,
                    ResultName = "Peripheral Blood Film",
                    ResultValue = "Film quality inadequate",
                    MeasurementUnit = "Qualitative",
                    IsAbnormal = false,
                    ResultFlag = "Not Evaluated",
                    TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    Notes = "DEMO DATA ONLY - not for clinical use.",
                    CapturedDateTime = new DateTime(2026, 10, 13, 9, 15, 0, DateTimeKind.Utc),
                    RequiresReview = true,
                    ReviewNotes = "Please prepare a new blood film, review the morphology, and correct the recorded finding before resubmission.",
                    VerifiedByTechnicianId = null,
                    VerifiedDateTime = null
                },
                new TestResultModel
                {
                    TestResultId = 2004,
                    TestRequestTestTypeId = 1004,
                    ResultName = "Peripheral Blood Film",
                    ResultValue = "Cell morphology unclear",
                    MeasurementUnit = "Qualitative",
                    IsAbnormal = false,
                    ResultFlag = "Not Evaluated",
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    Notes = "DEMO DATA ONLY - not for clinical use.",
                    CapturedDateTime = new DateTime(2026, 10, 13, 11, 45, 0, DateTimeKind.Utc),
                    RequiresReview = true,
                    ReviewNotes = "Please repeat the film examination and provide a clear qualitative interpretation before resubmission.",
                    VerifiedByTechnicianId = null,
                    VerifiedDateTime = null
                }
            );

            // DEMO VERIFICATION AUDIT HISTORY
            builder.Entity<TestResultVerificationHistoryModel>().HasData(
                new TestResultVerificationHistoryModel
                {
                    TestResultVerificationHistoryId = 3001,
                    TestResultId = 2001,
                    ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    Action = "SubmittedForVerification",
                    Notes = "Test submitted for independent verification.",
                    ActionDateTime = new DateTime(2026, 10, 14, 10, 0, 0, DateTimeKind.Utc)
                },
                new TestResultVerificationHistoryModel
                {
                    TestResultVerificationHistoryId = 3002,
                    TestResultId = 2002,
                    ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    Action = "SubmittedForVerification",
                    Notes = "Test submitted for independent verification.",
                    ActionDateTime = new DateTime(2026, 10, 14, 11, 15, 0, DateTimeKind.Utc)
                },
                new TestResultVerificationHistoryModel
                {
                    TestResultVerificationHistoryId = 3003,
                    TestResultId = 2003,
                    ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    Action = "ReturnedForReview",
                    Notes = "Please prepare a new blood film, review the morphology, and correct the recorded finding before resubmission.",
                    ActionDateTime = new DateTime(2026, 10, 13, 9, 40, 0, DateTimeKind.Utc)
                },
                new TestResultVerificationHistoryModel
                {
                    TestResultVerificationHistoryId = 3004,
                    TestResultId = 2004,
                    ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb",
                    Action = "ReturnedForReview",
                    Notes = "Please repeat the film examination and provide a clear qualitative interpretation before resubmission.",
                    ActionDateTime = new DateTime(2026, 10, 13, 12, 10, 0, DateTimeKind.Utc)
                }
            );

            // ============================================================
            // TEST TYPE / CONSUMABLE SEED DATA
            // ============================================================

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

            // ============================================================
            // DEVELOPMENT DEMO: PATIENT MEDICAL HISTORY
            // Lerato Patient (AspNetUsers.LastName = "Patient")
            // AspNetUsers.Id = c0fbfbb4-daf5-44e3-bf57-acfded7baceb; patient profile ID = f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15
            // Lookup IDs 1 and 2 are seeded below in the patient-model lookup tables.
            // These are fictional demonstration records only.
            // ============================================================
            // Lookup records required by the patient-history foreign keys.
            // AllergyCategories(1) and MedicationCategories(1) already exist
            // in the development database; do not re-seed those parent rows.
            // Category 9001 is new: verify it is unused before migration.
            builder.Entity<MedicalConditionCategory>().HasData(
                new MedicalConditionCategory
                {
                    MedicalConditionCategoryId = 9001,
                    Name = "Demo chronic conditions",
                    Description = "DEMO DATA ONLY - laboratory assessment",
                    Status = (NMB_HLabSys.Areas.Administrator.Models.Enums.Status)1
                }
            );

            builder.Entity<AllergyModel>().HasData(
                new AllergyModel { AllergyId = 1, AllergyName = "Penicillin", Description = "DEMO DATA ONLY", AllergyCategoryId = 1 },
                new AllergyModel { AllergyId = 2, AllergyName = "Peanuts", Description = "DEMO DATA ONLY", AllergyCategoryId = 1 }
            );

            builder.Entity<MedicalConditionModel>().HasData(
                new MedicalConditionModel { MedicalConditionId = 1, ConditionName = "Hypertension", Description = "DEMO DATA ONLY", MedicalConditionCategoryId = 9001 },
                new MedicalConditionModel { MedicalConditionId = 2, ConditionName = "Type 2 diabetes", Description = "DEMO DATA ONLY", MedicalConditionCategoryId = 9001 }
            );

            builder.Entity<MedicationModel>().HasData(
                new MedicationModel { MedicationId = 1, MedicationName = "Metformin", Description = "DEMO DATA ONLY", MedicationCategoryId = 1 },
                new MedicationModel { MedicationId = 2, MedicationName = "Amlodipine", Description = "DEMO DATA ONLY", MedicationCategoryId = 1 }
            );

            builder.Entity<PatientConditionModel>().HasData(
                new PatientConditionModel
                {
                    PatientConditionId = 5001,
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    MedicalConditionId = 1,
                    DateDiagnosed = new DateTime(2024, 3, 12),
                    Notes = "DEMO DATA ONLY - example medical condition."
                },
                new PatientConditionModel
                {
                    PatientConditionId = 5002,
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    MedicalConditionId = 2,
                    DateDiagnosed = new DateTime(2025, 6, 18),
                    Notes = "DEMO DATA ONLY - example medical condition."
                }
            );

            builder.Entity<PatientAllergyModel>().HasData(
                new PatientAllergyModel
                {
                    PatientAllergyId = 6001,
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    AllergyId = 1,
                    Severity = "Moderate",
                    Notes = "DEMO DATA ONLY - fictional allergy history."
                },
                new PatientAllergyModel
                {
                    PatientAllergyId = 6002,
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    AllergyId = 2,
                    Severity = "Mild",
                    Notes = "DEMO DATA ONLY - fictional allergy history."
                }
            );

            builder.Entity<PatientMedicationModel>().HasData(
                new PatientMedicationModel
                {
                    PatientMedicationId = 7001,
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    MedicationId = 1,
                    Dosage = "500 mg",
                    Frequency = "Twice daily",
                    StartDate = new DateTime(2026, 1, 10),
                    EndDate = null,
                    Notes = "DEMO DATA ONLY - fictional medication history."
                },
                new PatientMedicationModel
                {
                    PatientMedicationId = 7002,
                    PatientId = "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15",
                    MedicationId = 2,
                    Dosage = "10 mg",
                    Frequency = "Once daily",
                    StartDate = new DateTime(2026, 2, 15),
                    EndDate = null,
                    Notes = "DEMO DATA ONLY - fictional medication history."
                }
            );

            // ============================================================
            // TECHNICIAN ASSESSMENT DATA - DEVELOPMENT / DEMO ONLY
            // Main technician: 63950297-85b0-4b81-934a-bad2bf77f3cb
            // No real clinical data. Verify IDs against your existing database.
            // ============================================================

            // Four Submitted requests (1, 5, 7, 10) each have three unreceived samples.
            // Request 7 is explicitly set to Submitted above to match ReceiveSamplesController.Index.
            // All barcodes and sample IDs are unique development-only values.
            builder.Entity<SampleModel>().HasData(
                new SampleModel { SampleId = 8101, TestRequestId = 1, Barcode = "SMP-2026-008101", SampleTypeId = 1, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8102, TestRequestId = 5, Barcode = "SMP-2026-008102", SampleTypeId = 4, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8103, TestRequestId = 7, Barcode = "SMP-2026-008103", SampleTypeId = 4, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8104, TestRequestId = 10, Barcode = "SMP-2026-008104", SampleTypeId = 1, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8201, TestRequestId = 1, Barcode = "SMP-2026-008201", SampleTypeId = 2, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8202, TestRequestId = 5, Barcode = "SMP-2026-008202", SampleTypeId = 1, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8203, TestRequestId = 7, Barcode = "SMP-2026-008203", SampleTypeId = 1, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false },
                new SampleModel { SampleId = 8204, TestRequestId = 10, Barcode = "SMP-2026-008204", SampleTypeId = 2, TechnicianId = null, DateReceived = null, Status = "Submitted", IsReceived = false, IsDeleted = false }
            );

            // Five selected tests awaiting capture; no result rows yet.
            // Test types 1, 2 and 3 are actively assigned to the main technician.
            builder.Entity<TestRequestTestTypeModel>().HasData(
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1101, TestRequestId = 2, TestTypeId = 1, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "In Progress", StartDateTime = new DateTime(2026, 10, 15, 8, 0, 0), CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1102, TestRequestId = 2, TestTypeId = 2, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "In Progress", StartDateTime = new DateTime(2026, 10, 15, 9, 0, 0), CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1103, TestRequestId = 3, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "In Progress", StartDateTime = new DateTime(2026, 10, 15, 10, 0, 0), CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1104, TestRequestId = 6, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "In Progress", StartDateTime = new DateTime(2026, 10, 15, 11, 0, 0), CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1105, TestRequestId = 9, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "In Progress", StartDateTime = new DateTime(2026, 10, 15, 12, 0, 0), CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1106, TestRequestId = 8, TestTypeId = 3, TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Status = "Completed", StartDateTime = new DateTime(2026, 10, 14, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 14, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1107, TestRequestId = 3, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "To Be Reviewed", StartDateTime = new DateTime(2026, 10, 14, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 14, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1201, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 1, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 1, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1202, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 2, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 2, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1203, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 3, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 3, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1204, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 4, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 4, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1205, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 5, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 5, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1206, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 6, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 6, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1207, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 7, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 7, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1208, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 8, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 8, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1209, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 9, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 9, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1210, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 10, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 10, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1211, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 11, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 11, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1212, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 12, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 12, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1213, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 13, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 13, 9, 0, 0) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1214, TestRequestId = 4, TestTypeId = 3, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 14, 8, 0, 0), CompletionDateTime = new DateTime(2026, 10, 14, 9, 0, 0) },
                // Fictional reporting-only demonstration records spanning multiple categories.
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1301, TestRequestId = 4, TestTypeId = 1, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 1, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1302, TestRequestId = 8, TestTypeId = 6, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 2, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 2, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1303, TestRequestId = 2, TestTypeId = 11, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 3, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 3, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1304, TestRequestId = 6, TestTypeId = 17, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 4, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 4, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1305, TestRequestId = 9, TestTypeId = 20, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 5, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 5, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1306, TestRequestId = 4, TestTypeId = 2, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 6, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1307, TestRequestId = 8, TestTypeId = 7, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 7, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 7, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1308, TestRequestId = 2, TestTypeId = 12, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 8, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 8, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1309, TestRequestId = 6, TestTypeId = 1, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 9, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 9, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1310, TestRequestId = 9, TestTypeId = 6, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 10, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 10, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1311, TestRequestId = 4, TestTypeId = 11, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 11, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 11, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1312, TestRequestId = 8, TestTypeId = 17, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 12, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 12, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1313, TestRequestId = 2, TestTypeId = 20, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 13, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 13, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1314, TestRequestId = 6, TestTypeId = 2, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 14, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 14, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1315, TestRequestId = 9, TestTypeId = 7, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 1, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1316, TestRequestId = 4, TestTypeId = 12, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 2, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 2, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1317, TestRequestId = 8, TestTypeId = 1, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 3, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 3, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1318, TestRequestId = 2, TestTypeId = 6, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 4, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 4, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1319, TestRequestId = 6, TestTypeId = 11, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 5, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 5, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1320, TestRequestId = 9, TestTypeId = 17, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 6, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1321, TestRequestId = 4, TestTypeId = 20, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 7, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 7, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1322, TestRequestId = 8, TestTypeId = 2, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 8, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 8, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1323, TestRequestId = 2, TestTypeId = 7, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 9, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 9, 11, 0, 0, DateTimeKind.Utc) },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1324, TestRequestId = 6, TestTypeId = 12, TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Status = "Verified", StartDateTime = new DateTime(2026, 10, 10, 10, 0, 0, DateTimeKind.Utc), CompletionDateTime = new DateTime(2026, 10, 10, 11, 0, 0, DateTimeKind.Utc) }
            );

            builder.Entity<TestResultModel>().HasData(
                new TestResultModel { TestResultId = 2106, TestRequestTestTypeId = 1106, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: morphology review", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 14, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = null, VerifiedDateTime = null },
                new TestResultModel { TestResultId = 2107, TestRequestTestTypeId = 1107, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: morphology review", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 14, 8, 50, 0, DateTimeKind.Utc), RequiresReview = true, ReviewNotes = "DEMO: repeat examination required", VerifiedByTechnicianId = null, VerifiedDateTime = null },
                new TestResultModel { TestResultId = 2201, TestRequestTestTypeId = 1201, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 1, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 1, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2202, TestRequestTestTypeId = 1202, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 2, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 2, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2203, TestRequestTestTypeId = 1203, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 3, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 3, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2204, TestRequestTestTypeId = 1204, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 4, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 4, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2205, TestRequestTestTypeId = 1205, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 5, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 5, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2206, TestRequestTestTypeId = 1206, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 6, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 6, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2207, TestRequestTestTypeId = 1207, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 7, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 7, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2208, TestRequestTestTypeId = 1208, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 8, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 8, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2209, TestRequestTestTypeId = 1209, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 9, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 9, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2210, TestRequestTestTypeId = 1210, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 10, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 10, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2211, TestRequestTestTypeId = 1211, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 11, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 11, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2212, TestRequestTestTypeId = 1212, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 12, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 12, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2213, TestRequestTestTypeId = 1213, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 13, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 13, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultModel { TestResultId = 2214, TestRequestTestTypeId = 1214, ResultName = "Peripheral Blood Film", ResultValue = "DEMO: no abnormal cells seen", MeasurementUnit = "Qualitative", IsAbnormal = false, ResultFlag = "Not Evaluated", TechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Notes = "DEMO DATA ONLY", CapturedDateTime = new DateTime(2026, 10, 14, 8, 50, 0, DateTimeKind.Utc), RequiresReview = false, ReviewNotes = null, VerifiedByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", VerifiedDateTime = new DateTime(2026, 10, 14, 9, 15, 0, DateTimeKind.Utc) }
            );

            builder.Entity<TestResultVerificationHistoryModel>().HasData(
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3101, TestResultId = 2002, ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Action = "ReturnedForReview", Notes = "DEMO: verification attempt 1 returned", ActionDateTime = new DateTime(2026, 10, 14, 12, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3102, TestResultId = 2002, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "ResubmittedForVerification", Notes = "DEMO: corrected after attempt 1", ActionDateTime = new DateTime(2026, 10, 14, 13, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3103, TestResultId = 2002, ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Action = "ReturnedForReview", Notes = "DEMO: verification attempt 2 returned", ActionDateTime = new DateTime(2026, 10, 14, 14, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3104, TestResultId = 2002, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "ResubmittedForVerification", Notes = "DEMO: third verification now pending", ActionDateTime = new DateTime(2026, 10, 14, 15, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3111, TestResultId = 2003, ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Action = "ResubmittedForVerification", Notes = "DEMO: second verification requested", ActionDateTime = new DateTime(2026, 10, 13, 10, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3112, TestResultId = 2003, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "ReturnedForReview", Notes = "DEMO: verification attempt 2 returned", ActionDateTime = new DateTime(2026, 10, 13, 11, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3113, TestResultId = 2003, ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Action = "ResubmittedForVerification", Notes = "DEMO: third verification requested", ActionDateTime = new DateTime(2026, 10, 13, 12, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3114, TestResultId = 2003, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "ReturnedForReview", Notes = "DEMO: verification attempt 3 returned, recapture pending", ActionDateTime = new DateTime(2026, 10, 13, 13, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3121, TestResultId = 2106, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "SubmittedForVerification", Notes = "DEMO: awaiting independent verification", ActionDateTime = new DateTime(2026, 10, 14, 9, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3122, TestResultId = 2107, ActionByTechnicianId = "63950297-85b0-4b81-934a-bad2bf77f3cb", Action = "SubmittedForVerification", Notes = "DEMO: submitted for review", ActionDateTime = new DateTime(2026, 10, 14, 9, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3123, TestResultId = 2107, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "ReturnedForReview", Notes = "DEMO: correction required", ActionDateTime = new DateTime(2026, 10, 14, 10, 0, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3201, TestResultId = 2201, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 1, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3202, TestResultId = 2202, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 2, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3203, TestResultId = 2203, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 3, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3204, TestResultId = 2204, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 4, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3205, TestResultId = 2205, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 5, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3206, TestResultId = 2206, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 6, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3207, TestResultId = 2207, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 7, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3208, TestResultId = 2208, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 8, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3209, TestResultId = 2209, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 9, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3210, TestResultId = 2210, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 10, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3211, TestResultId = 2211, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 11, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3212, TestResultId = 2212, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 12, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3213, TestResultId = 2213, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 13, 9, 15, 0, DateTimeKind.Utc) },
                new TestResultVerificationHistoryModel { TestResultVerificationHistoryId = 3214, TestResultId = 2214, ActionByTechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185", Action = "Verified", Notes = "DEMO: historical verification for report", ActionDateTime = new DateTime(2026, 10, 14, 9, 15, 0, DateTimeKind.Utc) }
            );

            // ============================================================
            // ASSESSMENT: FIVE TESTS AWAITING TECHNICIAN SELECTION
            // These are unassigned, Submitted tests on requests whose samples
            // have already been received. Includes Urgent and Stat cases.
            // ============================================================
            builder.Entity<TestRequestsModel>().HasData(
                new TestRequestsModel
                {
                    TestRequestId = 11,
                    RequestNumber = "NMB-REQ-DEMO-000011",
                    PatientId = "c0fbfbb4-daf5-44e3-bf57-acfded7baceb",
                    DoctorId = "953f333d-2e68-46f6-ad26-0412fe75d908",
                    RequestDate = new DateTime(2026, 10, 15, 7, 30, 0),
                    Urgency = Urgency.Stat,
                    ClinicalNotes = "DEMO ONLY: urgent assessment workflow.",
                    Status = TestRequestStatus.SamplesReceived.ToString()
                }
            );

            builder.Entity<SampleModel>().HasData(
                new SampleModel
                {
                    SampleId = 8301,
                    TestRequestId = 11,
                    Barcode = "SMP-2026-008301",
                    SampleTypeId = 1,
                    TechnicianId = "85a94ac3-e56a-4ae2-bea1-85d398486185",
                    DateReceived = new DateTime(2026, 10, 15, 7, 45, 0),
                    Status = "SamplesReceived",
                    IsReceived = true,
                    IsDeleted = false
                }
            );

            builder.Entity<TestRequestTestTypeModel>().HasData(
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1401, TestRequestId = 2, TestTypeId = 1, TechnicianId = null, Status = "Submitted", StartDateTime = null, CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1402, TestRequestId = 2, TestTypeId = 2, TechnicianId = null, Status = "Submitted", StartDateTime = null, CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1403, TestRequestId = 6, TestTypeId = 3, TechnicianId = null, Status = "Submitted", StartDateTime = null, CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1404, TestRequestId = 11, TestTypeId = 1, TechnicianId = null, Status = "Submitted", StartDateTime = null, CompletionDateTime = null },
                new TestRequestTestTypeModel { TestRequestTestTypeId = 1405, TestRequestId = 11, TestTypeId = 2, TechnicianId = null, Status = "Submitted", StartDateTime = null, CompletionDateTime = null }
            );
        }
    }
}
