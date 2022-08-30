namespace IntranetApi.Models
{
    public class EmployeeImportError
    {
        public string Cells { get; set; }
        public string ErrorDetails { get; set; }
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string Rank { get; set; }
        public string Dept { get; set; }
        public string Status { get; set; }
        public string Brand { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string StartDate { get; set; }
        public string Salary { get; set; }
        public string BirthDate { get; set; }
        public string? IdNumber { get; set; }
        public string? Country { get; set; }
        public string BackendUser { get; set; }
        public string BackendPass { get; set; }
        public string IntranetRole { get; set; }
        public string IntranetPassword { get; set; }
        public string? Note { get; set; }
    }
}
