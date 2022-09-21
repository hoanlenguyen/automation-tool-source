namespace IntranetApi.Models
{
    public class LeaveHistoryFilter : BaseFilterDto
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public int? BrandId { get; set; }
    }
}
