using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Employee : BaseAuditEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string EmployeeCode { get; set; }

        public int RoleId { get; set; }
        public int DeptId { get; set; }
        public int BankId { get; set; }
        [MaxLength(20)]
        public string BankAccountNumber { get; set; }
        [MaxLength(100)]
        public string BrandIds { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public string IdNumber { get; set; }

        [MaxLength(20)]
        public string BackendUser { get; set; }

        [MaxLength(20)]
        public string BackendPass { get; set; }

        public int? UserId { get; set; }

        public int Salary { get; set; }

        [MaxLength(150)]
        public string? Note { get; set; }

    }
}