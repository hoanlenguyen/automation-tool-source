namespace IntranetApi.Models
{
    public class EmployeeCreateOrEdit : BaseAuditEntity
    {
        public string Name { get; set; }

        public string EmployeeCode { get; set; }

        public int RoleId { get; set; }
        public int DeptId { get; set; }
        public int BankId { get; set; }
        public int RankId { get; set; }
        public string BankAccountNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime BirthDate { get; set; }

        public string IdNumber { get; set; }

        public string BackendUser { get; set; }

        public string BackendPass { get; set; }

        public int? UserId { get; set; }
         
        public int Salary { get; set; }

        public string? Note { get; set; }

        public string BrandIds { get; set; }

        public string IntranetUsername { get; set; }

        public string IntranetPassword { get; set; }

        public string? Country { get; set; }
    }
}