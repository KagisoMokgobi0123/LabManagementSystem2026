using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestCategoryModel
    {
        // ============================================================
        // PRIMARY KEY
        // ============================================================

        [Key]
        public int TestCategoryId { get; set; }


        // ============================================================
        // CATEGORY DESCRIPTION
        //
        // Examples:
        // Hematology
        // Biochemistry
        // Microbiology
        // Immunology
        // Blood Banking
        // ============================================================

        [Required]
        [StringLength(100)]
        [Display(Name = "Test Category")]
        public string Description { get; set; } = string.Empty;


        // ============================================================
        // TEST TYPES
        //
        // One category can contain many test types.
        // ============================================================

        public virtual ICollection<TestTypeModel> TestTypes { get; set; }
            = new List<TestTypeModel>();
    }
}