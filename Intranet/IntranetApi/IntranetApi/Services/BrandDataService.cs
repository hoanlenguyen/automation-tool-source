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
    public static class BrandDataService
    {
        private static void ProcessFilterValues(ref BrandFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }
        
        public static void AddBrandDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("Brand/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Brands.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            })
            .RequireAuthorization(BrandPermissions.View)
            ;

            app.MapPost("Brand", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] BrandCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Brands.AnyAsync(p => p.Name == input.Name && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Brand { Name = input.Name, CreatorUserId = userId };
                db.Add(entity);
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetBrands);
                memoryCache.Remove(CacheKeys.GetBrandsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(BrandPermissions.Create)
            ;

            app.MapPut("Brand", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] BrandCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Brands.AnyAsync(p => p.Name == input.Name && input.Id != p.Id && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Brands.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.Name = input.Name;
                entity.Status = input.Status;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetBrands);
                memoryCache.Remove(CacheKeys.GetBrandsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(BrandPermissions.Update)
            ;

            app.MapDelete("Brand/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Brands.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetBrands);
                memoryCache.Remove(CacheKeys.GetBrandsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(BrandPermissions.Delete)
            ;

            app.MapPost("Brand/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] BrandFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Brands
                           .AsNoTracking()
                           .Where(p => !p.IsDeleted)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                .Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ProjectToType<BrandCreateOrEdit>()
                                .ToListAsync();
                return Results.Ok(new PagedResultDto<BrandCreateOrEdit>(totalCount, items));
            })
            .RequireAuthorization(BrandPermissions.View)
            ;

            app.MapGet("Brand/dropdown", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IMemoryCacheService cacheService) =>
            {
                return Results.Ok(cacheService.GetBrandsDropdown());
            });

            app.MapGet("Brand/clearCache", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache) =>
            {
                memoryCache.Remove(CacheKeys.GetBrands);
                memoryCache.Remove(CacheKeys.GetBrandsDropdown);
                return Results.Ok();
            });
        }
    }
}