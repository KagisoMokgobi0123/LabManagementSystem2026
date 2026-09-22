using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;
using DoctorModel = NMB_HLabSys.Models.Users.Doctor;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class PatientConsent
    {
        [Key]
        public int PatientConsentId { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }

        [Required]
        public DateTime GrantedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Navigation properties
        public PatientModel Patient { get; set; } = null!;

        public DoctorModel Doctor { get; set; } = null!;

        public virtual ICollection<ConsentRequestItem> ConsentRequestItems
        {
            get;
            set;
        } = new List<ConsentRequestItem>();
    }
}
