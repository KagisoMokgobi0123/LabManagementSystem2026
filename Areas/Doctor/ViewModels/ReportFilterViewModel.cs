using System.ComponentModel.DataAnnotations;
namespace NMB_HLabSys.Areas.Doctor.ViewModels;

public sealed class ReportFilterViewModel
{
    [DataType(DataType.Date)] public DateTime? From { get; set; }
    [DataType(DataType.Date)] public DateTime? To { get; set; }
    public string? Status { get; set; }
    [RegularExpression(@"^$|^[0-9]{13}$", ErrorMessage = "Enter a valid 13-digit patient ID number.")] public string? PatientIdNumber { get; set; }
    public int? TestTypeId { get; set; }
    public string? Urgency { get; set; }
}
