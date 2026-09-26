//using NMB_HLabSys.Models.Account.Enums;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Models.Account
{
    //public class RegisterViewModel : IValidatableObject
    //{
    //    // ============================================================
    //    // COMMON REGISTRATION INFORMATION
    //    // ============================================================

    //    [Required(ErrorMessage = "First Name is required.")]
    //    [Display(Name = "First Name")]
    //    [StringLength(
    //        100,
    //        ErrorMessage = "First Name cannot exceed 100 characters.")]
    //    public string FirstName { get; set; } = string.Empty;


    //    [Required(ErrorMessage = "Last Name is required.")]
    //    [Display(Name = "Last Name")]
    //    [StringLength(
    //        100,
    //        ErrorMessage = "Last Name cannot exceed 100 characters.")]
    //    public string LastName { get; set; } = string.Empty;


    //    [Required(ErrorMessage = "Email Address is required.")]
    //    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    //    [StringLength(
    //        256,
    //        ErrorMessage = "Email Address cannot exceed 256 characters.")]
    //    [Display(Name = "Email Address")]
    //    public string Email { get; set; } = string.Empty;


    //    [Required(ErrorMessage = "Please select a role.")]
    //    [Display(Name = "Role")]
    //    public UserRole Role { get; set; }


    //    // ============================================================
    //    // DOCTOR REGISTRATION
    //    // ============================================================

    //    [Display(Name = "HPCSA Number")]
    //    [StringLength(
    //        50,
    //        ErrorMessage = "HPCSA Number cannot exceed 50 characters.")]
    //    public string? HPCSANumber { get; set; }


    //    // ============================================================
    //    // LAB TECHNICIAN REGISTRATION
    //    // ============================================================

    //    [Display(Name = "Employee Number")]
    //    [StringLength(
    //        50,
    //        ErrorMessage = "Employee Number cannot exceed 50 characters.")]
    //    public string? EmployeeNumber { get; set; }


    //    // ============================================================
    //    // PATIENT REGISTRATION
    //    // ============================================================

    //    [Display(Name = "South African ID Number")]
    //    [StringLength(
    //        13,
    //        MinimumLength = 13,
    //        ErrorMessage = "South African ID Number must contain 13 digits.")]
    //    public string? IDNumber { get; set; }


    //    [Display(Name = "Date of Birth")]
    //    [DataType(DataType.Date)]
    //    public DateTime? DateOfBirth { get; set; }


    //    [Display(Name = "Cellphone Number")]
    //    [Phone(ErrorMessage = "Please enter a valid cellphone number.")]
    //    [StringLength(
    //        20,
    //        ErrorMessage = "Cellphone Number cannot exceed 20 characters.")]
    //    public string? CellPhone { get; set; }


    //    [Display(Name = "Home Address")]
    //    [StringLength(
    //        300,
    //        ErrorMessage = "Home Address cannot exceed 300 characters.")]
    //    public string? HomeAddress { get; set; }


    //    [Display(Name = "Password")]
    //    [DataType(DataType.Password)]
    //    [StringLength(
    //        100,
    //        MinimumLength = 8,
    //        ErrorMessage = "Password must be between 8 and 100 characters.")]
    //    public string? Password { get; set; }


    //    [Display(Name = "Confirm Password")]
    //    [DataType(DataType.Password)]
    //    [Compare(
    //        nameof(Password),
    //        ErrorMessage = "Password and confirmation password do not match.")]
    //    public string? ConfirmPassword { get; set; }


    //    // ============================================================
    //    // ROLE-SPECIFIC VALIDATION
    //    // ============================================================

    //    public IEnumerable<ValidationResult> Validate(
    //        ValidationContext validationContext)
    //    {
    //        // --------------------------------------------------------
    //        // Validate selected role
    //        // --------------------------------------------------------

    //        if (!Enum.IsDefined(
    //                typeof(UserRole),
    //                Role))
    //        {
    //            yield return new ValidationResult(
    //                "Please select a valid registration role.",
    //                new[]
    //                {
    //                    nameof(Role)
    //                });
    //        }


    //        // --------------------------------------------------------
    //        // Doctor
    //        // --------------------------------------------------------

    //        if (Role == UserRole.Doctor &&
    //            string.IsNullOrWhiteSpace(HPCSANumber))
    //        {
    //            yield return new ValidationResult(
    //                "HPCSA Number is required for Doctors.",
    //                new[]
    //                {
    //                    nameof(HPCSANumber)
    //                });
    //        }


    //        // --------------------------------------------------------
    //        // Lab Technician
    //        // --------------------------------------------------------

    //        if (Role == UserRole.LabTechnician &&
    //            string.IsNullOrWhiteSpace(EmployeeNumber))
    //        {
    //            yield return new ValidationResult(
    //                "Employee Number is required for Lab Technicians.",
    //                new[]
    //                {
    //                    nameof(EmployeeNumber)
    //                });
    //        }


    //        // --------------------------------------------------------
    //        // Patient
    //        //
    //        // Self-registered patients provide their own password.
    //        //
    //        // The Doctor -> Patient registration workflow is separate
    //        // and continues to use its own temporary-password process.
    //        // --------------------------------------------------------

    //        if (Role == UserRole.Patient)
    //        {
    //            if (string.IsNullOrWhiteSpace(IDNumber))
    //            {
    //                yield return new ValidationResult(
    //                    "South African ID Number is required for Patients.",
    //                    new[]
    //                    {
    //                        nameof(IDNumber)
    //                    });
    //            }

    //            if (DateOfBirth == null)
    //            {
    //                yield return new ValidationResult(
    //                    "Date of Birth is required for Patients.",
    //                    new[]
    //                    {
    //                        nameof(DateOfBirth)
    //                    });
    //            }

    //            if (string.IsNullOrWhiteSpace(CellPhone))
    //            {
    //                yield return new ValidationResult(
    //                    "Cellphone Number is required for Patients.",
    //                    new[]
    //                    {
    //                        nameof(CellPhone)
    //                    });
    //            }

    //            if (string.IsNullOrWhiteSpace(HomeAddress))
    //            {
    //                yield return new ValidationResult(
    //                    "Home Address is required for Patients.",
    //                    new[]
    //                    {
    //                        nameof(HomeAddress)
    //                    });
    //            }

    //            if (string.IsNullOrWhiteSpace(Password))
    //            {
    //                yield return new ValidationResult(
    //                    "Password is required for Patients.",
    //                    new[]
    //                    {
    //                        nameof(Password)
    //                    });
    //            }

    //            if (string.IsNullOrWhiteSpace(ConfirmPassword))
    //            {
    //                yield return new ValidationResult(
    //                    "Password confirmation is required for Patients.",
    //                    new[]
    //                    {
    //                        nameof(ConfirmPassword)
    //                    });
    //            }
    //        }
    //    }
    //}
}
