namespace IntranetApi.Models
{
    public class LeaveHistoryList
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int RankId { get; set; }
        public string Rank { get; set; }
        public string Brand { get; set; }
        public IEnumerable<string> Brands { get; set; } = new HashSet<string>();
        public IEnumerable<int> BrandEmployees { get; set; } = new HashSet<int>();
        public int SumDaysOfExtraPay { get; set; }
        public int SumHoursOfExtraPay { get; set; }
        public int SumDaysOfDeduction { get; set; }
        public int SumHoursOfDeduction { get; set; }
        public int SumDaysOfPaidOffs { get; set; }
        public int SumDaysOfPaidMCs { get; set; }
        public int LateAmount { get; set; }
        public decimal Fines { get; set; }
    }
}