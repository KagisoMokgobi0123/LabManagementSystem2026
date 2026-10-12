namespace NMB_HLabSys.Areas.Technician.ViewModels
{
    public class SelectPatientTestViewModel
    {
        public int RequestedTestId { get; set; }
        public string RequestReference { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TestTypeName { get; set; } = "";
        public string Priority { get; set; } = "Routine";
        public string TurnaroundDisplay { get; set; } = "Not specified";

        public string PatientIdNumber { get; set; } = "";
        public string DoctorFullName { get; set; } = "";
        public string DoctorUsername { get; set; } = "";
        public string ClinicalNotes { get; set; } = "";
        public string RequestStatus { get; set; } = "";
        public string TestStatus { get; set; } = "";

    }
}
