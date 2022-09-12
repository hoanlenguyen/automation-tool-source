using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace IntranetApi.Services
{
    public static class DepartmentDataService
    {
        private static void ProcessFilterValues(ref DepartmentFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        public static void AddDepartmentDataService(this WebApplication app)
        {
            app.MapGet("Department/{id:int}", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Departments.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            })
            .RequireAuthorization(DepartmentPermissions.View)
            ;

            app.MapPost("Department", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] DepartmentCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                if (input.WorkingHours < 0)
                    throw new Exception("No valid Working hours!");

                var checkExisted = await db.Departments.AnyAsync(p => p.Name == input.Name && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Department { Name = input.Name, CreatorUserId = userId, WorkingHours = input.WorkingHours };
                db.Add(entity);
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetDepartments);
                memoryCache.Remove(CacheKeys.GetDepartmentsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(DepartmentPermissions.Create)
            ;

            app.MapPut("Department", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] DepartmentCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                if (input.WorkingHours < 0)
                    throw new Exception("No valid Working hours!");

                var checkExisted = await db.Departments.AnyAsync(p => p.Name == input.Name && input.Id != p.Id && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Departments.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.Name = input.Name;
                entity.Status = input.Status;
                entity.WorkingHours = input.WorkingHours;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetDepartments);
                memoryCache.Remove(CacheKeys.GetDepartmentsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(DepartmentPermissions.Update)
            ;

            app.MapDelete("Department/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Departments.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetDepartments);
                memoryCache.Remove(CacheKeys.GetDepartmentsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(DepartmentPermissions.Delete)
            ;

            app.MapPost("Department/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] DepartmentFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Departments
                           .AsNoTracking()
                           .Where(p => !p.IsDeleted)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                .Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ProjectToType<DepartmentList>()
                                .ToListAsync();
                return Results.Ok(new PagedResultDto<DepartmentList>(totalCount, items));
            })
            .RequireAuthorization(DepartmentPermissions.View)
            ;

            app.MapGet("Department/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCacheService cacheService) =>
            {
                return Results.Ok(cacheService.GetDepartmentsDropdown());
            });
        }
    }
}