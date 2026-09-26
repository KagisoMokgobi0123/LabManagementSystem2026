using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Areas.Technician.Models.ViewModel;

using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Inventory;
using NMB_HLabSys.Models.Laboratory;
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
        // USER / IDENTITY
        // ============================================================

        public DbSet<Province> Provinces { get; set; } = null!;

        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<UserProfile> UserProfiles { get; set; } = null!;

        public DbSet<Doctor> Doctors { get; set; } = null!;

        public DbSet<TechnicianProfile> TechnicianProfiles
        {
            get;
            set;
        } = null!;

        public DbSet<ManagerProfile> ManagerProfiles
        {
            get;
            set;
        } = null!;

        public DbSet<AdministratorProfile> AdministratorProfiles
        {
            get;
            set;
        } = null!;

        public DbSet<PasswordResetOtp> PasswordResetOtps
        {
            get;
            set;
        } = null!;

        // ============================================================
        // LABORATORY
        // ============================================================

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<SampleType> SampleTypes { get; set; } = null!;

        public DbSet<TestType> TestTypes { get; set; } = null!;

        public DbSet<TechnicianTestType> TechnicianTestTypes
        {
            get;
            set;
        } = null!;

        // ============================================================
        // INVENTORY
        // ============================================================

        public DbSet<Consumable> Consumables
        {
            get;
            set;
        } = null!;

        public DbSet<Supplier> Suppliers
        {
            get;
            set;
        } = null!;

        public DbSet<ManageConsumable> ManageConsumables
        {
            get;
            set;
        } = null!;

        public DbSet<Order> Orders
        {
            get;
            set;
        } = null!;

        // ============================================================
        // OTHER
        // ============================================================

        public DbSet<CalendarEvent> CalendarEvents
        {
            get;
            set;
        } = null!;

        public DbSet<MedicalConditionCategory>
            MedicalConditionCategories
        {
            get;
            set;
        } = null!;

        public DbSet<ConsentRequestItem>
            ConsentRequestItems
        {
            get;
            set;
        } = null!;

        public DbSet<DoctorAlert> DoctorAlerts
        {
            get;
            set;
        } = null!;

        public DbSet<AllergyCategory> AllergyCategories
        {
            get;
            set;
        } = null!;

        public DbSet<MedicationCategory> MedicationCategories
        {
            get;
            set;
        } = null!;

        public DbSet<DoctorReport> DoctorReports
        {
            get;
            set;
        } = null!;

        public DbSet<PatientConsent> PatientConsents
        {
            get;
            set;
        } = null!;

        public DbSet<ResultRelease> ResultReleases
        {
            get;
            set;
        } = null!;

        public DbSet<ReviewHistory> Reviews
        {
            get;
            set;
        } = null!;

        public DbSet<SampleModel> Samples
        {
            get;
            set;
        } = null!;

        public DbSet<TestRequestItem> TestRequestItems
        {
            get;
            set;
        } = null!;

        public DbSet<TestRequestsModel> TestRequests
        {
            get;
            set;
        } = null!;

        public DbSet<MedicalCondition> MedicalConditions
        {
            get;
            set;
        } = null!;

        public DbSet<Patient> Patients
        {
            get;
            set;
        } = null!;

        public DbSet<PatientAllergy> PatientAllergies
        {
            get;
            set;
        } = null!;

        public DbSet<PatientCondition> PatientConditions
        {
            get;
            set;
        } = null!;

        public DbSet<PatientMedication> PatientMedications
        {
            get;
            set;
        } = null!;

        public DbSet<AllergyModel> Allergies
        {
            get;
            set;
        } = null!;

        public DbSet<MedicationModel> Medications
        {
            get;
            set;
        } = null!;

        public DbSet<PatientDoctorConsent> PatientDoctorConsents
        {
            get;
            set;
        } = null!;

        public DbSet<TestRequestTestTypeModel> TestRequestTests
        {
            get;
            set;
        } = null!;

        public DbSet<ClinicalNotesModel> ClinicalNotes
        {
            get;
            set;
        } = null!;

        public DbSet<ResultModel> Results
        {
            get;
            set;
        } = null!;

        public DbSet<TestRequestTestTypeHistoryModel>
            TestRequestTestTypeHistories
        {
            get;
            set;
        } = null!;

        public DbSet<TestTypeConsumableModel>
            TestTypeConsumables
        {
            get;
            set;
        } = null!;

        // ============================================================
        // MODEL CONFIGURATION
        // ============================================================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================================
            // SEED DATA
            // ========================================================

            DbInitializer.Seed(modelBuilder);

            // ========================================================
            // USER PROFILE
            // ========================================================

            modelBuilder.Entity<UserProfile>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserProfile>()
                .HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<UserProfile>()
                .HasIndex(x => x.IDNumber)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.UserProfile)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<UserProfile>(
                    x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================================
            // DOCTOR
            // ========================================================

            modelBuilder.Entity<Doctor>()
                .HasKey(x => x.DoctorId);

            modelBuilder.Entity<Doctor>()
                .HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.Doctor)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<Doctor>(
                    x => x.ApplicationUserId)
                .HasPrincipalKey<ApplicationUser>(
                    x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .ToTable("Doctortbl");

            // ========================================================
            // TECHNICIAN PROFILE
            // ========================================================

            modelBuilder.Entity<TechnicianProfile>()
                .HasKey(x => x.TechnicianId);

            modelBuilder.Entity<TechnicianProfile>()
                .HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.TechnicianProfile)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<TechnicianProfile>(
                    x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================================
            // MANAGER PROFILE
            // ========================================================

            modelBuilder.Entity<ManagerProfile>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ManagerProfile>()
                .HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.ManagerProfile)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<ManagerProfile>(
                    x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================================
            // ADMINISTRATOR PROFILE
            // ========================================================

            modelBuilder.Entity<AdministratorProfile>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<AdministratorProfile>()
                .HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.AdministratorProfile)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<AdministratorProfile>(
                    x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================================
            // LOCATION
            // ========================================================

            modelBuilder.Entity<Province>()
                .HasMany(x => x.Cities)
                .WithOne(x => x.Province)
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProfile>()
                .HasOne(x => x.Province)
                .WithMany()
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========================================================
            // PASSWORD RESET
            // ========================================================

            modelBuilder.Entity<PasswordResetOtp>()
                .HasOne(x => x.ApplicationUser)
                .WithMany(x => x.PasswordResetOtps)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================================
            // LABORATORY
            // ========================================================

            modelBuilder.Entity<Category>()
                .HasMany(x => x.TestTypes)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SampleType>()
                .HasMany(x => x.TestTypes)
                .WithOne(x => x.SampleType)
                .HasForeignKey(x => x.SampleTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TechnicianTestType>()
                .HasOne(x => x.Technician)
                .WithMany()
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TechnicianTestType>()
                .HasOne(x => x.TestType)
                .WithMany(x => x.AssignedTechnicians)
                .HasForeignKey(x => x.TestTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TechnicianTestType>()
                .HasIndex(x => new
                {
                    x.TechnicianId,
                    x.TestTypeId
                })
                .IsUnique();

            // ========================================================
            // INVENTORY
            // ========================================================

            modelBuilder.Entity<Consumable>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Consumables)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consumable>()
                .HasOne(x => x.Inventory)
                .WithOne(x => x.Consumable)
                .HasForeignKey<ManageConsumable>(
                    x => x.ConsumableId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ManageConsumable>()
                .HasIndex(x => x.ConsumableId)
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasMany(x => x.ManagedConsumables)
                .WithOne(x => x.Supplier)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManageConsumable>()
                .HasMany(x => x.Orders)
                .WithOne(x => x.ManageConsumable)
                .HasForeignKey(x => x.ManageConsumableId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========================================================
            // DECIMAL PRECISION
            // ========================================================

            modelBuilder.Entity<TestType>()
                .Property(x => x.NormalMin)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TestType>()
                .Property(x => x.NormalMax)
                .HasPrecision(18, 2);

            // ========================================================
            // SAMPLE
            // ========================================================

            modelBuilder.Entity<SampleModel>()
                .HasIndex(s => s.Barcode)
                .IsUnique();

            // ========================================================
            // TEST REQUEST
            // ========================================================

            modelBuilder.Entity<TestRequestsModel>()
                .Property(t => t.Urgency)
                .HasConversion<string>();

            // ========================================================
            // TEST REQUEST TEST TYPE
            // ========================================================

            modelBuilder.Entity<TestRequestTestTypeModel>(
                entity =>
                {
                    entity.HasKey(
                        e => e.TestRequestTestTypeId);

                    entity.Property(e => e.Status)
                        .HasMaxLength(50)
                        .HasDefaultValue("Submitted");

                    // ------------------------------------------------
                    // TestRequestTestType -> TestRequest
                    // ------------------------------------------------
                    //
                    // Explicitly use the TechnicianTests navigation
                    // on TestRequestsModel.
                    //
                    // No cascade delete to avoid SQL Server cascade
                    // path problems.
                    //

                    entity.HasOne(e => e.TestRequest)
                        .WithMany(r => r.TechnicianTests)
                        .HasForeignKey(e => e.TestRequestId)
                        .OnDelete(DeleteBehavior.NoAction);

                    // ------------------------------------------------
                    // TestRequestTestType -> TestType
                    // ------------------------------------------------
                    //
                    // No cascade delete.
                    //

                    entity.HasOne(e => e.TestType)
                        .WithMany()
                        .HasForeignKey(e => e.TestTypeId)
                        .OnDelete(DeleteBehavior.NoAction);

                    // ------------------------------------------------
                    // TestRequestTestType -> Technician
                    // ------------------------------------------------

                    entity.HasOne(e => e.Technician)
                        .WithMany()
                        .HasForeignKey(e => e.TechnicianId)
                        .HasPrincipalKey(e => e.Id)
                        .OnDelete(DeleteBehavior.NoAction);

                    // ------------------------------------------------
                    // TestRequestTestType -> Verify Technician
                    // ------------------------------------------------

                    entity.HasOne(e => e.VerifyTechnician)
                        .WithMany()
                        .HasForeignKey(e => e.VerifiedByTechnicianId)
                        .HasPrincipalKey(e => e.Id)
                        .OnDelete(DeleteBehavior.NoAction);
                });

            // ========================================================
            // IDENTITY PASSKEY
            // ========================================================

            modelBuilder.Ignore<
                Microsoft.AspNetCore.Identity.IdentityPasskeyData>();

            // ========================================================
            // TEST REQUEST -> DOCTOR
            // ========================================================

            modelBuilder.Entity<TestRequestsModel>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.TestRequests)
                .HasForeignKey(t => t.DoctorId)
                .HasPrincipalKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========================================================
            // TEST REQUEST -> RESULT RELEASE
            // ========================================================

            modelBuilder.Entity<TestRequestsModel>()
                .HasOne(t => t.ResultRelease)
                .WithMany()
                .HasForeignKey(t => t.ResultReleaseId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========================================================
            // PATIENT
            // ========================================================

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<TestRequestsModel>()
                .HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatientId)
                .HasPrincipalKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========================================================
            // DOCTOR ALERT
            // ========================================================

            modelBuilder.Entity<DoctorAlert>(entity =>
            {
                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Alerts)
                    .HasForeignKey(a => a.DoctorId)
                    .HasPrincipalKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.TestRequestItem)
                    .WithMany()
                    .HasForeignKey(a => a.RequestItemId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.TestRequests)
                    .WithMany()
                    .HasForeignKey(a => a.TestRequestId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ========================================================
            // DOCTOR REPORT
            // ========================================================

            modelBuilder.Entity<DoctorReport>(entity =>
            {
                entity.HasKey(x => x.DoctorReportId);

                entity.HasOne(x => x.Doctor)
                    .WithMany(d => d.Reports)
                    .HasForeignKey(x => x.DoctorId)
                    .HasPrincipalKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ========================================================
            // PATIENT CONSENT
            // ========================================================

            modelBuilder.Entity<PatientConsent>(entity =>
            {
                entity.HasKey(x => x.PatientConsentId);

                entity.HasOne(x => x.Doctor)
                    .WithMany(d => d.PatientConsents)
                    .HasForeignKey(x => x.DoctorId)
                    .HasPrincipalKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.Patient)
                    .WithMany()
                    .HasForeignKey(x => x.PatientId)
                    .HasPrincipalKey(x => x.PatientID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ========================================================
            // RESULT RELEASE
            // ========================================================

            modelBuilder.Entity<ResultRelease>(entity =>
            {
                entity.HasKey(x => x.ResultReleaseId);

                entity.HasOne(x => x.TestRequest)
                    .WithMany()
                    .HasForeignKey(x => x.TestRequestId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.ReleasedByDoctor)
                    .WithMany(d => d.ResultReleases)
                    .HasForeignKey(x => x.ReleasedByDoctorId)
                    .HasPrincipalKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(x => x.NotifyType)
                    .HasConversion<string>();
            });

            // ========================================================
            // CONSENT REQUEST ITEM
            // ========================================================

            modelBuilder.Entity<ConsentRequestItem>(entity =>
            {
                entity.HasKey(e => e.ConsentRequestItemId);

                entity.HasOne(e => e.PatientConsent)
                    .WithMany(p => p.ConsentRequestItems)
                    .HasForeignKey(e => e.PatientConsentID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TestRequest)
                    .WithMany(t => t.ConsentRequestItems)
                    .HasForeignKey(e => e.TestRequestID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ========================================================
            // PATIENT DOCTOR CONSENT
            // ========================================================

            modelBuilder.Entity<PatientDoctorConsent>(entity =>
            {
                entity.HasKey(x => x.PatientDoctorConsentID);

                entity.HasOne(x => x.Doctor)
                    .WithMany()
                    .HasForeignKey(x => x.DoctorUserId)
                    .HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.Patient)
                    .WithMany(p => p.PatientDoctorConsents)
                    .HasForeignKey(x => x.PatientID)
                    .HasPrincipalKey(p => p.PatientID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ========================================================
            // TEST REQUEST ITEM
            // ========================================================

            modelBuilder.Entity<TestRequestItem>()
                .HasOne(t => t.TestRequest)
                .WithMany(r => r.TestRequestItems)
                .HasForeignKey(t => t.TestRequestID)
                .OnDelete(DeleteBehavior.NoAction);

            // ========================================================
            // SAMPLE -> TEST REQUEST
            // ========================================================

            modelBuilder.Entity<SampleModel>()
                .HasOne(s => s.TestRequest)
                .WithMany()
                .HasForeignKey(s => s.TestRequestId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========================================================
            // TEST TYPE CONSUMABLE
            // ========================================================
            //
            // No HasData() here.
            //
            // TestTypeConsumableModel is transactional/configuration
            // data and should be created through the application rather
            // than through EF model seeding.
            //
            // Explicit relationship configuration prevents EF from
            // relying on ambiguous conventions.
            //

            modelBuilder.Entity<TestTypeConsumableModel>(entity =>
            {
                entity.HasKey(e => e.TestTypeConsumableId);

                entity.HasOne(e => e.TestType)
                    .WithMany()
                    .HasForeignKey(e => e.TestTypeId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Consumable)
                    .WithMany()
                    .HasForeignKey(e => e.ConsumableId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ========================================================
            // TEST REQUEST ITEM RESULT VALUE
            // ========================================================

            modelBuilder.Entity<TestRequestItem>()
                .Property(t => t.ResultValue)
                .HasColumnType("decimal(18,2)");
        }
    }
}
