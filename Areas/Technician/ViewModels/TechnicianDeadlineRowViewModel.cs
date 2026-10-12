namespace NMB_HLabSys.Areas.Technician.ViewModels
{
    public class TechnicianDeadlineRowViewModel
    {
        public string RequestReference { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TestTypeName { get; set; } = "";
        public string Status { get; set; } = "";

        // UTC timestamps
        public DateTime? StartDateTimeUtc { get; set; }

        public double? TurnaroundHours { get; set; }
    }
}
