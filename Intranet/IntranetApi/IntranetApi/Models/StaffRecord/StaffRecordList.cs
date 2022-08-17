using IntranetApi.Enum;

namespace IntranetApi.Models
{
    public class StaffRecordList
    {
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int RankId { get; set; }
        public string Rank { get; set; }
        public StaffRecordType RecordType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Remarks { get; set; }
    }
}
