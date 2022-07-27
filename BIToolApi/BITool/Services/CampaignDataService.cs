using BITool.DBContext;
using BITool.Enums;
using BITool.Helpers;
using BITool.Models;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace BITool.Services
{
    public static class CampaignDataService
    {
        private static void ProcessFilterValues(ref CampaignFilterDto input)
        {
            if (!string.IsNullOrEmpty(input.Keyword))
                input.Keyword = input.Keyword.Trim();

            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = SortDirection.DESC;
        }

        private static List<BaseDropdown> GetBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Campaign where IsDeleted = 0").ToList();
        }

        public static void AddCampaignDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("Campaign/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Campaign.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            });

            app.MapPost("Campaign", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] IExportDataToQueueService exportDataToQueue,
            [FromBody] CampaignCreateOrEditDto input
            ) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<Campaign>();
                entity.CreatorUserId = userId;
                db.Add(entity);
                db.SaveChanges();
                input.Id = entity.Id;
                Parallel.Invoke(
                    () => { exportDataToQueue.BulkInsertRecordCustomerExport(sqlConnectionStr, userId, input); },
                    () => { exportDataToQueue.UpdateLastUsedCampaignOnLeadManagement(sqlConnectionStr, input); }
                    );
                memoryCache.Remove(CacheKeys.GetCampaignsDropdown);
                return Results.Ok();
            });

            app.MapPut("Campaign", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] CampaignCreateOrEditDto input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Campaign.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                input.Adapt(entity);
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                memoryCache.Remove(CacheKeys.GetCampaignsDropdown);
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapDelete("Campaign/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Campaign.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                memoryCache.Remove(CacheKeys.GetCampaignsDropdown);
                using var connection = new MySqlConnection(sqlConnectionStr);
                connection.Open();
                var commandStr = $"delete from RecordCustomerExport where CampaignID = {id}";
                using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                //commandStr = $"update leadmanagementreport set LastUsedCampaignId = null where LastUsedCampaignId = {id}";
                //using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                //{
                //    myCmd.CommandType = CommandType.Text;
                //    myCmd.ExecuteNonQuery();
                //}

                //commandStr = $"update leadmanagementreport set SecondLastUsedCampaignId = null where SecondLastUsedCampaignId = {id}";
                //using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                //{
                //    myCmd.CommandType = CommandType.Text;
                //    myCmd.ExecuteNonQuery();
                //}

                //commandStr = $"update leadmanagementreport set ThirdLastUsedCampaignId = null where ThirdLastUsedCampaignId = {id}";
                //using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                //{
                //    myCmd.CommandType = CommandType.Text;
                //    myCmd.ExecuteNonQuery();
                //}

                connection.Close();
                return Results.Ok();
            });

            app.MapPost("Campaign/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] CampaignFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var query = db.Campaign.AsNoTracking()
                            .Where(p => !p.IsDeleted)
                            .WhereIf(!string.IsNullOrEmpty(input.Keyword),p=>p.Name.Contains(input.Keyword))
                            ;
                var totalCount= query.Count();
                query = input.SortDirection switch
                {
                    SortDirection.ASC => input.SortBy switch
                    {
                        nameof(Campaign.Name)=> query.OrderBy(p=>p.Name),
                        nameof(Campaign.StartDate)=> query.OrderBy(p=>p.StartDate),
                        _ => query.OrderBy(p => p.Id)
                    },
                    _ => input.SortBy switch
                    {
                        nameof(Campaign.Name) => query.OrderByDescending(p => p.Name),
                        nameof(Campaign.StartDate)=> query.OrderByDescending(p=>p.StartDate),
                        _ => query.OrderByDescending(p => p.Id)
                    }
                };
                var items = query.Skip(input.SkipCount).Take(input.RowsPerPage).ToList();
                return Results.Ok(new PagedResultDto<Campaign>(totalCount, items));                
            });

            app.MapGet("Campaign/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache) =>
            {
                List<BaseDropdown> items = null;
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                if (!memoryCache.TryGetValue(CacheKeys.GetCampaignsDropdown, out items))
                {
                    items = GetBaseDropdown(sqlConnectionStr);
                    memoryCache.Set(CacheKeys.GetCampaignsDropdown, items, cacheOptions);
                }
                return Results.Ok(items);
            });

            app.MapPost("Campaign/assign", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IExportDataToQueueService exportDataToQueue,
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] CampaignCreateOrEditDto input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                if (input.Id == 0)
                {
                    var entity = input.Adapt<Campaign>();
                    entity.CreatorUserId = userId;
                    db.Add(entity);
                    db.SaveChanges();
                    memoryCache.Remove(CacheKeys.GetCampaignsDropdown);
                    Console.WriteLine($"new Campaign id: {entity.Id}");
                    input.Id = entity.Id;
                }
                
                Console.WriteLine($"Campaign/assign: {input.Id}");
                //var shouldSendEmail = processAssign.CustomerList.Count > config.GetValue<int>("ShouldSendEmailWhenReachLimit");
                Parallel.Invoke(
                    () => { exportDataToQueue.BulkInsertRecordCustomerExport(sqlConnectionStr, userId, input);},
                    () => { exportDataToQueue.UpdateLastUsedCampaignOnLeadManagement(sqlConnectionStr, input);}
                    );

                return Results.Ok(input);
            });
        }
    }
}