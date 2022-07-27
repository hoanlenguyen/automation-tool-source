namespace BITool.Models
{
    public class AdminCampaignDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Brand { get; set; }
        public string? Channel { get; set; }
        public int Amount { get; set; }
        public int PointRangeFrom { get; set; }
        public int PointRangeTo { get; set; }
        public int ExportTimesFrom { get; set; }
        public int ExportTimesTo { get; set; }
    }
}