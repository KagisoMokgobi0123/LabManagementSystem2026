using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Doctor.Models;
namespace NMB_HLabSys.Areas.Doctor.ViewModels;
public sealed class TestRequestEditViewModel
{
    public int TestRequestId { get; set; }
    public string RequestNumber { get; set; } = "";
    [Required] public Urgency Urgency { get; set; } = Urgency.Routine;
    [StringLength(2000)] public string? ClinicalNotes { get; set; }
}
