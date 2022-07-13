namespace IntranetApi.Models
{
    public class EmployeeImportHistory: BaseEntity
    {
        public string FileName { get; set; }
        public DateTime ImportTime { get; set; } = DateTime.Now;
        public int TotalRows { get; set; }
        public int TotalErrorRows { get; set; }
        public int ImportByUserId { get; set; }
    }
}
