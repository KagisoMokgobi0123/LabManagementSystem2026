using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.Technician.Models
{
    public class NotificationModel
    {
        [Key]
        public int NotificationId { get; set; }
        public DateTime IssuedDate { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public RegisterUser? Doctor { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
