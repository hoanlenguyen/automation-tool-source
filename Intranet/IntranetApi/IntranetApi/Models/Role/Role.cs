using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Role : IdentityRole<int>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int? CreatorUserId { get; set; }
        [MaxLength(100)]
        public override string Name { get; set; }
        [MaxLength(100)]
        public override string NormalizedName { get; set; }
        [MaxLength(50)]
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public DateTime? LastModificationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public bool IsSuperAddmin { get; set; } = false;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
