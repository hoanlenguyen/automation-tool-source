namespace IntranetApi.Models
{
    public class EmployeeExcelInput
    {
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string Role { get; set; }
        public int? RoleId { get; set; }
        public string Dept { get; set; }
        public int? DeptId { get; set; }
        public string Brand { get; set; }
        public List<int>BrandIds { get; set; }
        public string BankName { get; set; }
        public string BankId { get; set; }
        public string BankAccountNumber { get; set; }
        public string Salary { get; set; }
        public string StartDateStr { get; set; }
        public DateTime? StartDate { get; set; }
        public string BirthDateStr { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdNumber { get; set; }
        public string BackendUser { get; set; }
        public string BackendPass { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public int? UserId { get; set; }
    }
}
