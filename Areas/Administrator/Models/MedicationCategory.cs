using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Patient.Models;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class MedicationCategory
    {
        public int MedicationCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public ICollection<MedicationModel> Medications { get; set; }
    }
}
