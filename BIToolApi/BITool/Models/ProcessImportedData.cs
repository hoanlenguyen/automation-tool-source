namespace BITool.Models
{
    public class ProcessImportedData
    {
        public string FileName { get; set; }
        public string UserEmail { get; set; }
        public bool ShouldSendEmail { get; set; }
        public string SignalRConnectionId { get; set; }
        public List<CustomerImportDto> CustomerImports { get; set; }=new List<CustomerImportDto>();
    }
}
