using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoctorModel = NMB_HLabSys.Models.Users.Doctor;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class DoctorReport
    {
        [Key]
        public int DoctorReportId { get; set; }

        [Required]
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; }

        // Navigation properties
        public DoctorModel Doctor { get; set; }


    }
}