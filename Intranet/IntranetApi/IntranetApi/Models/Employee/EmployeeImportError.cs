namespace IntranetApi.Models
{
    public class EmployeeImportError
    {
        public string Cells { get; set; }
        public string ErrorDetails { get; set; }
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string Role { get; set; }
        public string Dept { get; set; }
        public string Brand { get; set; }
        public string BankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime Birthday { get; set; }
        public string IdNumber { get; set; }
        public string BackendUser { get; set; }
        public string BackendPass { get; set; }
    }
}
