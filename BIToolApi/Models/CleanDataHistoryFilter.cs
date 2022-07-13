namespace BITool.Models
{
    public class CleanDataHistoryFilter : BaseFilterDto
    {
        public DateTime? CleanTimeFrom { get; set; }
        public DateTime? CleanTimeTo { get; set; }
        public string? Source { get; set; }
    }
}
