using IntranetApi.Enum;

namespace IntranetApi.Models
{
    public class StaffRecordList : BaseAuditEntity
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int RankId { get; set; }
        public string Rank { get; set; }
        public StaffRecordType RecordType { get; set; }
        public string RecordTypeName => RecordType switch
        {
            StaffRecordType.ExtraPay => "Extra pay",
            StaffRecordType.Deduction => "Deduction",
            StaffRecordType.PaidOffs => "Paid-Offs",
            StaffRecordType.PaidMCs => "Paid-MCs",
            _ =>"Extra pay"
        };
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Remarks { get; set; }
        public string? OtherDepartment { get; set; }
    }
}
