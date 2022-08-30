namespace IntranetApi.Models
{
    public class RoleListItem : BaseEntity
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Status { get; set; }
        public DateTime CreationTime { get; set; }
        public int? CreatorUserId { get; set; }
        public string CreatorUser { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUser { get; set; }
        public int? LastModifierUserId { get; set; }
        public List<string> EmployeeNames { get; set; } = new List<string>();
    }    
    public class RoleEmployeeList
    {
        public int RoleId { get; set; }
        public List<string> EmployeeNames { get; set; } = new List<string>();
    }
}
