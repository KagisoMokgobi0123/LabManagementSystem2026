using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.ViewModels.Administrator
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public string Description { get; set; }
    }
}
