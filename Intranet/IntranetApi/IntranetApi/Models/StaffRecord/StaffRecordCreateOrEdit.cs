using IntranetApi.Enum;

namespace IntranetApi.Models
{
    public class StaffRecordCreateOrEdit : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public string? OtherDepartment { get; set; }
        public int RecordType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Remarks { get; set; }
        public List<string> StaffRecordDocuments { get; set; }= new List<string>();
    }
}