using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys.Areas.LabManager.Models
{
    public class TestTypeModel
    {
        [Key]
        public int TestTypeId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public DateTime TurnAroundTime { get; set; } = DateTime.Now;

        public ICollection<TestRequestsModel> TestRequests { get; set; } = new List<TestRequestsModel>();

    }
}
