using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleDeactivationAuditModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SampleId { get; set; }

        [ForeignKey(nameof(SampleId))]
        public virtual SampleModel? Sample { get; set; }

        [Required]
        [StringLength(450)]
        public string TechnicianId { get; set; } = string.Empty;

        [ForeignKey(nameof(TechnicianId))]
        public virtual ApplicationUser? Technician { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public DateTime DeactivatedAt { get; set; } = DateTime.UtcNow;
    }
}
