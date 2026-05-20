using NMB_HLabSys.Areas.Administrator.Models;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;


namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientMedication
    {
        public int PatientMedicationID { get; set; }
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }

        public int MedicationId { get; set; }
        public MedicationModel Medication { get; set; }
    }
}
