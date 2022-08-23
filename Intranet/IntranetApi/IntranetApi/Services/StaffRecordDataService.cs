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
    public static class StaffRecordDataService
    {
        private static void ProcessFilterValues(ref StaffRecordFilterDto input)
        {
            if (!string.IsNullOrEmpty(input.Keyword))
                input.Keyword = input.Keyword.Trim();

            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = nameof(BaseEntity.Id);
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = SortDirection.DESC;
        }

        private static List<BaseDropdown> GetDataList(string sqlConnectionStr, string tableName)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>($"select Id, Name from {tableName}s where IsDeleted = 0").ToList();
        }

        public static void AddStaffRecordDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("StaffRecord/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.StaffRecords
                            .Include(p=>p.StaffRecordDocuments)
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity.Adapt<StaffRecordCreateOrEdit>());
            })
            .RequireAuthorization(StaffRecordPermissions.View)
            ;

            app.MapPost("StaffRecord", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IFileStorageService fileService,
            [FromBody] StaffRecordCreateOrEdit input
            //,HttpRequest request
            ) =>
            {
                //if (request.Form.Files.Any())
                //{
                //    foreach (var file in request.Form.Files)
                //    {
                //        if (file is null || file.Length == 0)
                //            continue;

                //        using var fileStream = file.OpenReadStream();
                //        byte[] bytes = new byte[file.Length];
                //        fileStream.Read(bytes, 0, (int)file.Length);
                //        input.StaffRecordDocuments.Add(await fileService.SaveAndGetShortUrl(bytes, file.FileName, "StaffRecord", isAddAppfix: true));
                //    }
                //}
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<StaffRecord>();
                entity.CreatorUserId = userId;
                db.StaffRecords.Add(entity);
                db.SaveChanges();
                return Results.Ok();
            })
            //.RequireAuthorization(StaffRecordPermissions.Create)
            ;

            app.MapPut("StaffRecord", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] IFileStorageService fileService,
            [FromBody] StaffRecordCreateOrEdit input
            //,HttpRequest request
            ) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                int.TryParse(userIdStr, out var userId);
                var entity = db.StaffRecords.Include(p => p.StaffRecordDocuments).FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();
 
                entity.StaffRecordDocuments.Clear();
                Console.WriteLine($"RecordType {entity.RecordType}");
                input.Adapt(entity);
                Console.WriteLine($"RecordType {entity.RecordType}");
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(StaffRecordPermissions.Update)
            ;

            app.MapDelete("StaffRecord/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.StaffRecords.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(StaffRecordPermissions.Delete)
            ;

            app.MapPost("StaffRecord/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCache memoryCache,
            [FromBody] StaffRecordFilterDto input            
            ) =>
            {
                List<BaseDropdown> brands = null;
                List<BaseDropdown> departments = null;
                List<BaseDropdown> ranks = null;
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));

                if (!memoryCache.TryGetValue(CacheKeys.GetBrandsDropdown, out brands))
                {
                    brands = GetDataList(sqlConnectionStr, nameof(Brand));
                    memoryCache.Set(CacheKeys.GetBrandsDropdown, brands, cacheOptions);
                }

                if (!memoryCache.TryGetValue(CacheKeys.GetDepartmentsDropdown, out departments))
                {
                    departments = GetDataList(sqlConnectionStr, nameof(Department));
                    memoryCache.Set(CacheKeys.GetDepartmentsDropdown, departments, cacheOptions);
                }

                if (!memoryCache.TryGetValue(CacheKeys.GetRanksDropdown, out ranks))
                {
                    ranks = GetDataList(sqlConnectionStr, nameof(Rank));
                    memoryCache.Set(CacheKeys.GetRanksDropdown, ranks, cacheOptions);
                }
                ProcessFilterValues(ref input);
                var query = db.StaffRecords
                            .Include(p=>p.Employee)
                            .AsNoTracking()
                            .Where(p => !p.IsDeleted)
                            //.WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                             ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                       .Skip(input.SkipCount)
                                       .Take(input.RowsPerPage)
                                       .ProjectToType<StaffRecordList>()
                                       .ToListAsync();
                foreach (var item in items)
                {
                    item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                    item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                }
                return Results.Ok(new PagedResultDto<StaffRecordList>(totalCount, items));
            })
            .RequireAuthorization(StaffRecordPermissions.View)
            ;

            app.MapGet("StaffRecord/GetEmployeeByBrand", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db) =>
            {
                var result = new List<EmployeeDropdown>();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var brandIds= db.BrandEmployees.Where(p=>p.EmployeeId== userId).Select(p => p.BrandId).Distinct(); 

                var query = from be in db.BrandEmployees
                            join u in db.Users on be.EmployeeId equals u.Id
                            where brandIds.Contains(be.BrandId)
                            select new {u.Id, u.EmployeeCode, u.Name, FullName=$"{u.EmployeeCode} - {u.Name}"};

                //var employees = db.BrandEmployees
                //                .Include(p=>p.Employee)
                //                .Where(p=>p.EmployeeId== userId)
                //                .Select(p=>new {p.Employee.Id, p.Employee.EmployeeCode, p.Employee.Name})
                //                .ToList();
                
                return Results.Ok(query.ToList().DistinctBy(p => p.Id));
            })
            .RequireAuthorization(StaffRecordPermissions.View)
            ;
        }
    }
}