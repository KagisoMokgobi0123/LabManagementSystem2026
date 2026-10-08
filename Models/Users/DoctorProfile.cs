using NMB_HLabSys.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Models.Users
{
    public class DoctorProfile
    {

        [Key]
        public int DoctorProfileId { get; set; }

        // APPLICATION USER

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        // PROFESSIONAL INFORMATION

        [Required]
        [StringLength(50)]
        [Display(Name = "HPCSA Number")]
        public string HPCSANumber { get; set; } = string.Empty;
    }
}