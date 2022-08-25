namespace IntranetApi.Models
{
    public class DepartmentCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public int WorkingHours { get; set; }
        public DateTime CreationTime { get; set; }
    }
}