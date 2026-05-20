using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.ViewModels.Administrator
{
    public class MedicalConditionViewModel
    {
        public int MedicalConditionId { get; set; }

        [Required(ErrorMessage = "Condition name is required")]
        [StringLength(150)]
        public string ConditionName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int MedicalConditionCategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
