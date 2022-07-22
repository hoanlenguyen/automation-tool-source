namespace BITool.Models
{
    public class CampaignCreateOrEditDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public string? Brand { get; set; }
        public string? Channel { get; set; }
        public int Amount { get; set; }
        public int PointRangeFrom { get; set; }
        public int PointRangeTo { get; set; }
        public DateTime? ExportTimeFrom { get; set; }
        public DateTime? ExportTimeTo { get; set; }
        public string? Remarks { get; set; }
    }
}
