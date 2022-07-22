namespace BITool.Models
{
    public class CustomerTotalPointsImport
    {
        public long CustomerMobileNo { get; set; }
        public int TotalPoints { get; set; }
    }

    public class CustomerTotalPointsImportError
    {
        public string Cell { get; set; }
        public string ErrorDetail { get; set; }
        public string CustomerMobileNo { get; set; }
        public string TotalPoints { get; set; }
    }
}
