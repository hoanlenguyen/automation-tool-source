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
    public static class RankDataService
    {
        private static void ProcessFilterValues(ref RankFilterDto input)
        {
            if (!string.IsNullOrEmpty(input.Keyword))
                input.Keyword = input.Keyword.Trim();

            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = nameof(BaseEntity.Id);
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = SortDirection.DESC;
        }

        private static List<BaseDropdown> GetBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Rank where IsDeleted = 0").ToList();
        }

        public static void AddRankDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("Rank/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Rank.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            })
            .RequireAuthorization(RankPermissions.View)
            ;

            app.MapPost("Rank", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] RankCreateOrEdit input
            ) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<Rank>();
                entity.CreatorUserId = userId;
                db.Add(entity);
                db.SaveChanges();
                input.Id = entity.Id;                
                memoryCache.Remove(CacheKeys.GetRanksDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(RankPermissions.Create)
            ;

            app.MapPut("Rank", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] RankCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Rank.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                input.Adapt(entity);
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                memoryCache.Remove(CacheKeys.GetRanksDropdown);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(RankPermissions.Update)
            ;

            app.MapDelete("Rank/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Rank.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetRanksDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(RankPermissions.Delete)
            ;

            app.MapPost("Rank/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] RankFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Rank.AsNoTracking()
                            .Where(p => !p.IsDeleted)
                            .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                            ;
                var totalCount =await query.CountAsync();
                query = query.OrderByDynamic(input.SortBy, input.SortDirection);
                var items = await query.Skip(input.SkipCount).Take(input.RowsPerPage).ToListAsync();
                return Results.Ok(new PagedResultDto<Rank>(totalCount, items));
            })
            .RequireAuthorization(RankPermissions.View)
            ;

            app.MapGet("Rank/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache) =>
            {
                List<BaseDropdown> items = null;
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                if (!memoryCache.TryGetValue(CacheKeys.GetRanksDropdown, out items))
                {
                    items = GetBaseDropdown(sqlConnectionStr);
                    memoryCache.Set(CacheKeys.GetRanksDropdown, items, cacheOptions);
                }
                return Results.Ok(items);
            });             
        }
    }
}