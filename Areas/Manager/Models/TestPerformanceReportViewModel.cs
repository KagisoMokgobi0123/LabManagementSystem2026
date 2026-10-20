using System;
using System.Collections.Generic;

namespace NMB_HLabSys.Areas.Manager.Models
{
    public class TestPerformanceReportViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalCompletedTests { get; set; }

        public int CategoriesPerformed { get; set; }

        public decimal AverageTestsPerDay { get; set; }

        public int TopCategoriesLimit { get; set; } = 10;

        public List<TestCategoryReportItemViewModel> CategoryBreakdown
        {
            get;
            set;
        } = new();

        public List<TestPerformanceTrendViewModel> PerformanceTrend
        {
            get;
            set;
        } = new();
    }

    public class TestCategoryReportItemViewModel
    {
        public int TestCategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int TotalTests { get; set; }

        public decimal Percentage { get; set; }
    }

    public class TestPerformanceTrendViewModel
    {
        public DateTime Date { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int TotalTests { get; set; }
    }
}