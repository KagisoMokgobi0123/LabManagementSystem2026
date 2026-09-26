using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Models.Users
{
    public class TechnicianProfile
    {
        [Key]
        public int TechnicianId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [Display(Name = "Employee Number")]
        [StringLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;
    }
}