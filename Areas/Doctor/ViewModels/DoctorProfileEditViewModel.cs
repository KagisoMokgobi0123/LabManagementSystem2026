using System.ComponentModel.DataAnnotations;
namespace NMB_HLabSys.Areas.Doctor.ViewModels;
public sealed class DoctorProfileEditViewModel
{
    [Required, StringLength(100)] public string FirstName { get; set; } = "";
    [Required, StringLength(100)] public string LastName { get; set; } = "";
    [EmailAddress] public string Email { get; set; } = "";
}
