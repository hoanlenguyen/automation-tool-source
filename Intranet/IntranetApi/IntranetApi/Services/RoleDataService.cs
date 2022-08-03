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

        private static int GetTotalCountByFilter(string sqlConnectionStr, ref RoleFilterDto input)
        {
            using (var conn = new MySqlConnection(sqlConnectionStr))
            {
                conn.Open();
                var cmd = new MySqlCommand(StoredProcedureName.GetRoleTotal, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@keyword", input.Keyword);
                cmd.Parameters.AddWithValue("@status", input.Status);

                MySqlDataReader rdr = cmd.ExecuteReader();
                int count = 0;
                while (rdr.Read())
                    count = (int)rdr.GetInt64(0);
                rdr.Close();
                conn.Close();
                return count;
            }
        }

        private static List<BaseDropdown> GetBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from UserRole where IsDeleted = 0").ToList();
        }

        public static void AddRoleDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("Role/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Role.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            });

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
                input.Name = input.Name.Trim();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Role { Name = input.Name, CreatorUserId = userId, NormalizedName = input.Name.ToUpper() };
                await roleManager.CreateAsync(entity);

                //db.Add(entity);
                //db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                return Results.Ok(entity);
            });

            app.MapPut("Role", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] RoleCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Role.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.Name = input.Name;
                entity.Status = input.Status;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapDelete("Role/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Role.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                return Results.Ok();
            });

            app.MapPost("Role/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] RoleFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Role.AsNoTracking()
                           .Where(p => !p.IsDeleted)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                query = query.OrderByDynamic(input.SortBy, input.SortDirection);
                var data = await query.Skip(input.SkipCount).Take(input.RowsPerPage).ToListAsync();
                var items = data.Adapt<List<RoleCreateOrEdit>>();                 
                return Results.Ok(new PagedResultDto<RoleCreateOrEdit>(totalCount, items));
            });

            app.MapGet("Role/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache) =>
            {
                List<BaseDropdown> items = null;
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out items))
                {
                    items = GetBaseDropdown(sqlConnectionStr);
                    memoryCache.Set(CacheKeys.GetRolesDropdown, items, cacheOptions);
                }
                return Results.Ok(items);
            });

            app.MapPost("Role/addPermission", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromQuery] int roleId,
            [FromQuery] string module
            ) =>
            {
                var role = await db.Roles.FirstOrDefaultAsync(p=>p.Id==roleId);
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

            app.MapGet("Role/assign", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromServices] UserManager<User> userManager,
            [FromQuery] int roleId,
            [FromQuery] int userId
            ) =>
            {
                var role = await db.Role.AsNoTracking().FirstOrDefaultAsync(p => p.Id == roleId);
                var user = await db.User/*.AsNoTracking()*/.FirstOrDefaultAsync(p => p.Id == userId);
                var result1 = await userManager.AddToRoleAsync(user, role.Name);

                return Results.Ok(result1);
            });

            app.MapGet("Role/claims", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromServices] UserManager<User> userManager,
            [FromQuery] string id
            ) =>
            {
                Console.WriteLine(id);
                var user= await userManager.FindByIdAsync(id);
                var roles = await userManager.GetRolesAsync(user);
                //var role =await roleManager.FindByIdAsync(id);
                //var claims = await roleManager.GetClaimsAsync(role);
                var permissions = new List<string>();
                foreach (var roleName in roles)
                {
                    var role = await roleManager.FindByNameAsync(roleName);
                    if(role != null)
                    {
                        var claims = await roleManager.GetClaimsAsync(role);
                        permissions.AddRange(claims.Select(p=>p.Value).ToArray());
                    }
                }
                return Results.Ok(permissions);
            });
        }
    }
}