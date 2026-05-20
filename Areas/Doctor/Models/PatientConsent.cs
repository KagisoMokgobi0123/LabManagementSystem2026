using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;


namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class PatientConsent
    {
        [Key]
        public int PatientConsentId { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [Required]
        public DateTime GrantedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Navigation properties
        public PatientModel Patient { get; set; }
        public Doctor Doctor { get; set; }
        public virtual ICollection<ConsentRequestItem> ConsentRequestItems { get; set; } = new List<ConsentRequestItem>();

    }
}