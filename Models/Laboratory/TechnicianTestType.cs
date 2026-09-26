using NMB_HLabSys.Models.Identity;
using System;

namespace NMB_HLabSys.Models.Laboratory
{
    public class TechnicianTestType
    {
        public int TechnicianTestTypeId { get; set; }


        public string TechnicianId { get; set; }

        public ApplicationUser Technician { get; set; }

        public string Status { get; set; }

        public int TestTypeId { get; set; }

        public TestType TestType { get; set; }


        public DateTime DateAssigned { get; set; } = DateTime.UtcNow;
    }
}