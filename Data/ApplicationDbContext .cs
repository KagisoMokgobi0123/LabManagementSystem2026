using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;

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

        public DbSet<TechnicianTestTypeModel> TechnicianTestTypes { get; set; }

        public DbSet<TestTypeConsumableModel> TestTypeConsumables { get; set; }


        // ============================================================
        // MANAGER - CONSUMABLES & ORDERS
        // ============================================================

        public DbSet<ConsumableModel> Consumables { get; set; }

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

        public DbSet<MedicationCategory> MedicationCategories { get; set; }

        public DbSet<Medication> Medications { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // ============================================================
            // USER PROFILE RELATIONSHIPS
            // ============================================================

            // ------------------------------------------------------------
            // APPLICATION USER -> DOCTOR PROFILE
            // ------------------------------------------------------------

            builder.Entity<DoctorProfile>()
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<DoctorProfile>(
                    x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ------------------------------------------------------------
            // APPLICATION USER -> TECHNICIAN PROFILE
            // ------------------------------------------------------------

            builder.Entity<TechnicianProfile>()
                .HasOne(x => x.User)
                .WithOne()
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
                    Status =
                        TestRequestStatus.Submitted.ToString()
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
                    TurnAroundTimeHours = 120,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 2,
                    Name = "White Blood Cell Differential",
                    TestCategoryId = 1,
                    SampleTypeId = 1,
                    MeasurementUnit = 2,
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 180,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 3,
                    Name = "Peripheral Blood Film",
                    TestCategoryId = 1,
                    SampleTypeId = 1,
                    MeasurementUnit = 3,
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 240,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 6,
                    Name = "Prothrombin Time (PT)",
                    TestCategoryId = 2,
                    SampleTypeId = 4,
                    MeasurementUnit = 5,
                    NormalMin = 11.0m,
                    NormalMax = 13.5m,
                    TurnAroundTimeHours = 120,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 7,
                    Name = "Activated Partial Thromboplastin Time (aPTT)",
                    TestCategoryId = 2,
                    SampleTypeId = 4,
                    MeasurementUnit = 5,
                    NormalMin = 25m,
                    NormalMax = 35m,
                    TurnAroundTimeHours = 120,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 8,
                    Name = "International Normalized Ratio (INR)",
                    TestCategoryId = 2,
                    SampleTypeId = 4,
                    MeasurementUnit = 6,
                    NormalMin = 0.8m,
                    NormalMax = 1.2m,
                    TurnAroundTimeHours = 120,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 11,
                    Name = "Glucose (Fasting)",
                    TestCategoryId = 3,
                    SampleTypeId = 4,
                    MeasurementUnit = 9,
                    NormalMin = 3.9m,
                    NormalMax = 5.5m,
                    TurnAroundTimeHours = 60,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 12,
                    Name = "Hemoglobin A1C (HbA1c)",
                    TestCategoryId = 3,
                    SampleTypeId = 1,
                    MeasurementUnit = 2,
                    NormalMin = 4.0m,
                    NormalMax = 5.6m,
                    TurnAroundTimeHours = 120,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 17,
                    Name = "Urinalysis (Dipstick)",
                    TestCategoryId = 4,
                    SampleTypeId = 2,
                    MeasurementUnit = 3,
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 60,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
                },

                new TestTypeModel
                {
                    TestTypeId = 20,
                    Name = "Blood Culture",
                    TestCategoryId = 5,
                    SampleTypeId = 1,
                    MeasurementUnit = 3,
                    NormalMin = 0m,
                    NormalMax = 0m,
                    TurnAroundTimeHours = 4320,
                    IsActive =
                        TestTypeModel.RecordStatus.Active
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
        }
    }
}