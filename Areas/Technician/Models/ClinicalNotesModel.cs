using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class ClinicalNotesModel
    {
        [Key]
        public int NotesId { get; set; }
        public DateTime IssuedDate { get; set; } 
        public string DoctorId { get; set; } = string.Empty;
        public RegisterUser? Doctor { get; set; }
        public string Notes {  get; set; } = string.Empty;

    }
}
