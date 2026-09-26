using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Models.Users
{
    [Table("Doctortbl")]
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [Display(Name = "HPCSA Number")]
        [StringLength(50)]
        public string HPCSANumber { get; set; } = string.Empty;

        [NotMapped]
        public string UserID
        {
            get => ApplicationUserId;
            set => ApplicationUserId = value;
        }

        /*
         * ------------------------------------------------------------
         * EXISTING DOCTOR RELATIONSHIPS
         * ------------------------------------------------------------
         */
         
        public virtual ICollection<TestRequestsModel> TestRequests
        {
            get;
            set;
        } = new List<TestRequestsModel>();

        public virtual ICollection<PatientConsent> PatientConsents
        {
            get;
            set;
        } = new List<PatientConsent>();

        public virtual ICollection<DoctorAlert> Alerts
        {
            get;
            set;
        } = new List<DoctorAlert>();

        public virtual ICollection<DoctorReport> Reports
        {
            get;
            set;
        } = new List<DoctorReport>();

        public virtual ICollection<ResultRelease> ResultReleases
        {
            get;
            set;
        } = new List<ResultRelease>();
    }
}
