using System;
using System.Collections.Generic;
using NMB_HLabSys.Models.Laboratory.Enums;

namespace NMB_HLabSys.Models.Laboratory
{
    public class SampleType
    {
        public int SampleTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public SampleTypeStatus Status { get; set; } = SampleTypeStatus.Active;

        public ICollection<TestType> TestTypes { get; set; } = new List<TestType>();
    }
}
