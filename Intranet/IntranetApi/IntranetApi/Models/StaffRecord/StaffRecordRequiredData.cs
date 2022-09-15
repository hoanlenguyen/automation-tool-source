namespace IntranetApi.Models
{
    public class StaffRecordRequiredFilterData
    {
        public IEnumerable<int> BrandIds { get; set; } = new List<int>();
        public bool IsAllBrand { get; set; }
        public int DepartmentId { get; set; }
        public int RankId { get; set; }
        public int RankLevel { get; set; }
    }
}
