using NMB_HLabSys.Areas.Administrator.Models;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Patient.Models
{
    public class AllergyModel
    {
        [Key]
        public int AllergyId { get; set; }

        [Required]
        [StringLength(150)]
        public string AllergyName { get; set; } = string.Empty;

        public string? Description { get; set; } 

        // FK
        public int AllergyCategoryId { get; set; }
        public AllergyCategory AllergyCategory { get; set; } = null;

        // Many-to-many with Patient
        public ICollection<PatientAllergy> PatientAllergies { get; set; } = new List<PatientAllergy>();
    }
}
