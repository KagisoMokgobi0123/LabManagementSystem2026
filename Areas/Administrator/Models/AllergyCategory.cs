using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Administrator.Models.Enums;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class AllergyCategory
    {
        public int AllergyCategoryId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public Status Status { get; set; }

        public ICollection<Allergy> Allergies { get; set; }
            = new List<Allergy>();
    }
}
