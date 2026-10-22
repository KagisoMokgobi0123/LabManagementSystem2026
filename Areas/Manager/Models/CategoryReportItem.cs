namespace NMB_HLabSys.Areas.Manager.Models
{
    public class CategoryReportItem
    {
        public int TestCategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;


        public decimal Percentage { get; set; }
        public int CompletedTests { get; set; }

        public int InProgressTests { get; set; }

        public int TotalTests => CompletedTests + InProgressTests;

        public decimal CompletionPercentage => TotalTests == 0 ? 0
                : (decimal)CompletedTests / TotalTests * 100;
    }
}