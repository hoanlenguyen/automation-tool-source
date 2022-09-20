namespace IntranetApi.Models
{
    public class StaffRecordFilter : BaseFilterDto
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
}
