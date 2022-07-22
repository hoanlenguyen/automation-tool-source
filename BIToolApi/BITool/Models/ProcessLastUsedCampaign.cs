namespace BITool.Models
{
    public class ProcessLastUsedCampaign
    {
        public string UserEmail { get; set; }
        public bool ShouldSendEmail { get; set; }
        public int CampaignID { get; set; }
        public string SignalRConnectionId { get; set; }
        public List<long> CustomerList { get; set; } = new List<long>();
    }
}
