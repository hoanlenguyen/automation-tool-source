namespace IntranetApi.Models
{
    public class EmployeeList:BaseEntity
    {
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public int RoleId { get; set; }
        public int DeptId { get; set; }
        public int BankId { get; set; }
        public int RankId { get; set; }
        public List<int> BrandIds { get; set; } = new List<int>();
    }
}
