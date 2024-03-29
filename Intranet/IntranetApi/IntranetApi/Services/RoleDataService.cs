﻿using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

        public static void AddRoleDataService(this WebApplication app)
        {
            app.MapGet("Role/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Roles.Include(p => p.RoleClaims)
                                    .Include(p => p.RoleDepartments)
                                    .ThenInclude(p => p.Department)
                                    .AsNoTracking()
                                    .FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                var result = entity.Adapt<RoleCreateOrEdit>();

                return Results.Ok(result);
            })
            .RequireAuthorization(RolePermissions.View)
            ;

            app.MapPost("Role", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] RoleManager<Role> roleManager,
            [FromBody] RoleCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid role name!");

                var checkExisted = await db.Roles.AnyAsync(p => p.Name == input.Name && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");

                input.Name = input.Name.Trim();
                var entity = input.Adapt<Role>();
                entity.CreatorUserId = loggedUser.Id;
                await db.Roles.AddAsync(entity); 
                memoryCache.Remove(CacheKeys.GetRoles);
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                await db.SaveChangesAsync();
                return Results.Ok(entity);
            })
            .RequireAuthorization(RolePermissions.Create)
            ;

            app.MapPut("Role", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] RoleCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                if (await db.Roles.AnyAsync(p => p.Name == input.Name && p.Id != input.Id && !p.IsDeleted))
                    throw new Exception("Name already exists");

                var entity = db.Roles
                            .Include(p=>p.RoleClaims)
                            .Include(p=>p.RoleDepartments)
                            .FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.RoleClaims.Clear();
                entity.RoleDepartments.Clear();
                input.Adapt(entity);
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);                 
                memoryCache.Remove(CacheKeys.GetRoles);
                memoryCache.Remove(CacheKeys.GetRolesDropdown);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(RolePermissions.Update)
            ;

            app.MapDelete("Role/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var entity = db.Roles.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetRoles);
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
                var query = db.Roles
                           .AsNoTracking()
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
                    var creators = await db.Users.AsNoTracking()
                                    .Where(p => creatorUserIds.Contains(p.Id))
                                    .Select(p => new BaseDropdown { Id = p.Id, Name = p.Name })
                                    .ToListAsync();
                    var roleIds = items.Select(p => p.Id);
                    var roleEmployees = await db.UserRoles.Include(p => p.User)
                                            .AsNoTracking()
                                            .Where(p => roleIds.Contains(p.RoleId))
                                            .GroupBy(p => p.RoleId)
                                            .Select(p => new RoleEmployeeList { RoleId = p.Key, Employees = p.Select(q => new EmployeeSimpleDto { Name = q.User.Name, EmployeeCode = q.User.EmployeeCode }).ToList() })
                                            .ToListAsync();
                    foreach (var item in items)
                    {
                        if (item.CreatorUserId != null)
                            item.CreatorUser = creators.FirstOrDefault(p => p.Id == item.CreatorUserId.Value)?.Name;

                        if (item.LastModifierUserId != null)
                            item.LastModifierUser = creators.FirstOrDefault(p => p.Id == item.LastModifierUserId.Value)?.Name;

                        var roleEmployee = roleEmployees.FirstOrDefault(p => p.RoleId == item.Id);
                        if (roleEmployee != null)
                        {
                            item.Employees = roleEmployee.Employees;
                        }
                    }
                }
                return Results.Ok(new PagedResultDto<RoleListItem>(totalCount, items));
            })
            .RequireAuthorization(RolePermissions.View)
            ;

            app.MapGet("Role/dropdown", [Authorize]
            async Task<IResult> (
            IMemoryCacheService memoryCacheService) =>
            {
                return Results.Ok(memoryCacheService.GetRolesDropdown());
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
            [FromServices] ApplicationDbContext db,
            [FromServices] RoleManager<Role> roleManager,
            [FromServices] UserManager<User> userManager,
            [FromQuery] string id
            ) =>
            {
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

            app.MapGet("Role/AllPermissions", [Authorize]
            async Task<IResult> () =>
            {
                List<PermissionUIDto> items = new();
                foreach (var value in Permissions.AllPermissionModules)
                {
                    var item = new PermissionUIDto { Id = value, Label = value };
                    item.Children = Permissions.GeneratePermissionsForModule(value)
                                    .Select(p=>new PermissionBaseUIDto
                                    {
                                        Id= p,
                                        Label= p
                                    }).ToList();
                    
                    items.Add(item);
                }
                return Results.Ok(items);
            });

            app.MapGet("Role/UserPrincipal", [Authorize]
            async Task<IResult> (IUserPrincipal user) =>
            {
                
                return Results.Ok(new { user });
            });
        }
    }
}