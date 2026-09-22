//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace NMB_HLabSys.Areas.Doctor.Models
//{
//    [Table("Doctortbl")]
//    public class Doct
//    {
//        [Key]
//        public int DoctorId { get; set; }

//        public string UserID { get; set; }

//        public int HPCSANumber { get; set; }

//        // [Required]
//        // [ForeignKey("User")]
//        // public int UserId { get; set; }

//        // Navigation Properties
//        //public virtual User User { get; set; }
//        public virtual ICollection<TestRequestsModel> TestRequests { get; set; }
//        public virtual ICollection<PatientConsent> PatientConsents { get; set; }
//        public virtual ICollection<DoctorAlert> Alerts { get; set; }
//        public virtual ICollection<DoctorReport> Reports { get; set; }
//        public virtual ICollection<ResultRelease> ResultReleases { get; set; }
//    }

//}
 