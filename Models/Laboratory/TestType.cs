using System;
using System.Collections.Generic;
using NMB_HLabSys.Models.Laboratory.Enums;

namespace NMB_HLabSys.Models.Laboratory
{
    /// <summary>
    /// A laboratory test definition (see testtype.html): belongs to a Category,
    /// is performed on a SampleType, has a reporting MeasurementUnit and normal range,
    /// and a standard turn-around time (in hours).
    /// </summary>
    public class TestType
    {
        public int TestTypeId { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SampleTypeId { get; set; }
        public SampleType SampleType { get; set; }

        public string MeasurementUnit { get; set; }
        public decimal? NormalMin { get; set; }
        public decimal? NormalMax { get; set; }

        public int TurnAroundTimeHours { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public TestTypeStatus Status { get; set; } = TestTypeStatus.Active;

        public ICollection<TechnicianTestType> AssignedTechnicians { get; set; } = new List<TechnicianTestType>();
    }
}
