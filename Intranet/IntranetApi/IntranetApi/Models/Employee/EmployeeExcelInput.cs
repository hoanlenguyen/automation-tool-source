using IntranetApi.Enum;

namespace IntranetApi.Models
{
    public class EmployeeExcelInput : BaseAuditEntity
    {
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string Role { get; set; }
        public int? RoleId { get; set; }
        public string Dept { get; set; }
        public int? RankId { get; set; }
        public string Rank { get; set; }
        public int? DeptId { get; set; }
        public string Brand { get; set; }
        public string BrandIds { get; set; }
        public List<int>BrandIdList { get; set; }
        public string BankName { get; set; }
        public int? BankId { get; set; }
        public string BankAccountNumber { get; set; }
        public string SalaryStr { get; set; }
        public int Salary { get; set; }
        public string StartDateStr { get; set; }
        public DateTime? StartDate { get; set; }
        public string BirthDateStr { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdNumber { get; set; }
        public string BackendUser { get; set; }
        public string BackendPass { get; set; }
        public string StatusStr { get; set; }
        public string? Note { get; set; }
        public int? UserId { get; set; }
        public string LastModifierUser { get; set; }
        public string IntranetUsername { get; set; }

        public string IntranetPassword { get; set; }
    }
}
