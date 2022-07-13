namespace BITool.Models
{
    public class CustomerDto
    {
        public int CustomerID { get; set; }
        public DateTime DateFirstAdded { get; set; } = DateTime.Now;
        public string Source { get; set; }
        public long CustomerMobileNo { get; set; }
        public int Status { get; set; } = 1;
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedON { get; set; } = DateTime.Now;
    }
}
