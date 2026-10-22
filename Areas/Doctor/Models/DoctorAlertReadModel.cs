using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Doctor.Models;

public sealed class DoctorAlertReadModel
{
    public int DoctorAlertReadId { get; set; }

    [Required]
    public string DoctorId { get; set; } = string.Empty;

    public int TestRequestTestTypeId { get; set; }

    public DateTime ReadAtUtc { get; set; }
}