using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Account.Enums;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Models.Users;
using NMB_HLabSys.Services.Interfaces;
using NMB_HLabSys.ViewModels.Doctor;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;

namespace NMB_HLabSys.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }


        // ============================================================
        // REGISTER
        // ============================================================

        public async Task<bool> RegisterAsync(
            RegisterViewModel model)
        {
            var result =
                await RegisterWithResultAsync(model);

            return result.Succeeded;
        }


        // ============================================================
        // REGISTER WITH RESULT
        // ============================================================

        public async Task<AccountRegistrationResult>
            RegisterWithResultAsync(
                RegisterViewModel model)
        {
            if (model == null)
            {
                return AccountRegistrationResult.Failed(
                    "Registration information was not provided.");
            }

            var email =
                model.Email?.Trim();

            var firstName =
                model.FirstName?.Trim();

            var lastName =
                model.LastName?.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                return AccountRegistrationResult.Failed(
                    "First Name is required.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return AccountRegistrationResult.Failed(
                    "Last Name is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return AccountRegistrationResult.Failed(
                    "Email address is required.");
            }

            /*
             * --------------------------------------------------------
             * Validate supported role.
             * --------------------------------------------------------
             */

            if (!Enum.IsDefined(
                    typeof(UserRole),
                    model.Role))
            {
                return AccountRegistrationResult.Failed(
                    "The selected registration role is invalid.");
            }

            /*
             * --------------------------------------------------------
             * Check whether an Identity account already exists.
             * --------------------------------------------------------
             */

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return AccountRegistrationResult.Failed(
                    "An account with this email address already exists.");
            }

            /*
             * --------------------------------------------------------
             * Patient registration uses its own workflow.
             *
             * Patients have a Patient record containing their personal
             * information and do not participate in the shared staff
             * UserProfile completion workflow.
             * --------------------------------------------------------
             */

            if (model.Role == UserRole.Patient)
            {
                return await RegisterPatientAsync(
                    model,
                    email);
            }

            /*
             * --------------------------------------------------------
             * Staff registration.
             *
             * Staff members now receive a UserProfile immediately.
             *
             * The profile is intentionally PARTIAL.
             *
             * At registration time we know:
             *
             * FirstName
             * LastName
             * Email
             *
             * The remaining information is supplied later through
             * /Profile/Complete.
             * --------------------------------------------------------
             */

            return await RegisterStaffAsync(
                model,
                email);
        }


        // ============================================================
        // PATIENT REGISTRATION
        // ============================================================

        private async Task<AccountRegistrationResult>
            RegisterPatientAsync(
                RegisterViewModel model,
                string email)
        {
            var idNumber =
                model.IDNumber?.Trim();

            if (string.IsNullOrWhiteSpace(idNumber) ||
                idNumber.Length != 13)
            {
                return AccountRegistrationResult.Failed(
                    "A valid 13-digit South African ID Number is required.");
            }

            if (!model.DateOfBirth.HasValue)
            {
                return AccountRegistrationResult.Failed(
                    "Date of Birth is required for Patients.");
            }

            var cellPhone =
                model.CellPhone?.Trim();

            if (string.IsNullOrWhiteSpace(cellPhone))
            {
                return AccountRegistrationResult.Failed(
                    "Cellphone Number is required for Patients.");
            }

            var homeAddress =
                model.HomeAddress?.Trim();

            if (string.IsNullOrWhiteSpace(homeAddress))
            {
                return AccountRegistrationResult.Failed(
                    "Home Address is required for Patients.");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return AccountRegistrationResult.Failed(
                    "Password is required for Patients.");
            }

            if (string.IsNullOrWhiteSpace(
                    model.ConfirmPassword))
            {
                return AccountRegistrationResult.Failed(
                    "Password confirmation is required for Patients.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return AccountRegistrationResult.Failed(
                    "Password and password confirmation do not match.");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName) ||
                string.IsNullOrWhiteSpace(model.LastName))
            {
                return AccountRegistrationResult.Failed(
                    "First Name and Last Name are required.");
            }

            /*
             * --------------------------------------------------------
             * Check whether patient ID already exists.
             * --------------------------------------------------------
             */

            var existingPatient =
                await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.IDNumber == idNumber);

            if (existingPatient != null)
            {
                return AccountRegistrationResult.Failed(
                    "A patient with this South African ID Number already exists.");
            }

            /*
             * --------------------------------------------------------
             * Create Identity account.
             *
             * Self-registered patients are active immediately.
             *
             * Patients use the Patient record rather than the shared
             * staff UserProfile.
             * --------------------------------------------------------
             */

            var patientUser =
                new ApplicationUser
                {
                    UserName =
                        email,

                    Email =
                        email,

                    IsActive =
                        true,

                    MustChangePassword =
                        false,

                    IsProfileCompleted =
                        true,

                    EmailConfirmed =
                        true,

                    CreatedDate =
                        DateTime.UtcNow
                };

            var createResult =
                await _userManager.CreateAsync(
                    patientUser,
                    model.Password);

            if (!createResult.Succeeded)
            {
                return AccountRegistrationResult.Failed(
                    createResult.Errors
                        .Select(x => x.Description));
            }

            /*
             * --------------------------------------------------------
             * Assign Patient role.
             * --------------------------------------------------------
             */

            var roleResult =
                await _userManager.AddToRoleAsync(
                    patientUser,
                    UserRole.Patient.ToString());

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(
                    patientUser);

                return AccountRegistrationResult.Failed(
                    roleResult.Errors
                        .Select(x => x.Description));
            }

            /*
             * --------------------------------------------------------
             * Create Patient record.
             * --------------------------------------------------------
             */

            var patient =
                new PatientModel
                {
                    ApplicationUserId =
                        patientUser.Id,

                    Name =
                        model.FirstName.Trim(),

                    Surname =
                        model.LastName.Trim(),

                    IDNumber =
                        idNumber,

                    DateOfBirth =
                        model.DateOfBirth.Value,

                    CellPhone =
                        cellPhone,

                    Email =
                        email,

                    HomeAddress =
                        homeAddress,

                    RegisterUserId =
                        null
                };

            await _context.Patients.AddAsync(
                patient);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(
                    patientUser);

                return AccountRegistrationResult.Failed(
                    GetExceptionMessage(ex));
            }

            return AccountRegistrationResult.Success();
        }


        // ============================================================
        // STAFF REGISTRATION
        // ============================================================

        private async Task<AccountRegistrationResult>
            RegisterStaffAsync(
                RegisterViewModel model,
                string email)
        {
            /*
             * --------------------------------------------------------
             * Validate supported staff role.
             * --------------------------------------------------------
             */

            if (model.Role != UserRole.Administrator &&
                model.Role != UserRole.Doctor &&
                model.Role != UserRole.LabManager &&
                model.Role != UserRole.LabTechnician)
            {
                return AccountRegistrationResult.Failed(
                    "The selected staff role is not supported.");
            }

            /*
             * --------------------------------------------------------
             * Validate common registration information.
             * --------------------------------------------------------
             */

            var firstName =
                model.FirstName?.Trim();

            var lastName =
                model.LastName?.Trim();

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                return AccountRegistrationResult.Failed(
                    "First Name and Last Name are required.");
            }

            /*
             * --------------------------------------------------------
             * Doctor-specific validation.
             * --------------------------------------------------------
             */

            if (model.Role == UserRole.Doctor &&
                string.IsNullOrWhiteSpace(
                    model.HPCSANumber))
            {
                return AccountRegistrationResult.Failed(
                    "HPCSA Number is required for Doctors.");
            }

            /*
             * --------------------------------------------------------
             * Lab Technician-specific validation.
             * --------------------------------------------------------
             */

            if (model.Role == UserRole.LabTechnician &&
                string.IsNullOrWhiteSpace(
                    model.EmployeeNumber))
            {
                return AccountRegistrationResult.Failed(
                    "Employee Number is required for Lab Technicians.");
            }

            /*
             * --------------------------------------------------------
             * Generate temporary password.
             * --------------------------------------------------------
             */

            var temporaryPassword =
                GenerateTemporaryPassword();

            /*
             * --------------------------------------------------------
             * Create Identity account.
             * --------------------------------------------------------
             */

            var user =
                new ApplicationUser
                {
                    UserName =
                        email,

                    Email =
                        email,

                    IsActive =
                        false,

                    MustChangePassword =
                        true,

                    /*
                     * The profile is only partially populated at
                     * registration time.
                     *
                     * Therefore this MUST remain false.
                     */
                    IsProfileCompleted =
                        false,

                    EmailConfirmed =
                        true,

                    CreatedDate =
                        DateTime.UtcNow
                };

            var createResult =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);

            if (!createResult.Succeeded)
            {
                return AccountRegistrationResult.Failed(
                    createResult.Errors
                        .Select(x => x.Description));
            }

            /*
             * --------------------------------------------------------
             * Assign selected role.
             * --------------------------------------------------------
             */

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    model.Role.ToString());

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(
                    user);

                return AccountRegistrationResult.Failed(
                    roleResult.Errors
                        .Select(x => x.Description));
            }

            /*
             * --------------------------------------------------------
             * Create the shared UserProfile immediately.
             *
             * IMPORTANT:
             *
             * The profile is intentionally incomplete.
             *
             * At this stage we only know:
             *
             * FirstName
             * LastName
             * Email
             *
             * The following fields remain at their default values and
             * will be supplied through ProfileService:
             *
             * Title
             * Gender
             * IDNumber
             * DateOfBirth
             * Ethnicity
             * Country
             * Province
             * City
             * Address
             * PostalCode
             * ProfileImage
             * --------------------------------------------------------
             */

            var userProfile =
                new UserProfile
                {
                    ApplicationUserId =
                        user.Id,

                    FirstName =
                        firstName,

                    LastName =
                        lastName,

                    CreatedDate =
                        DateTime.UtcNow,

                    UpdatedDate =
                        null
                };


            await _context.UserProfiles.AddAsync(
                userProfile);

            /*
             * --------------------------------------------------------
             * Create role-specific profile.
             * --------------------------------------------------------
             */

            switch (model.Role)
            {
                case UserRole.Doctor:
                    {
                        var doctor =
                            new Doctor
                            {
                                ApplicationUserId =
                                    user.Id,

                                HPCSANumber =
                                    model.HPCSANumber!.Trim()
                            };

                        await _context.Doctors.AddAsync(
                            doctor);

                        break;
                    }

                case UserRole.LabTechnician:
                    {
                        var technicianProfile =
                            new TechnicianProfile
                            {
                                ApplicationUserId =
                                    user.Id,

                                EmployeeNumber =
                                    model.EmployeeNumber!.Trim()
                            };

                        await _context.TechnicianProfiles.AddAsync(
                            technicianProfile);

                        break;
                    }

                case UserRole.LabManager:
                    {
                        var managerProfile =
                            new ManagerProfile
                            {
                                ApplicationUserId =
                                    user.Id
                            };

                        await _context.ManagerProfiles.AddAsync(
                            managerProfile);

                        break;
                    }

                case UserRole.Administrator:
                    {
                        var administratorProfile =
                            new AdministratorProfile
                            {
                                ApplicationUserId =
                                    user.Id
                            };

                        await _context.AdministratorProfiles.AddAsync(
                            administratorProfile);

                        break;
                    }

                default:
                    await _userManager.DeleteAsync(
                        user);

                    return AccountRegistrationResult.Failed(
                        "The selected staff role is not supported.");
            }

            /*
             * --------------------------------------------------------
             * Save:
             *
             * 1. UserProfile
             * 2. Role-specific profile
             *
             * The Identity user was already created above.
             * --------------------------------------------------------
             */

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                /*
                 * Remove the Identity account if the profile or
                 * role-specific profile could not be saved.
                 */

                await _userManager.DeleteAsync(
                    user);

                return AccountRegistrationResult.Failed(
                    GetExceptionMessage(ex));
            }

            /*
             * --------------------------------------------------------
             * Send temporary-password email.
             * --------------------------------------------------------
             */

            var request =
                _httpContextAccessor
                    .HttpContext?
                    .Request;

            var activationLink =
                request == null
                    ? string.Empty
                    : $"{request.Scheme}://{request.Host}" +
                      $"/Account/Activate" +
                      $"?email={Uri.EscapeDataString(email)}";

            try
            {
                await _emailService
                    .SendTemporaryPasswordEmailAsync(
                        email,
                        firstName,
                        temporaryPassword,
                        activationLink);
            }
            catch
            {
                /*
                 * Account and profile creation have already succeeded.
                 *
                 * Do not delete the account if the email service fails.
                 */
            }

            return AccountRegistrationResult.Success();
        }


        // ============================================================
        // REGISTER PATIENT BY DOCTOR
        // ============================================================

        public async Task<DoctorPatientRegistrationResult>
            RegisterPatientByDoctorAsync(
                CreatePatientViewModel model,
                string doctorUserId)
        {
            if (model == null)
            {
                return DoctorPatientRegistrationResult.Failed(
                    "Invalid patient information.");
            }

            if (string.IsNullOrWhiteSpace(doctorUserId))
            {
                return DoctorPatientRegistrationResult.Failed(
                    "The logged-in doctor could not be identified.");
            }

            var email =
                model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                return DoctorPatientRegistrationResult.Failed(
                    "Patient email address is required.");
            }

            var idNumber =
                model.IDNumber?.Trim();

            if (string.IsNullOrWhiteSpace(idNumber) ||
                idNumber.Length != 13)
            {
                return DoctorPatientRegistrationResult.Failed(
                    "A valid 13-digit ID number is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Surname))
            {
                return DoctorPatientRegistrationResult.Failed(
                    "Patient name and surname are required.");
            }

            if (string.IsNullOrWhiteSpace(model.CellPhone))
            {
                return DoctorPatientRegistrationResult.Failed(
                    "Patient cellphone number is required.");
            }

            if (string.IsNullOrWhiteSpace(model.HomeAddress))
            {
                return DoctorPatientRegistrationResult.Failed(
                    "Patient home address is required.");
            }

            /*
             * --------------------------------------------------------
             * Check existing patient.
             * --------------------------------------------------------
             */

            var existingPatient =
                await _context.Patients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.IDNumber == idNumber);

            if (existingPatient != null)
            {
                return DoctorPatientRegistrationResult.Failed(
                    "A patient with this ID number already exists.");
            }

            /*
             * --------------------------------------------------------
             * Check existing Identity account.
             * --------------------------------------------------------
             */

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return DoctorPatientRegistrationResult.Failed(
                    "An account with this email address already exists.");
            }

            /*
             * --------------------------------------------------------
             * Generate temporary password.
             * --------------------------------------------------------
             */

            var temporaryPassword =
                GenerateTemporaryPassword();

            /*
             * --------------------------------------------------------
             * Create Identity account.
             *
             * Doctor-created patients continue to use the separate
             * Patient workflow and therefore do not use UserProfile.
             * --------------------------------------------------------
             */

            var user =
                new ApplicationUser
                {
                    UserName =
                        email,

                    Email =
                        email,

                    EmailConfirmed =
                        true,

                    IsActive =
                        false,

                    MustChangePassword =
                        true,

                    IsProfileCompleted =
                        true,

                    CreatedDate =
                        DateTime.UtcNow
                };

            var createResult =
                await _userManager.CreateAsync(
                    user,
                    temporaryPassword);

            if (!createResult.Succeeded)
            {
                var errors =
                    createResult.Errors
                        .Select(e => e.Description);

                return DoctorPatientRegistrationResult.Failed(
                    string.Join(" ", errors));
            }

            /*
             * --------------------------------------------------------
             * Assign Patient role.
             * --------------------------------------------------------
             */

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    UserRole.Patient.ToString());

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(
                    user);

                return DoctorPatientRegistrationResult.Failed(
                    "The Patient role could not be assigned.");
            }

            /*
             * --------------------------------------------------------
             * Create Patient record.
             * --------------------------------------------------------
             */

            var patient =
                new PatientModel
                {
                    ApplicationUserId =
                        user.Id,

                    Name =
                        model.Name.Trim(),

                    Surname =
                        model.Surname.Trim(),

                    IDNumber =
                        idNumber,

                    DateOfBirth =
                        model.DateOfBirth ??
                        DateTime.Now,

                    CellPhone =
                        model.CellPhone.Trim(),

                    Email =
                        email,

                    HomeAddress =
                        model.HomeAddress.Trim(),

                    RegisterUserId =
                        doctorUserId
                };

            await _context.Patients.AddAsync(
                patient);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(
                    user);

                return DoctorPatientRegistrationResult.Failed(
                    GetExceptionMessage(ex));
            }

            /*
             * --------------------------------------------------------
             * Add medical history.
             * --------------------------------------------------------
             */

            if (model.SelectedConditionIds != null)
            {
                foreach (var conditionId
                    in model.SelectedConditionIds)
                {
                    await _context.PatientConditions.AddAsync(
                        new PatientCondition
                        {
                            PatientID =
                                patient.PatientID,

                            MedicalConditionID =
                                conditionId
                        });
                }
            }

            if (model.SelectedAllergyIds != null)
            {
                foreach (var allergyId
                    in model.SelectedAllergyIds)
                {
                    await _context.PatientAllergies.AddAsync(
                        new PatientAllergy
                        {
                            PatientID =
                                patient.PatientID,

                            AllergyID =
                                allergyId
                        });
                }
            }

            if (model.SelectedMedicationIds != null)
            {
                foreach (var medicationId
                    in model.SelectedMedicationIds)
                {
                    await _context.PatientMedications.AddAsync(
                        new PatientMedication
                        {
                            PatientID =
                                patient.PatientID,

                            MedicationId =
                                medicationId
                        });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _context.Patients.Remove(
                    patient);

                await _context.SaveChangesAsync();

                await _userManager.DeleteAsync(
                    user);

                return DoctorPatientRegistrationResult.Failed(
                    GetExceptionMessage(ex));
            }

            return DoctorPatientRegistrationResult.Success(
                patient,
                user,
                temporaryPassword);
        }


        // ============================================================
        // ACTIVATE ACCOUNT
        // ============================================================

        public async Task<bool> ActivateAccountAsync(
            ActivateAccountViewModel model)
        {
            if (model == null)
            {
                return false;
            }

            var email =
                model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var user =
                await _userManager.FindByEmailAsync(
                    email);

            if (user == null)
            {
                return false;
            }

            /*
             * Already-active accounts do not need activation.
             */

            if (user.IsActive)
            {
                return false;
            }

            /*
             * Staff and doctor-created patients use the temporary
             * password activation process.
             */

            if (user.MustChangePassword)
            {
                if (string.IsNullOrWhiteSpace(
                    model.TemporaryPassword))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(
                    model.NewPassword))
                {
                    return false;
                }

                var temporaryPasswordValid =
                    await _userManager.CheckPasswordAsync(
                        user,
                        model.TemporaryPassword);

                if (!temporaryPasswordValid)
                {
                    return false;
                }

                var resetToken =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(
                            user);

                var passwordResult =
                    await _userManager.ResetPasswordAsync(
                        user,
                        resetToken,
                        model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    return false;
                }

                user.MustChangePassword =
                    false;
            }

            /*
             * Account becomes active only after successful
             * password activation.
             */

            user.IsActive =
                true;

            var updateResult =
                await _userManager.UpdateAsync(
                    user);

            if (!updateResult.Succeeded)
            {
                return false;
            }

            return true;
        }


        // ============================================================
        // LOGIN
        // ============================================================

        public async Task<bool> LoginAsync(
            LoginViewModel model)
        {
            if (model == null)
            {
                return false;
            }

            var email =
                model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var user =
                await _userManager.FindByEmailAsync(
                    email);

            if (user == null ||
                !user.IsActive)
            {
                return false;
            }

            /*
             * Staff and doctor-created patients must complete
             * activation before login.
             */

            if (user.MustChangePassword)
            {
                return false;
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return false;
            }

            user.LastLoginDate =
                DateTime.UtcNow;

            await _userManager.UpdateAsync(
                user);

            return true;
        }


        // ============================================================
        // LOGOUT
        // ============================================================

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        // ============================================================
        // PROFILE COMPLETION
        // ============================================================

        public async Task<bool>
            RequiresProfileCompletionAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var user =
                await _userManager.FindByIdAsync(
                    userId);

            if (user == null)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * Patients use their own workflow.
             * --------------------------------------------------------
             */

            var isPatient =
                await _userManager.IsInRoleAsync(
                    user,
                    UserRole.Patient.ToString());

            if (isPatient)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * Staff use the shared UserProfile.
             *
             * Registration now creates the UserProfile immediately.
             *
             * Therefore a newly registered staff member will normally
             * have:
             *
             * IsProfileCompleted = false
             *
             * and:
             *
             * UserProfile exists
             *
             * This method correctly returns true because the profile
             * has not yet been completed.
             * --------------------------------------------------------
             */

            if (!user.IsProfileCompleted)
            {
                return true;
            }

            /*
             * --------------------------------------------------------
             * Protect against an inconsistent database state where
             * IsProfileCompleted is true but the UserProfile row does
             * not exist.
             * --------------------------------------------------------
             */

            var profileExists =
                await _context.UserProfiles
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ApplicationUserId == userId);

            return !profileExists;
        }
        // ============================================================
        // ACCOUNT / PROFILE EDIT
        // ============================================================

        public async Task<bool> UpdateAccountProfileAsync(
            string userId,
            EditProfileViewModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                model == null)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * Find Identity user.
             * --------------------------------------------------------
             */

            var user =
                await _userManager.FindByIdAsync(
                    userId);

            if (user == null)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * Patients use their own profile workflow.
             * --------------------------------------------------------
             */

            var isPatient =
                await _userManager.IsInRoleAsync(
                    user,
                    UserRole.Patient.ToString());

            if (isPatient)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * Find shared UserProfile.
             *
             * Staff profiles should already exist because the
             * registration workflow creates them immediately.
             * --------------------------------------------------------
             */

            var profile =
                await _context.UserProfiles
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationUserId == userId);

            /*
             * --------------------------------------------------------
             * Protect against an inconsistent database state.
             *
             * If the profile does not exist, create it.
             * --------------------------------------------------------
             */

            if (profile == null)
            {
                profile =
                    new UserProfile
                    {
                        ApplicationUserId =
                            userId,

                        FirstName =
                            model.FirstName.Trim(),

                        LastName =
                            model.LastName.Trim(),

                        CreatedDate =
                            DateTime.UtcNow
                    };

                await _context.UserProfiles.AddAsync(
                    profile);
            }
            else
            {
                /*
                 * ----------------------------------------------------
                 * Update existing profile.
                 * ----------------------------------------------------
                 */

                profile.FirstName =
                    model.FirstName.Trim();

                profile.LastName =
                    model.LastName.Trim();
            }

            /*
             * --------------------------------------------------------
             * Update personal information.
             * --------------------------------------------------------
             */

            profile.Title =
                model.Title;

            profile.Gender =
                model.Gender;

            profile.IDNumber =
                model.IDNumber?.Trim();

            profile.DateOfBirth =
                model.DateOfBirth;

            profile.Ethnicity =
                model.Ethnicity;

            /*
             * --------------------------------------------------------
             * Update address information.
             * --------------------------------------------------------
             */

            profile.Country =
                model.Country?.Trim();

            profile.ProvinceId =
                model.ProvinceId;

            profile.CityId =
                model.CityId;

            profile.Address1 =
                model.Address1?.Trim();

            profile.Address2 =
                string.IsNullOrWhiteSpace(
                    model.Address2)
                    ? null
                    : model.Address2.Trim();

            profile.PostalCode =
                model.PostalCode?.Trim();

            /*
             * --------------------------------------------------------
             * Profile image.
             *
             * UserProfile stores the image PATH, not the IFormFile.
             *
             * Save the uploaded file and store its relative path.
             * --------------------------------------------------------
             */

            if (model.ProfileImage != null &&
                model.ProfileImage.Length > 0)
            {
                var webRootPath =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot");

                var profileImagesPath =
                    Path.Combine(
                        webRootPath,
                        "uploads",
                        "profiles");

                Directory.CreateDirectory(
                    profileImagesPath);

                /*
                 * Generate a unique filename so users cannot
                 * accidentally overwrite another profile image.
                 */

                var extension =
                    Path.GetExtension(
                        model.ProfileImage.FileName);

                var fileName =
                    $"{Guid.NewGuid():N}{extension}";

                var filePath =
                    Path.Combine(
                        profileImagesPath,
                        fileName);

                using (var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create))
                {
                    await model.ProfileImage
                        .CopyToAsync(stream);
                }

                /*
                 * Store the web-accessible relative path.
                 */

                profile.ProfileImagePath =
                    $"/uploads/profiles/{fileName}";
            }

            /*
             * --------------------------------------------------------
             * Update profile timestamp.
             * --------------------------------------------------------
             */

            profile.UpdatedDate =
                DateTime.UtcNow;

            /*
             * --------------------------------------------------------
             * Password change.
             *
             * Password changing is optional.
             *
             * If a new password is supplied, the current password
             * must also be supplied and must be valid.
             * --------------------------------------------------------
             */

            if (!string.IsNullOrWhiteSpace(
                model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(
                    model.CurrentPassword))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(
                    model.ConfirmNewPassword))
                {
                    return false;
                }

                if (model.NewPassword !=
                    model.ConfirmNewPassword)
                {
                    return false;
                }

                var currentPasswordValid =
                    await _userManager.CheckPasswordAsync(
                        user,
                        model.CurrentPassword);

                if (!currentPasswordValid)
                {
                    return false;
                }

                var passwordToken =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(
                            user);

                var passwordResult =
                    await _userManager.ResetPasswordAsync(
                        user,
                        passwordToken,
                        model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    return false;
                }
            }

            /*
             * --------------------------------------------------------
             * Mark the shared profile as completed.
             * --------------------------------------------------------
             */

            user.IsProfileCompleted =
                true;

            /*
             * --------------------------------------------------------
             * Update Identity user.
             * --------------------------------------------------------
             */

            var userResult =
                await _userManager.UpdateAsync(
                    user);

            if (!userResult.Succeeded)
            {
                return false;
            }

            /*
             * --------------------------------------------------------
             * Save UserProfile.
             * --------------------------------------------------------
             */

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }


        // ============================================================
        // TEMPORARY PASSWORD
        // ============================================================

        private static string GenerateTemporaryPassword()
        {
            const string upper =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            const string lower =
                "abcdefghijklmnopqrstuvwxyz";

            const string digits =
                "0123456789";

            const string special =
                "!@#$%^&*";

            var rng =
                new Random();

            var password =
                new[]
                {
                    upper[rng.Next(upper.Length)].ToString(),

                    digits[rng.Next(digits.Length)].ToString(),

                    special[rng.Next(special.Length)].ToString(),

                    lower[rng.Next(lower.Length)].ToString(),

                    lower[rng.Next(lower.Length)].ToString(),

                    lower[rng.Next(lower.Length)].ToString(),

                    upper[rng.Next(upper.Length)].ToString(),

                    digits[rng.Next(digits.Length)].ToString()
                };

            return string.Concat(
                password.OrderBy(_ =>
                    rng.Next()));
        }


        // ============================================================
        // EXCEPTION MESSAGE
        // ============================================================

        private static string GetExceptionMessage(
            Exception exception)
        {
            if (exception == null)
            {
                return "Registration failed.";
            }

            var messages =
                new List<string>();

            var current =
                exception;

            while (current != null)
            {
                if (!string.IsNullOrWhiteSpace(
                    current.Message))
                {
                    messages.Add(
                        current.Message);
                }

                current =
                    current.InnerException;
            }

            var message =
                string.Join(
                    " | ",
                    messages.Distinct());

            if (string.IsNullOrWhiteSpace(
                message))
            {
                return
                    "Registration failed because of an unexpected database error.";
            }

            return message;
        }
    }


    // ================================================================
    // DOCTOR PATIENT REGISTRATION RESULT
    // ================================================================

    public class DoctorPatientRegistrationResult
    {
        public bool Succeeded { get; private set; }

        public string ErrorMessage { get; private set; } =
            string.Empty;

        public PatientModel? Patient { get; private set; }

        public ApplicationUser? User { get; private set; }

        public string? TemporaryPassword { get; private set; }


        private DoctorPatientRegistrationResult()
        {
        }


        public static DoctorPatientRegistrationResult Success(
            PatientModel patient,
            ApplicationUser user,
            string temporaryPassword)
        {
            return new DoctorPatientRegistrationResult
            {
                Succeeded = true,

                Patient = patient,

                User = user,

                TemporaryPassword =
                    temporaryPassword,

                ErrorMessage =
                    string.Empty
            };
        }


        public static DoctorPatientRegistrationResult Failed(
            string errorMessage)
        {
            return new DoctorPatientRegistrationResult
            {
                Succeeded = false,

                ErrorMessage =
                    string.IsNullOrWhiteSpace(errorMessage)
                        ? "Patient registration failed."
                        : errorMessage
            };
        }
    }
}
