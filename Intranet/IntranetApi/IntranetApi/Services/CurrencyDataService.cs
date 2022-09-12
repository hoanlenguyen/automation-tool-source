using IntranetApi.DbContext;
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
    public static class CurrencyDataService
    {
        private static void ProcessFilterValues(ref CurrencyFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        public static void AddCurrencyDataService(this WebApplication app)
        {
            app.MapGet("Currency/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = await db.Currencies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            })
            .RequireAuthorization(CurrencyPermissions.View)
            ;

            app.MapPost("Currency", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] CurrencyCreateOrEdit input
            ) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Currencies.AnyAsync(p => p.Name == input.Name && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<Currency>();
                db.Currencies.Add(entity);
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetCurrencies);
                memoryCache.Remove(CacheKeys.GetCurrenciesDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(CurrencyPermissions.Create)
            ;

            app.MapPut("Currency", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] CurrencyCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Currencies.AnyAsync(p => p.Name == input.Name && input.Id != p.Id && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Currencies.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                input.Adapt(entity);
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                memoryCache.Remove(CacheKeys.GetCurrencies);
                memoryCache.Remove(CacheKeys.GetCurrenciesDropdown);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(CurrencyPermissions.Update)
            ;

            app.MapDelete("Currency/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Currencies.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetCurrencies);
                memoryCache.Remove(CacheKeys.GetCurrenciesDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(CurrencyPermissions.Delete)
            ;

            app.MapPost("Currency/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] CurrencyFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Currencies
                           .AsNoTracking()
                           .Where(p => !p.IsDeleted)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                .Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ProjectToType<CurrencyList>()
                                .ToListAsync();
                return Results.Ok(new PagedResultDto<CurrencyList>(totalCount, items));
            })
            .RequireAuthorization(CurrencyPermissions.View)
            ;

            app.MapGet("Currency/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] /*IMemoryCacheService cacheService*/ ApplicationDbContext db
            ) =>
            {
                var items = db.Currencies.Where(p => !p.IsDeleted)
                                    .Select(p => new { p.Id, p.Name, p.CurrencySymbol })
                                    .ToList();
                return Results.Ok(items);
            });
        }
    }
}