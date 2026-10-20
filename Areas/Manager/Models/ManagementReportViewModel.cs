using System;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class ManagementReportViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalCompletedTests { get; set; }

        public int TotalCategories { get; set; }

        public decimal AverageTestsPerDay { get; set; }

        public int CategoriesPerformed
        {
            get => TotalCategories;
            set => TotalCategories = value;
        }

        public int TopCategoriesLimit { get; set; } = 10;

        public List<CategoryReportItem> CategoryBreakdown { get; set; }
            = new();

        public List<string> ChartLabels { get; set; }
            = new();

        public List<ChartSeriesViewModel> ChartSeries { get; set; }
            = new();
    }

    
}


