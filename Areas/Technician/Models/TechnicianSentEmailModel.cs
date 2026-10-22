using System;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class TechnicianSentEmailModel
    {
        [Key]
        public int SentEmailId { get; set; }

        [Required]
        public int TestRequestId { get; set; }

        // Null for a combined final report.
        // Populated for an individual preliminary report.
        public int? TestRequestTestTypeId { get; set; }

        [Required]
        [StringLength(30)]
        public string ReportType { get; set; } = "Final";

        [Required]
        [StringLength(256)]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PdfFileName { get; set; } = string.Empty;

        public byte[]? PdfAttachment { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SentAt { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }
    }
}