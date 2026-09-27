using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TechnicianTestTypeModel
    {
        [Key]
        public int TechnicianTestTypeId { get; set; }

        // Technician
        [Required]
        public string TechnicianId { get; set; } = string.Empty;

        [ForeignKey(nameof(TechnicianId))]
        public virtual ApplicationUser? Technician { get; set; }

        // Test Type
        [Required]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))]
        public virtual TestTypeModel? TestType { get; set; }

        // Manager who assigned the test type
        [Required]
        public string ManagerId { get; set; } = string.Empty;

        [ForeignKey(nameof(ManagerId))]
        public virtual ApplicationUser? Manager { get; set; }

        // Assignment information
        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}