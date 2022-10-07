using Microsoft.AspNetCore.Authorization;

namespace IntranetApi.Models.Permission
{
    public class PermissionRequirement : IAuthorizationRequirement
    {

        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission; 
    }
}
