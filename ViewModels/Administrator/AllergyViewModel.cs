using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.ViewModels.Administrator
{
    public class AllergyViewModel
    {
        public int AllergyId { get; set; }

        [Required(ErrorMessage = "Allergy name is required")]
        [StringLength(150)]
        public string AllergyName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int AllergyCategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
