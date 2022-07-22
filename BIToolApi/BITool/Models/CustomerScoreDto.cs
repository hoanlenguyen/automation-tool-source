namespace BITool.Models
{
    public class CustomerScoreDto
    {
        public int CustomerScoreID { get; set; }
        public string Source { get; set; }
        public long CustomerMobileNo { get; set; }
        public int ScoreID { get; set; }
        public DateTime DateOccurred { get; set; }
        public int Status { get; set; } = 1;
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedON { get; set; }=DateTime.Now;
    }
}