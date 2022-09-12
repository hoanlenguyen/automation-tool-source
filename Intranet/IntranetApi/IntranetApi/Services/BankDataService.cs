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
    public static class BankDataService
    {
        private static void ProcessFilterValues(ref BankFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        public static void AddBankDataService(this WebApplication app)
        {
            app.MapGet("bank/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = await db.Banks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            })
            .RequireAuthorization(BankPermissions.View)
            ;

            app.MapPost("bank", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] BankCreateOrEdit input
            ) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Banks.AnyAsync(p => p.Name == input.Name && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Bank { Name = input.Name, CreatorUserId = userId };
                db.Banks.Add(entity);
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetBanks);
                memoryCache.Remove(CacheKeys.GetBanksDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(BankPermissions.Create)
            ;

            app.MapPut("bank", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] BankCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                var checkExisted = await db.Banks.AnyAsync(p => p.Name == input.Name && input.Id != p.Id && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception("Name already exists");
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Banks.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.Name = input.Name;
                entity.Status = input.Status;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                memoryCache.Remove(CacheKeys.GetBanks);
                memoryCache.Remove(CacheKeys.GetBanksDropdown);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(BankPermissions.Update)
            ;

            app.MapDelete("bank/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Banks.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetBanks);
                memoryCache.Remove(CacheKeys.GetBanksDropdown);
                return Results.Ok();
            })
            .RequireAuthorization(BankPermissions.Delete)
            ;

            app.MapPost("bank/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] BankFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Banks
                           .AsNoTracking()
                           .Where(p => !p.IsDeleted)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                .Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ProjectToType<BankCreateOrEdit>()
                                .ToListAsync();
                return Results.Ok(new PagedResultDto<BankCreateOrEdit>(totalCount, items));
            })
            .RequireAuthorization(BankPermissions.View)
            ;

            app.MapGet("Bank/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCacheService cacheService) =>
            {
                return Results.Ok(cacheService.GetBanksDropdown());
            });
        }
    }
}