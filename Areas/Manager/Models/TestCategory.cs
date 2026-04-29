using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestCategory
    {
        [Key]
        public int TestCategoryId { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
