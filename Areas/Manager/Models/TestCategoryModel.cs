using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestCategoryModel
    {
        public enum RecordStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Key]
        public int TestCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Test Category")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public RecordStatus IsActive { get; set; }
            = RecordStatus.Active;

        public virtual ICollection<TestTypeModel> TestTypes { get; set; }
            = new List<TestTypeModel>();
    }

}
