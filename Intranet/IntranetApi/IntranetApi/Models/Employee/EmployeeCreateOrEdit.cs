﻿namespace IntranetApi.Models
{
    public class EmployeeCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public string? EmployeeCode { get; set; }
        public bool Status { get; set; }
        public int RoleId { get; set; }
        public int DeptId { get; set; }
        public int BankId { get; set; }
        public int RankId { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? IdNumber { get; set; }
        public string? BackendUser { get; set; }
        public string? BackendPass { get; set; }

        public int Salary { get; set; }

        public string? Note { get; set; }

        public string IntranetPassword { get; set; }

        public string? Country { get; set; }
        public List<int> BrandIds { get; set; } = new List<int>();
    }
}