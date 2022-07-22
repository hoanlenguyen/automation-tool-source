namespace BITool.Models
{
    public class RecordCustomerExport
    {
        public int ID { get; set; }
        public long CustomerMobileNo { get; set; }
        public int CampaignID { get; set; }
        public DateTime DateExported { get; set; }
        public int Status { get; set; } =1;
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedON { get; set; }=DateTime.Now;
    }
}
