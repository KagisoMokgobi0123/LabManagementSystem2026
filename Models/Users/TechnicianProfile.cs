using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Models.Users
{
    public class TechnicianProfile
    {

        [Key]
        public int TechnicianProfileId { get; set; }

        // APPLICATION USER

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }


        // IDENTIFICATION

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^\d{13}$",ErrorMessage = "South African ID number must contain exactly 13 digits.")]
        public string IdNumber { get; set; } = string.Empty;


        [Required]
        [StringLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;

        // TEST TYPE ASSIGNMENTS

        public virtual ICollection<TechnicianTestTypeModel>
            TestTypeAssignments
        { get; set; }
            = new List<TechnicianTestTypeModel>();
    }
}