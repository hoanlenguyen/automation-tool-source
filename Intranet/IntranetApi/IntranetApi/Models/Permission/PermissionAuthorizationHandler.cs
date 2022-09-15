using IntranetApi.DbContext;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IntranetApi.Models.Permission
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceScopeFactory scopeFactory;

        public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                {
                    return Task.CompletedTask;
                }

                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //Console.WriteLine($"PermissionRequirement {requirement.Permission

                var userId = Convert.ToInt32(userIdClaim.Value);
                //Console.WriteLine($"userId {userId}");
                var query = from s in db.UserRoles
                            join sa in db.RoleClaims on s.RoleId equals sa.RoleId
                            where s.UserId == userId && sa.ClaimType == Permissions.Type
                            select sa.ClaimValue;
                //Console.WriteLine(query);
                var claims = query.ToList();
                if (claims.Any(p => p.Equals(requirement.Permission, StringComparison.OrdinalIgnoreCase)))
                    context.Succeed(requirement);
                else
                {
                    //var filterContext = context.Resource as AuthorizationFilterContext;
                    //var response = filterContext?.HttpContext.Response;
                    //response?.OnStarting(async () =>
                    //{
                    //    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    //    //await response.Body.WriteAsync(message, 0, message.Length); only when you want to pass a message
                    //});
                    context.Fail();
                }

                return Task.CompletedTask;
            }
        }
    }
}