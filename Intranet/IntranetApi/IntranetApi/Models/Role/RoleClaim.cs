using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class RoleClaim : IdentityRoleClaim<int>
    {
        [MaxLength(50)]
        public override string ClaimType { get; set; }

        [MaxLength(100)]
        public override string ClaimValue { get; set; }
    }
}