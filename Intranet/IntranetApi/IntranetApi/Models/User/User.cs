using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(300)]
        public string Name { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsFirstTimeLogin { get; set; } = false;
    }
}
