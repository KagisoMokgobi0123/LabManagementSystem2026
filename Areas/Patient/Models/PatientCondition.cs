using NMB_HLabSys.Areas.Administrator.Models;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;


namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientCondition
    {
        public int PatientConditionID { get; set; }
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }

        public int MedicalConditionID { get; set; }
        public MedicalCondition MedicalCondition { get; set; }
    }
}
