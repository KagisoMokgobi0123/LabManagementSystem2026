using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Doctor.Models;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class ConsentRequestItemModel
    {
        [Key]
        public int ConsentRequestItemId { get; set; }

        public int PatientConsentId { get; set; }

        [ForeignKey("PatientConsentId")]
        public virtual PatientDoctorConsentModel? PatientDoctorConsent { get; set; }

        public int TestRequestId { get; set; }

        [ForeignKey("TestRequestId")]
        public virtual Doctor.Models.TestRequestsModel? TestRequest { get; set; }
    }
}