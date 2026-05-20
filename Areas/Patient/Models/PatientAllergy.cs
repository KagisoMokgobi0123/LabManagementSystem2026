using NMB_HLabSys.Areas.Administrator.Models;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientAllergy
    {
        public int PatientAllergyID { get; set; }
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; } = null;

        public int AllergyID { get; set; }
        public AllergyModel Allergy { get; set; } = null;
    }
}
