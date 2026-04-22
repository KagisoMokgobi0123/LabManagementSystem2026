using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class SampleTypeModel
    {
        [Key]
        public int SampleTypeId { get; set; }

        [Required(ErrorMessage ="Sample type name is required")]
        public string SampleName { get; set; } = string.Empty;
    }
}
