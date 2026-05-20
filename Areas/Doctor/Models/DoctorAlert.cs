using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class DoctorAlert
    {
        [Key]
        public int DoctorAlertId { get; set; }

        [Required]
        public int DoctorId { get; set; }  // int FK to Doctortbl

        [Required]
        public int RequestItemId { get; set; }

        [Required]
        public int TestRequestId { get; set; }

        [Required]
        public DateTime AlertDate { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties — NO [ForeignKey] attributes here
        // All relationships configured explicitly in DbContext
        public Doctor Doctor { get; set; }
        public TestRequestItem TestRequestItem { get; set; }
        public TestRequestsModel TestRequests { get; set; }
    }
}