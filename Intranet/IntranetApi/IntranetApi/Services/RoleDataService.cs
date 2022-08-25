using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace IntranetApi.Services
{
    public static class RoleDataService
    {
        private static void ProcessFilterValues(ref RoleFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        private static List<BaseDropdown> GetBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Roles where IsDeleted = 0").ToList();
        }

        public static void AddRoleDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("Role/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Roles.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                var result = entity.Adapt<RoleCreateOrEdit>();
                result.Permissions =await db.RoleClaims
                                        .AsNoTracking()     
                                        .Where(p => p.RoleId == id)
                                        .Select(p => p.ClaimValue)
                                        .ToListAsync();

                return Results.Ok(result);
            })
            .RequireAuthorization(RolePermissions.View)
            ; 

            app.MapPost("Role", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] RoleManager<Role> roleManager,
            [FromBody] RoleCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid role name!");

                if (await roleManager.RoleExistsAsync(input.Name))
                    throw new Exception("Role existed!");

                input.Name = input.Name.Trim();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Role { Name = input.Name, CreatorUserId = userId, NormalizedName = input.Name.ToUpper() };
                await roleManager.CreateAsync(entity);
 
                Console.WriteLine($"RoleId {entity.Id}");
                if (input.Permissions.Any())
                {
                    Console.WriteLine($"permissions Count: {input.Permissions.Count}");
                    var roleClaims = input.Permissions.Select(p => new RoleClaim { RoleId = entity.Id, ClaimType = Permissions.Type, ClaimValue = p });
                    await db.RoleClaims.AddRangeAsync(roleClaims);
                }
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                await db.SaveChangesAsync();
                return Results.Ok(entity);
            })
            .RequireAuthorization(RolePermissions.Create)
            ;

            app.MapPut("Role", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] RoleCreateOrEdit input) =>
            {
                if(await db.Roles.AnyAsync(p=>p.Name == input.Name && p.Id != input.Id))
                    throw new Exception("Role existed!");

                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Roles.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.Name = input.Name;
                entity.NormalizedName = input.Name.ToUpper();
                entity.Status = input.Status;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                var existedRoleClaims = await db.RoleClaims.Where(p => p.RoleId == entity.Id).ToListAsync();
                if(existedRoleClaims.Any())
                    db.RoleClaims.RemoveRange(existedRoleClaims);

                if (input.Permissions.Any())
                {
                    var roleClaims = input.Permissions.Select(p => new RoleClaim { RoleId = entity.Id, ClaimType = Permissions.Type, ClaimValue = p });
                    await db.RoleClaims.AddRangeAsync(roleClaims);
                }
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(RolePermissions.Update)
            ;

            app.MapDelete("Role/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Roles.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(RolePermissions.Delete)
            ;

            app.MapPost("Role/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] RoleFilterDto input) =>
            {
                ProcessFilterValues(ref input);                 
                var query = db.Roles.AsNoTracking()
                           .Where(p => !p.IsDeleted)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword));

                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                       .Skip(input.SkipCount)
                                       .Take(input.RowsPerPage)
                                       .ProjectToType<RoleListItem>()
                                       .ToListAsync();
                if (items.Any())
                {
                    var creatorUserIds = items.Select(p => p.CreatorUserId.GetValueOrDefault()).Distinct().ToList();
                    var lastModifierUserIds = items.Select(p => p.LastModifierUserId.GetValueOrDefault()).Distinct();
                    creatorUserIds.AddRange(lastModifierUserIds);
                    var users = db.Users.AsNoTracking()
                                    .Where(p => creatorUserIds.Contains(p.Id))
                                    .Select(p => new BaseDropdown { Id = p.Id, Name = p.Email })
                                    .ToList();
                    foreach (var item in items)
                    {
                        if (item.CreatorUserId != null)
                            item.CreatorUser = users.FirstOrDefault(p => p.Id == item.CreatorUserId.Value)?.Name;

                        if (item.LastModifierUserId != null)
                            item.LastModifierUser = users.FirstOrDefault(p => p.Id == item.LastModifierUserId.Value)?.Name;

                        item.Count = await db.UserRoles.Where(p => p.RoleId == item.Id).CountAsync();
                    }
                }               
                return Results.Ok(new PagedResultDto<RoleListItem>(totalCount, items));
            })
            .RequireAuthorization(RolePermissions.View)
            ;

            app.MapGet("Role/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache) =>
            {
                //List<BaseDropdown> items = null;
                //var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                //if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out items))
                //{
                //    items = GetBaseDropdown(sqlConnectionStr);
                //    memoryCache.Set(CacheKeys.GetRolesDropdown, items, cacheOptions);
                //}
                //return Results.Ok(items);
                return Results.Ok(GetBaseDropdown(sqlConnectionStr));
            });

            app.MapPost("Role/addPermission", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromQuery] int roleId,
            [FromQuery] string module
            ) =>
            {
                var role = await db.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
                var allClaims = await roleManager.GetClaimsAsync(role);
                var allPermissions = Permissions.GeneratePermissionsForModule(module);
                foreach (var permission in allPermissions)
                {
                    if (!allClaims.Any(a => a.Type.Equals(Permissions.Type, StringComparison.OrdinalIgnoreCase) && a.Value.Equals(permission, StringComparison.OrdinalIgnoreCase)))
                    {
                        await roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));
                    }
                }
                return Results.Ok();
            });

            app.MapGet("Role/assign", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromServices] UserManager<User> userManager,
            [FromQuery] int roleId,
            [FromQuery] int userId
            ) =>
            {
                var role = await db.Roles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == roleId);
                var user = await db.Users/*.AsNoTracking()*/.FirstOrDefaultAsync(p => p.Id == userId);
                var result1 = await userManager.AddToRoleAsync(user, role.Name);

                return Results.Ok(result1);
            });

            app.MapGet("Role/claims", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromServices] UserManager<User> userManager,
            [FromQuery] string id
            ) =>
            {
                Console.WriteLine(id);
                var user = await userManager.FindByIdAsync(id);
                var roles = await userManager.GetRolesAsync(user);
                var permissions = new List<string>();
                foreach (var roleName in roles)
                {
                    var role = await roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var claims = await roleManager.GetClaimsAsync(role);
                        permissions.AddRange(claims.Select(p => p.Value).ToArray());
                    }
                }
                return Results.Ok(permissions);
            });
        }
    }
}