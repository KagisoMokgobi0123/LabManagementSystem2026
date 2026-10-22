namespace NMB_HLabSys.Areas.Manager.Models
{

    public class DailyCategoryCount
    {
        public DateTime Date { get; set; }

        public int TestCategoryId { get; set; }

        public int TotalTests { get; set; }
    }
}
