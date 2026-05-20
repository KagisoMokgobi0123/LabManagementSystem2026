using NMB_HLabSys.Areas.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NMB_HLabSys.Areas.Technician.Models;

namespace NMB_HLabSys.Areas.Doctor.Models
{
    public class SampleTypeModel
    {
        [Key]
        public int SampleTypeId { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public List<SampleModel> Samples { get; set; }

        public bool IsActive { get; set; }
        public ICollection<TestTypeModel> TestTypes { get; set; } = new List<TestTypeModel>();
    }
}