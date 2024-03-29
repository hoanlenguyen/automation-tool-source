﻿using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

        public static void AddBrandDataService(this WebApplication app)
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
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] BrandCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Brands.AnyAsync(p => p.Name == input.Name && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var entity = input.Adapt<Brand>();
                entity.CreatorUserId = loggedUser.Id;
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
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] BrandCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Brands.AnyAsync(p => p.Name == input.Name && input.Id != p.Id && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var entity = db.Brands.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                input.Adapt(entity);
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetBrands);
                memoryCache.Remove(CacheKeys.GetBrandsDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(BrandPermissions.Update)
            ;

            app.MapDelete("Brand/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var entity = db.Brands.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
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
                                .ProjectToType<BrandList>()
                                .ToListAsync();
                return Results.Ok(new PagedResultDto<BrandList>(totalCount, items));
            })
            .RequireAuthorization(BrandPermissions.View)
            ;

            app.MapGet("Brand/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCacheService cacheService) =>
            {
                return Results.Ok(cacheService.GetBrandsDropdown());
            });

            app.MapGet("Brand/clearCache", [Authorize]
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