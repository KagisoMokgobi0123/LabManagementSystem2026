using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Doctor.Models;

namespace NMB_HLabSys.Areas.Doctor.ViewModels;

public sealed class TestRequestCreateViewModel
{
    [Required] public string PatientIDNumber { get; set; } = "";
    [Required] public Urgency Urgency { get; set; } = Urgency.Routine;
    [StringLength(2000)] public string? ClinicalNotes { get; set; }
    [MinLength(1, ErrorMessage="Select at least one test type.")] public List<int> TestTypeIds { get; set; } = new();
    public List<SampleEntryViewModel> Samples { get; set; } = new();
}

public sealed class SampleEntryViewModel
{
    [Required] public int SampleTypeId { get; set; }
    [Required, StringLength(100), RegularExpression(@"^[A-Za-z0-9-]{3,100}$", ErrorMessage="Enter a valid barcode (3–100 letters, numbers or hyphens).")]
    public string Barcode { get; set; } = "";
}
