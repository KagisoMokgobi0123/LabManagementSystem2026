using Microsoft.SqlServer.Server;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Areas.Manager.Models;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Models.Account;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestTypeModel
    {

        [Key]
        public int TestTypeId { get; set; }
        public string Name { get; set; } = string.Empty;

        public int TestCategoryId { get; set; }

        public TestCategoryModel TestCategory { get; set; } = null!;

        public int SampleTypeId { get; set; }
        public SampleTypeModel SampleType { get; set; } = null!;
        public double MeasurementUnit { get; set; }
        public decimal NormalMin { get; set; }
        public decimal NormalMax { get; set; }
        public int ConsumableId { get; set; }
        public ConsumableModel Consumable { get; set; } = null!;
        public int TurnAroundTimeHours { get; set; }
        public ICollection<TestTypeConsumableModel> TestTypeConsumables { get; set; }
    = new List<TestTypeConsumableModel>();
        public List<TechnicianTestTypeModel> TechnicianTestTypes { get; set; } = new();
        public ICollection<TestRequestsModel> TestRequests { get; set; } = new List<TestRequestsModel>();

    }
}
