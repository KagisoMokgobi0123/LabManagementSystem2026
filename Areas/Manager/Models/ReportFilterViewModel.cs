using System;
using System.Collections.Generic;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class ReportFilterViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<ReportRow> Rows { get; set; } = new List<ReportRow>();
        public Dictionary<string, int> CategoryTotals { get; set; } = new Dictionary<string, int>();
    }

    public class ReportRow
    {
        public string ConsumableName { get; set; }
        public string CategoryName { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityDelivered { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
