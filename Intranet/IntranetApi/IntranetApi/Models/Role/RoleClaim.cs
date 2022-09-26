using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class RoleClaim : IdentityRoleClaim<int>
    {
        [MaxLength(50)]
        public override string ClaimType { get; set; }

        [MaxLength(100)]
        public override string ClaimValue { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}