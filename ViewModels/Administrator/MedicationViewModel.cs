using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.ViewModels.Administrator
{
    public class MedicationViewModel
    {
        public int MedicationId { get; set; }

        [Required(ErrorMessage = "Medication name is required")]
        [StringLength(150)]
        public string MedicationName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int MedicationCategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
