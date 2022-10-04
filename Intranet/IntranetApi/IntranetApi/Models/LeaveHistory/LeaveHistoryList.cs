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
        public string Country { get; set; }
        public string CurrencySymbol { get; set; }
        public IEnumerable<string> Brands { get; set; } = new HashSet<string>();
        public IEnumerable<int> BrandEmployees { get; set; } = new HashSet<int>();
        public float SumDaysOfExtraPay { get; set; }
        public float SumHoursOfExtraPay { get; set; }
        public float SumDaysOfDeduction { get; set; }
        public float SumHoursOfDeduction { get; set; }
        public float SumDaysOfPaidOffs { get; set; }
        public float SumDaysOfPaidMCs { get; set; }
        public decimal SumCalculationAmount { get; set; }
        public float LateAmount { get; set; }
        public decimal Fines { get; set; }
    }
}