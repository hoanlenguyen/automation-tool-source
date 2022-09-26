using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class RoleDepartment
    {
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }
    }
}
