namespace BITool.Models
{
    public class OverallReportFilter
    {
        public int TotalPointsFrom { get; set; }
        public int TotalPointsTo { get; set; }
    }

    public class OverallReportPointsCount
    {
        public int TotalPoints { get; set; }
        public int Count { get; set; }
    }

    public class OverallReportPointsCountView
    {
        public string TotalPoints { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}