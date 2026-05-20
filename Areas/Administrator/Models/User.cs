using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NMB_HLabSys.Areas.Doctor.Models;
using System.ComponentModel.DataAnnotations.Schema;


namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [EmailAddress]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string EmailAddress { get; set; } // used as username per spec

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        public string UserType { get; set; } // LabManager, LabTechnician, Doctor, Patient, Admin

        public bool MustChangePassword { get; set; } = true;

        // Password reset
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public DateTime? LoginTime { get; set; }

        // Navigation Properties
        public virtual ICollection<NMB_HLabSys.Areas.Doctor.Models.Doctor> Doctors { get; set; }
        public virtual ICollection<LabTechnician> LabTechnicians { get; set; }
        public virtual ICollection<LabManager> LabManagers { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }
}
