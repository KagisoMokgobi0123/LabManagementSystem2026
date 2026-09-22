using NMB_HLabSys.Areas.Administrator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Models.Inventory;
using NMB_HLabSys.Models.Laboratory;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public enum TestItemStatus
    {
        Submitted,
        InProgress,
        Completed,
        Cancelled
    }
    public class TestRequestItem
    {
        [Key]
        public int TestRequestItemId { get; set; }


        [Required]
        [ForeignKey("TestRequest")]
        public int TestRequestID { get; set; }

        [Required]
        [ForeignKey("TestType")]
        public int TestTypeID { get; set; }

        public string Notes { get; set; } = string.Empty;


        public bool IsAbnormal {  get; set; }

        [Required]
        public TestItemStatus Status { get; set; }
        public int? AssignedTechnicianId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ResultValue { get; set; }
        public string ResultNotes { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? CompletionDateTime { get; set; }

        // Navigation properties
        public TestRequestsModel TestRequest { get; set; }
       public TestType TestType { get; set; }
        public LabTechnician AssignedTechnician { get; set; }
        public List<DoctorAlert> Alerts { get; set; }

    }
}