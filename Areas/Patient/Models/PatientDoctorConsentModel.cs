using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Models.Account;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class PatientDoctorConsentModel
    {
        [Key]
        public int PatientDoctorConsentId { get; set; }

        public string PatientId { get; set; } = string.Empty;  // ← Use string to match PatientModel

        [ForeignKey("PatientId")]
        public virtual PatientModel? Patient { get; set; }

        public string? DoctorUserId { get; set; }  // FK to AspNetUsers (RegisterUser)

        [ForeignKey("DoctorUserId")]
        public virtual RegisterUser? Doctor { get; set; }

        public DateTime GrantedDate { get; set; } = DateTime.Now;  // ← ADD THIS
        public DateTime? RevokedDate { get; set; }  // ← ADD THIS
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }  // ← ADD THIS

        public virtual ICollection<ConsentRequestItemModel> ConsentRequestItems { get; set; } = new List<ConsentRequestItemModel>();
    }
}