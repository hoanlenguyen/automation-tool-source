using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace IntranetApi.Services
{
    public static class StaffRecordDataService
    {
        private static void ProcessFilterValues(ref StaffRecordFilter input)
        {
            if (!string.IsNullOrEmpty(input.Keyword))
                input.Keyword = input.Keyword.Trim();

            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = nameof(BaseEntity.Id);

            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = SortDirection.DESC;

            if (input.ToTime != null)
                input.ToTime = input.ToTime.Value.Date.AddDays(1).AddTicks(-1);
        }

        public static void AddStaffRecordDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("StaffRecord/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.StaffRecords
                            .Include(p => p.StaffRecordDocuments)
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
            [FromBody] StaffRecordCreateOrEdit input
            ) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<StaffRecord>();
                var workingHours = await db.Departments
                                        .Where(p => p.Id == entity.DepartmentId)
                                        .Select(p => p.WorkingHours)
                                        .FirstOrDefaultAsync();
                var salary = await db.Users.Where(p => p.Id == entity.EmployeeId)
                                         .Select(p => p.Salary)
                                         .FirstOrDefaultAsync();
                switch (entity.RecordDetailType)
                {
                    case StaffRecordDetailType.PaidMCs:
                    case StaffRecordDetailType.PaidOffs:
                    case StaffRecordDetailType.ExtraPayCoverShift:
                    case StaffRecordDetailType.DeductionUnpaidLeave:
                        {
                            entity.NumberOfDays = (entity.EndDate - entity.StartDate).Days + 1;
                            if (entity.RecordDetailType == StaffRecordDetailType.ExtraPayCoverShift)
                            {
                                entity.CalculationAmount = entity.NumberOfDays * (salary / 365);
                            }

                            if (entity.RecordDetailType == StaffRecordDetailType.DeductionUnpaidLeave)
                            {
                                entity.CalculationAmount = -entity.NumberOfDays * (salary / 365);
                            }
                            break;
                        }
                    case StaffRecordDetailType.ExtraPayOTs:
                        {
                            entity.NumberOfHours = (int)Math.Round((entity.EndDate - entity.StartDate).TotalHours);
                            if (entity.RecordDetailType == StaffRecordDetailType.ExtraPayOTs && workingHours > 0)
                            {
                                entity.CalculationAmount = entity.NumberOfHours * (salary / (365 * workingHours));
                            }
                            break;
                        }
                    case StaffRecordDetailType.DeductionLate:
                        {
                            entity.CalculationAmount = -entity.LateAmount;
                            break;
                        }
                    default: break;
                }

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
            [FromBody] StaffRecordCreateOrEdit input
            ) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);

                var entity = db.StaffRecords.Include(p => p.StaffRecordDocuments).FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.StaffRecordDocuments.Clear();
                input.Adapt(entity);
                entity.NumberOfDays = 0;
                entity.NumberOfHours = 0;
                entity.CalculationAmount = 0;
                var workingHours = await db.Departments
                                        .Where(p => p.Id == entity.DepartmentId)
                                        .Select(p => p.WorkingHours)
                                        .FirstOrDefaultAsync();

                var salary = await db.Users.Where(p => p.Id == entity.EmployeeId)
                                         .Select(p => p.Salary)
                                         .FirstOrDefaultAsync();

                switch (entity.RecordDetailType)
                {
                    case StaffRecordDetailType.PaidMCs:
                    case StaffRecordDetailType.PaidOffs:
                    case StaffRecordDetailType.ExtraPayCoverShift:
                    case StaffRecordDetailType.DeductionUnpaidLeave:
                        {
                            entity.NumberOfDays = (entity.EndDate - entity.StartDate).Days + 1;
                            if (entity.RecordDetailType == StaffRecordDetailType.ExtraPayCoverShift)
                            {
                                entity.CalculationAmount = entity.NumberOfDays * (salary / 365);
                            }

                            if (entity.RecordDetailType == StaffRecordDetailType.DeductionUnpaidLeave)
                            {
                                entity.CalculationAmount = -entity.NumberOfDays * (salary / 365);
                            }
                            break;
                        }
                    case StaffRecordDetailType.ExtraPayOTs:
                        {
                            entity.NumberOfHours = (int)Math.Round((entity.EndDate - entity.StartDate).TotalHours);
                            if (entity.RecordDetailType == StaffRecordDetailType.ExtraPayOTs && workingHours > 0)
                            {
                                entity.CalculationAmount = entity.NumberOfHours * (salary / (365 * workingHours));
                            }
                            break;
                        }
                    case StaffRecordDetailType.DeductionLate:
                        {
                            entity.CalculationAmount = -entity.LateAmount;
                            break;
                        }
                    default: break;
                }
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
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
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(StaffRecordPermissions.Delete)
            ;

            app.MapPost("StaffRecord/list", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromBody] StaffRecordFilter input
            ) =>
            {
                ProcessFilterValues(ref input);
                //List<BaseDropdown> brands = cacheService.GetBrands();
                List<BaseDropdown> departments = cacheService.GetDepartments();
                //List<BaseDropdown> ranks = cacheService.GetRanks();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var totalCount = 0;
                var items = new List<StaffRecordList>();
                var isSuperAdmin = await db.UserRoles
                                .Include(p => p.Role)
                                .AnyAsync(p => p.UserId == userId && p.Role.IsSuperAddmin && !p.Role.IsDeleted);

                if (isSuperAdmin)
                {
                    var query = db.StaffRecords
                                .Include(p => p.Employee)
                                .AsNoTracking()
                                .Where(p => !p.IsDeleted)
                                .WhereIf(input.FromTime != null, p => p.CreationTime >= input.FromTime)
                                .WhereIf(input.ToTime != null, p => p.CreationTime <= input.ToTime)
                                ;
                    totalCount = await query.CountAsync();
                    items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                        .Skip(input.SkipCount)
                                        .Take(input.RowsPerPage)
                                        .ProjectToType<StaffRecordList>()
                                        .ToListAsync();
                }
                else
                {
                    using var connection = new MySqlConnection(sqlConnectionStr);
                    items = connection.Query<StaffRecordList>("SP_Filter_Time_Off",
                        new
                        {
                            currentUserId = userId,
                            fromTime = input.FromTime,
                            toTime = input.ToTime,
                            exportLimit = input.RowsPerPage,
                            exportOffset = input.SkipCount
                        },
                        commandType: CommandType.StoredProcedure).ToList();

                    totalCount = items.Any() ? items.FirstOrDefault().TotalCount : 0;
                }

                var creatorIds = items.Select(p => p.CreatorUserId);
                var creators = await db.Users.Where(p => creatorIds.Contains(p.Id)).Select(p => new BaseDropdown { Id = p.Id, Name = p.Name }).ToListAsync();
                foreach (var item in items)
                {
                    //item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                    item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                    item.CreatorName = creators.FirstOrDefault(p => p.Id == item.CreatorUserId)?.Name;
                }
                return Results.Ok(new PagedResultDto<StaffRecordList>(totalCount, items));
            })
            //.RequireAuthorization(StaffRecordPermissions.View)
            ;

            app.MapGet("StaffRecord/GetEmployeesByCurrentUser", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] ApplicationDbContext db) =>
            {
                var result = new List<EmployeeDropdown>();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var depts = cacheService.GetDepartments();
                var isSuperAdmin = await db.UserRoles
                                .Include(p => p.Role)
                                .AnyAsync(p => p.UserId == userId && p.Role.IsSuperAddmin && !p.Role.IsDeleted);

                if (isSuperAdmin)
                {
                    var employees = db.Users
                            .Where(p => !p.IsDeleted)
                            .Select(u => new StaffRecordDropdown { Id = u.Id, EmployeeCode = u.EmployeeCode, Name = u.Name, DepartmentId = u.DeptId })
                            .ToList();
                    foreach (var item in employees)
                    {
                        item.DepartmentName = depts.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                    }
                    return Results.Ok(employees);
                }

                using var connection = new MySqlConnection(sqlConnectionStr);
                var items = connection.Query<StaffRecordDropdown>("SP_Get_Employees_By_Current_User",
                    new { currentUserId = userId },
                    commandType: CommandType.StoredProcedure).ToList();

                items = items.DistinctBy(p => p.Id).ToList();
                foreach (var item in items)
                {
                    item.DepartmentName = depts.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                }
                return Results.Ok(items);
            })
            .RequireAuthorization(StaffRecordPermissions.View)
            ;
        }
    }
}