using NMB_HLabSys.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NMB_HLabSys.ViewModels.Administrator;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Patient.Models;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class Patient
    {
        [Key]
        public int PatientID { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13)]

        public string IDNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? CellPhone { get; set; }

        public string? Email { get; set; }
       

        public string? HomeAddress { get; set; }


        public string RegisterUserId { get; set; }
        public RegisterUser RegisterUser { get; set; }

        // Medical history (many-to-many)
        public ICollection<PatientCondition> PatientConditions { get; set; }
        public ICollection<PatientAllergy> PatientAllergies { get; set; }
        public ICollection<PatientMedication> PatientMedications { get; set; }

        // Consent — doctors this patient has granted access to
        public ICollection<PatientDoctorConsent> PatientDoctorConsents { get; set; }
    }
}