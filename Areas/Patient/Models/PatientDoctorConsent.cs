using NMB_HLabSys.Models.Account;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;


namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientDoctorConsent
    {
        public int PatientDoctorConsentID { get; set; }

        public int PatientID { get; set; }
        public PatientModel Patient { get; set; } = null!;

        public string? DoctorUserId { get; set; }  // FK to AspNetUsers (RegisterUser)
        public RegisterUser? Doctor { get; set; }

        public bool IsActive { get; set; }
    }
}
