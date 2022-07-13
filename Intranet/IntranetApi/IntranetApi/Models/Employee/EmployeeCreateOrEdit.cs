namespace IntranetApi.Models
{
    public class EmployeeCreateOrEdit : BaseAuditEntity
    {
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public int RoleId { get; set; }
        public int DeptId { get; set; }
        public string BankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public string Salary { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime Birthday { get; set; }
        public string IdNumber { get; set; }
        public string BackendUser { get; set; }
        public string BackendPass { get; set; }
        public int? UserId { get; set; }
    }
}