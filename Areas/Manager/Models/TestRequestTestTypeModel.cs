
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestRequestTestTypeModel
    {
        [Key]
        public int TestRequestTestTypeId { get; set; }

        public int TestRequestId { get; set; }
        public TestRequestsModel? TestRequest { get; set; }

        public int TestTypeId { get; set; }
        public TestTypeModel? TestType { get; set; }

        public string? TechnicianId { get; set; }   

        public ApplicationUser? Technician { get; set; }

        public string Status { get; set; } = "Submitted";

        public DateTime? StartDateTime { get; set; }

        public DateTime? CompletionDateTime { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ResultValue { get; set; }

        [StringLength(2000)]
        public string? ResultNotes { get; set; }

        public bool IsAbnormal { get; set; }

    }
}