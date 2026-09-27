using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleTypeModel
    {
        // ============================================================
        // PRIMARY KEY
        // ============================================================

        [Key]
        public int SampleTypeId { get; set; }


        // ============================================================
        // SAMPLE TYPE NAME
        //
        // Examples:
        // Blood
        // Urine
        // Plasma
        // Serum
        // Bone Marrow
        // ============================================================

        [Required]
        [StringLength(100)]
        [Display(Name = "Sample Type")]
        public string Name { get; set; } = string.Empty;


        // ============================================================
        // ACTIVE STATUS
        //
        // Allows a sample type to be disabled without deleting it.
        // ============================================================

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;


        // ============================================================
        // NAVIGATION - SAMPLES
        // ============================================================

        public virtual ICollection<SampleModel> Samples { get; set; }
            = new List<SampleModel>();
    }
}