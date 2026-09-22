using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoctorModel = NMB_HLabSys.Models.Users.Doctor;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public enum NotifyType
    {
        EmailResults,
        EmailAppointment
    }
    public class ResultRelease
    {
        [Key]
        public int ResultReleaseId { get; set; }

        [Required]
        public int TestRequestId { get; set; }

        [ForeignKey(nameof(TestRequestId))]
        public TestRequestsModel TestRequest { get; set; }


        [Required]
        public int ReleasedByDoctorId { get; set; }

        [Required]
        public DateTime ReleasedAt { get; set; }
        public string Note { get; set; }

        [Required]
        public NotifyType NotifyType { get; set; }

        // Navigation properties
        public DoctorModel? ReleasedByDoctor { get; set; }
    }
}