using IntranetApi.Enum;

namespace IntranetApi.Models
{
    public class StaffRecordData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int DepartmentId { get; set; }
        public int RankId { get; set; }
        public string BrandIds { get; set; }
        public StaffRecordType RecordType { get; set; }
        public StaffRecordDetailType RecordDetailType { get; set; }
        public int LateAmount { get; set; }

        //public string Reason { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public int NumberOfHours { get; set; }
        public decimal Fine { get; set; }
        public decimal CalculationAmount { get; set; }
        //public string? Remarks { get; set; }
    }
}