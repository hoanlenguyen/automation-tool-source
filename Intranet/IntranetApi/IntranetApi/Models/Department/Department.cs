using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Department : BaseAuditEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }
        public int WorkingHours { get; set; }
        public virtual ICollection<RoleDepartment> RoleDepartments { get; set; } = new HashSet<RoleDepartment>();
    }
}
