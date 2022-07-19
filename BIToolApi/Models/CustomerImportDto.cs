namespace BITool.Models
{
    public class CustomerImportDto
    {
        public DateTime DateOccurred { get; set; }
        public string Source { get; set; }
        public long CustomerMobileNo { get; set; }
        //public List<int> ScoreIds { get; set; } = new List<int>();
        public int TotalPoints { get; set; }
    }
}