using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class EmployeeBulkInsert
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string EmployeeCode { get; set; }

        public int RoleId { get; set; }
        public int DeptId { get; set; }
        public int BankId { get; set; }

        [MaxLength(20)]
        public string BankAccountNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public string IdNumber { get; set; }

        [MaxLength(20)]
        public string BackendUser { get; set; }

        [MaxLength(20)]
        public string BackendPass { get; set; }

        public int? UserId { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int Salary { get; set; }

        [MaxLength(150)]
        public string? Note { get; set; }

        [MaxLength(20)]
        public string IntranetUsername { get; set; }

        [MaxLength(20)]
        public string IntranetPassword { get; set; }

        [MaxLength(100)]
        public string BrandIds { get; set; }
    }
}
