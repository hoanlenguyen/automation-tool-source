using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(300)]
        public string FullName { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
