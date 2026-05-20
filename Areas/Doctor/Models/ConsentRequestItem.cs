using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Doctor.Models;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class ConsentRequestItem
    {
        [Key]
        public int ConsentRequestItemId { get; set; }

        [Required]
        [ForeignKey("ConsentRequest")]
        public int ConsentRequestId { get; set; }

        [Required]
        [ForeignKey("TestRequest")]
        public int TestRequestID { get; set; }

        [Required]
        [ForeignKey("PatientConsent")]
        public int PatientConsentID { get; set; }


        [Required]
        public DateTime GrantedAt { get; set; }

        // Navigation properties
        public PatientConsent PatientConsent { get; set; }

        //public ConsentRequest ConsentRequest { get; set; }

        public TestRequestsModel TestRequest { get; set; }

       

    }
}