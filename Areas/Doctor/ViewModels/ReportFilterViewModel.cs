using System.ComponentModel.DataAnnotations;
namespace NMB_HLabSys.Areas.Doctor.ViewModels;
public sealed class ReportFilterViewModel
{
    [DataType(DataType.Date)] public DateTime? From { get; set; }
    [DataType(DataType.Date)] public DateTime? To { get; set; }
    public string? Status { get; set; }
}
