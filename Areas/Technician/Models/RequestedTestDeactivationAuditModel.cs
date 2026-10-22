using System;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class RequestedTestDeactivationAuditModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TestRequestTestTypeId { get; set; }

        [Required]
        [StringLength(450)]
        public string TechnicianId { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public DateTime DeactivatedAt { get; set; } = DateTime.UtcNow;
    }
}