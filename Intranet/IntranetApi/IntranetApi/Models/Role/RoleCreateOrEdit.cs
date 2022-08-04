namespace IntranetApi.Models
{
    public class RoleCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime CreationTime { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
