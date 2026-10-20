namespace NMB_HLabSys.Areas.Manager.Models
{
    public class CategoryReportItem
    {
        public int TestCategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int TotalTests { get; set; }

        public decimal Percentage { get; set; }
    }
}