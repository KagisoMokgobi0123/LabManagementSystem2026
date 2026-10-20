namespace NMB_HLabSys.Areas.Manager.Models
{
    public class ChartSeriesViewModel
    {
        public string Label { get; set; } = string.Empty;

        public List<int> Data { get; set; } = new();
    }
}