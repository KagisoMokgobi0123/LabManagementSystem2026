using NMB_HLabSys.Areas.Patient.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NMB_HLabSys.Models.Users;


namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class DoctorAlert
    {
     
            [Key]
            public int DoctorAlertId { get; set; }

            [ForeignKey("Doctor")]
            public int DoctorId { get; set; }
            public DoctorProfile Doctor { get; set; }

            [ForeignKey("TestRequestItem")]
            public int RequestItemId { get; set; }
            public TestRequestItem TestRequestItem { get; set; }

            [ForeignKey("TestRequests")]
            public int TestRequestId { get; set; }
            public TestRequestsModel TestRequests { get; set; }

            public string AlertMessage { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsRead { get; set; }
            public DateTime? ReadAt { get; set; }
            public string AlertType { get; set; }
        }
    }
