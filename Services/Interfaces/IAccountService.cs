using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Services.Implementations;
using NMB_HLabSys.ViewModels.Doctor;

namespace NMB_HLabSys.Services.Interfaces
{
    public interface IAccountService
    {
        // ============================================================
        // REGISTRATION
        // ============================================================

        /// <summary>
        /// Registers a new user.
        ///
        /// The registration process creates the ASP.NET Identity
        /// ApplicationUser and the shared UserProfile.
        ///
        /// Information available during registration is persisted
        /// immediately:
        ///
        /// ApplicationUser:
        /// - Email
        ///
        /// UserProfile:
        /// - ApplicationUserId
        /// - FirstName
        /// - LastName
        ///
        /// Additional UserProfile information is completed later
        /// through the Profile/Complete workflow.
        ///
        /// Patient self-registration:
        /// - Patient supplies their own password.
        /// - Patient account is active immediately.
        /// - Patient does not require account activation.
        ///
        /// Staff registration:
        /// - A temporary password is generated.
        /// - Account starts inactive.
        /// - User must activate the account before login.
        /// - Shared UserProfile is created during registration with
        ///   FirstName and LastName.
        /// - Remaining profile information is completed after login.
        /// </summary>
        Task<bool> RegisterAsync(
            RegisterViewModel model);


        /// <summary>
        /// Registers a new user and returns a detailed result.
        ///
        /// Registration includes:
        ///
        /// 1. Creating the ApplicationUser.
        /// 2. Persisting the registration email on ApplicationUser.
        /// 3. Assigning the selected role.
        /// 4. Creating the shared UserProfile.
        /// 5. Persisting FirstName and LastName to UserProfile.
        /// 6. Creating any role-specific profile required by the
        ///    selected registration workflow.
        /// 7. Generating/sending activation details where applicable.
        ///
        /// The method returns detailed errors so the controller can
        /// distinguish duplicate-email, validation, role, profile,
        /// and other registration failures.
        /// </summary>
        Task<AccountRegistrationResult>
            RegisterWithResultAsync(
                RegisterViewModel model);


        // ============================================================
        // DOCTOR -> PATIENT REGISTRATION
        // ============================================================

        /// <summary>
        /// Registers a patient from the Doctor Patient Management
        /// module.
        ///
        /// This is a separate registration workflow from normal
        /// self-registration.
        ///
        /// Account creation, Patient creation, Patient role
        /// assignment, temporary password generation, and
        /// medical-history creation are handled by AccountService.
        ///
        /// Where applicable, the shared UserProfile is also created
        /// using the personal information available during this
        /// registration workflow.
        /// </summary>
        Task<DoctorPatientRegistrationResult>
            RegisterPatientByDoctorAsync(
                CreatePatientViewModel model,
                string doctorUserId);


        // ============================================================
        // ACCOUNT ACTIVATION
        // ============================================================

        /// <summary>
        /// Activates an existing account using the temporary password
        /// and sets the user's permanent password.
        ///
        /// This applies to accounts that are created inactive and
        /// require activation, such as staff accounts and patients
        /// created through the Doctor -> Patient workflow.
        /// </summary>
        Task<bool> ActivateAccountAsync(
            ActivateAccountViewModel model);


        // ============================================================
        // LOGIN
        // ============================================================

        /// <summary>
        /// Authenticates an active user.
        ///
        /// Self-registered patients can log in directly because their
        /// accounts are activated during registration.
        ///
        /// Staff and doctor-created patients must complete the
        /// temporary-password activation process first.
        ///
        /// Staff users whose shared UserProfile is not yet completed
        /// are redirected by the controller to the profile completion
        /// workflow after successful authentication.
        /// </summary>
        Task<bool> LoginAsync(
            LoginViewModel model);


        // ============================================================
        // LOGOUT
        // ============================================================

        /// <summary>
        /// Signs the current user out.
        /// </summary>
        Task LogoutAsync();


        // ============================================================
        // PROFILE COMPLETION
        // ============================================================

        /// <summary>
        /// Determines whether the user still needs to complete their
        /// shared UserProfile.
        ///
        /// The shared UserProfile is expected to exist because it is
        /// created during registration.
        ///
        /// For staff users, profile completion is required when:
        ///
        /// - IsProfileCompleted is false, or
        /// - the UserProfile record does not exist.
        ///
        /// Patients do not use the shared staff profile-completion
        /// workflow.
        /// </summary>
        Task<bool> RequiresProfileCompletionAsync(
            string userId);


        // ============================================================
        // ACCOUNT / PROFILE EDIT
        // ============================================================

        /// <summary>
        /// Updates the authenticated user's account and shared profile.
        ///
        /// ApplicationUser information:
        /// - FirstName
        /// - LastName
        /// - Password, when a new password is supplied
        ///
        /// UserProfile information:
        /// - Title
        /// - Gender
        /// - ID Number
        /// - Date of Birth
        /// - Ethnicity
        /// - Country
        /// - Province
        /// - City
        /// - Address
        /// - Postal Code
        /// - Profile Image
        ///
        /// The existing email address is not changed by this workflow.
        /// </summary>
        Task<bool> UpdateAccountProfileAsync(
            string userId,
            EditProfileViewModel model);
    }


    // ================================================================
    // ACCOUNT REGISTRATION RESULT
    // ================================================================

    /// <summary>
    /// Provides the outcome of the normal Account registration flow.
    ///
    /// This result is separate from
    /// DoctorPatientRegistrationResult because the normal registration
    /// page and Doctor Patient Management page have different
    /// registration workflows.
    /// </summary>
    public class AccountRegistrationResult
    {
        /// <summary>
        /// Indicates whether registration completed successfully.
        /// </summary>
        public bool Succeeded { get; private set; }


        /// <summary>
        /// General error message when registration fails.
        /// </summary>
        public string ErrorMessage { get; private set; } =
            string.Empty;


        /// <summary>
        /// Identity or registration errors returned by the service.
        /// </summary>
        public string[] Errors { get; private set; } =
            Array.Empty<string>();


        /// <summary>
        /// Creates a successful registration result.
        /// </summary>
        public static AccountRegistrationResult Success()
        {
            return new AccountRegistrationResult
            {
                Succeeded = true,

                ErrorMessage =
                    string.Empty,

                Errors =
                    Array.Empty<string>()
            };
        }


        /// <summary>
        /// Creates a failed registration result with one error.
        /// </summary>
        public static AccountRegistrationResult Failed(
            string errorMessage)
        {
            var message =
                string.IsNullOrWhiteSpace(errorMessage)
                    ? "Registration failed."
                    : errorMessage;

            return new AccountRegistrationResult
            {
                Succeeded = false,

                ErrorMessage =
                    message,

                Errors =
                    new[]
                    {
                        message
                    }
            };
        }


        /// <summary>
        /// Creates a failed registration result from multiple errors.
        /// </summary>
        public static AccountRegistrationResult Failed(
            IEnumerable<string> errors)
        {
            var errorArray =
                errors?
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .ToArray()
                ?? Array.Empty<string>();

            if (errorArray.Length == 0)
            {
                errorArray =
                    new[]
                    {
                        "Registration failed."
                    };
            }

            return new AccountRegistrationResult
            {
                Succeeded = false,

                ErrorMessage =
                    string.Join(
                        " ",
                        errorArray),

                Errors =
                    errorArray
            };
        }
    }
}