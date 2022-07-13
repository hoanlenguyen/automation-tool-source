using Microsoft.AspNetCore.Identity;

namespace IntranetApi.Models
{
    public class UserRole : IdentityRole<int>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int? CreatorUserId { get; set; }
    }
}
