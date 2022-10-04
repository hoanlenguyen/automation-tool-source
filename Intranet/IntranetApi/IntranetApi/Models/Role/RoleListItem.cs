namespace IntranetApi.Models
{
    public class RoleListItem : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime CreationTime { get; set; }
        public int? CreatorUserId { get; set; }
        public string CreatorUser { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUser { get; set; }
        public int? LastModifierUserId { get; set; }
        public List<EmployeeSimpleDto> Employees { get; set; } = new List<EmployeeSimpleDto>();
    }    
    public class RoleEmployeeList
    {
        public int RoleId { get; set; }
        public List<EmployeeSimpleDto> Employees { get; set; } = new List<EmployeeSimpleDto>();
    }

    public class EmployeeSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string  EmployeeCode  { get; set; }
    }
}
