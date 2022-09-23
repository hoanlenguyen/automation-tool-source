using IntranetApi.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(150)]
        public string Name { get; set; }

        public UserType UserType { get; set; } = UserType.Employee;
        public bool IsDeleted { get; set; } = false;
        public bool IsFirstTimeLogin { get; set; } = true;
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public bool Status { get; set; } = true;
        //[NotMapped]

        //public override string? Email { get; set; }

        #region Employee

        [MaxLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public int RankId { get; set; }
        public int DeptId { get; set; }
        public int BankId { get; set; }

        [MaxLength(150)]
        public string BankAccountName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? BankAccountNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(20)]
        public string? IdNumber { get; set; }

        [MaxLength(20)]
        public string? BackendUser { get; set; }

        [MaxLength(20)]
        public string? BackendPass { get; set; }

        public int Salary { get; set; }

        [MaxLength(150)]
        public string? Note { get; set; }

        [MaxLength(20)]
        public string IntranetPassword { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Country { get; set; }

        public virtual ICollection<StaffRecord> StaffRecords { get; set; } = new HashSet<StaffRecord>();
        public virtual ICollection<BrandEmployee> BrandEmployees { get; set; } = new HashSet<BrandEmployee>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        //[ForeignKey(nameof(RankId))]
        //public virtual Rank Rank { get; set; }

        //[ForeignKey(nameof(DeptId))]
        //public virtual Department Department { get; set; }

        //[ForeignKey(nameof(BankId))]
        //public virtual Bank Bank { get; set; }

        //[ForeignKey(nameof(RoleId))]
        //public virtual Role Role { get; set; }

        #endregion Employee
    }
}