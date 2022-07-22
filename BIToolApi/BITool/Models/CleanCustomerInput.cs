namespace BITool.Models
{
    public class CleanCustomerInput
    {
        public string Source { get; set; }
        public string CustomerMobileNo { get; set; }
    }

    public class CleanCustomerOutput
    {
        public string Source { get; set; }
        public long CustomerMobileNo { get; set; }
    }

    public class CleanCustomerProcess
    {
        public string Source { get; set; }
        public IList<string> MobileList { get; set; }= new List<string>();
    }     
}
