using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Validation;

namespace NMB_HLabSys.Areas.Doctor.ViewModels;

public sealed class PatientCreateViewModel
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, StringLength(100)] public string Surname { get; set; } = "";
    [Required, SouthAfricanIdNumber] public string IDNumber { get; set; } = "";
    [Required, DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }
    [Required, Phone] public string ContactNumber { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, StringLength(200)] public string Address { get; set; } = "";
    [StringLength(100)] public string? Suburb { get; set; }
    [Required, StringLength(100)] public string City { get; set; } = "";
    [Required, StringLength(100)] public string Province { get; set; } = "Eastern Cape";
    [StringLength(4, MinimumLength=4)] public string? PostalCode { get; set; }
    public List<int> MedicalConditionIds { get; set; } = new();
    public List<int> AllergyIds { get; set; } = new();
    public List<int> MedicationIds { get; set; } = new();
    public bool NoKnownMedicalConditions { get; set; }
    public bool NoKnownAllergies { get; set; }
    public bool NoCurrentMedications { get; set; }
}
