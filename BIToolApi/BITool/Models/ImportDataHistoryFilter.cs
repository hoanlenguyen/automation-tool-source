namespace BITool.Models
{
    public class ImportDataHistoryFilter: BaseFilterDto
    {
        public DateTime? ImportTimeFrom { get; set; }
        public DateTime? ImportTimeTo { get; set; }
        public string? Source { get; set; }
    }
}
