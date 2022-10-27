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

            if (input.FromTime != null)
                input.FromTime = input.FromTime.Value.Date;

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
            .RequireAuthorization(TimeOffPermissions.View)
            ;

            app.MapPost("StaffRecord", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromBody] StaffRecordCreateOrEdit input
            ) =>
            {
                var entity = input.Adapt<StaffRecord>();
                var employeeDetail = await (from u in db.Users
                                            join d in db.Departments on u.DeptId equals d.Id
                                            where u.Id == entity.EmployeeId
                                            select new { u.Id, u.Salary, d.WorkingHours })
                                            .FirstOrDefaultAsync();

                entity.UpdateCalculationAmount(salary: employeeDetail?.Salary ?? 0, workingHours: employeeDetail?.WorkingHours ?? 0);
                entity.CreatorUserId = loggedUser.Id;
                db.StaffRecords.Add(entity);
                db.SaveChanges();
                return Results.Ok();
            })
            //.RequireAuthorization(StaffRecordPermissions.Create)
            ;

            app.MapPut("StaffRecord", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromBody] StaffRecordCreateOrEdit input
            ) =>
            {
                var entity = db.StaffRecords.Include(p => p.StaffRecordDocuments).FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                entity.StaffRecordDocuments.Clear();
                input.Adapt(entity);
                var employeeDetail = await (from u in db.Users
                                            join d in db.Departments on u.DeptId equals d.Id
                                            where u.Id == entity.EmployeeId
                                            select new { u.Id, u.Salary, d.WorkingHours })
                                            .FirstOrDefaultAsync();

                entity.UpdateCalculationAmount(salary: employeeDetail?.Salary ?? 0, workingHours: employeeDetail?.WorkingHours ?? 0);
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(TimeOffPermissions.Update)
            ;
            app.MapDelete("StaffRecord/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.StaffRecords.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(TimeOffPermissions.Delete)
            ;

            app.MapPost("StaffRecord/list", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] IUserPrincipal loggedUser,
            [FromBody] StaffRecordFilter input
            ) =>
            {
                ProcessFilterValues(ref input);
                List<BaseDropdown> departments = cacheService.GetDepartments();
                var userId = loggedUser.Id;
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
                    var isDESC = input.SortDirection.Equals(SortDirection.DESC, StringComparison.OrdinalIgnoreCase);
                    query = input.SortBy switch
                    {
                        nameof(User.DeptId) => isDESC ? query.OrderByDescending(p => p.Employee.DeptId) : query.OrderBy(p => p.Employee.DeptId),
                        nameof(User.Name) => isDESC ? query.OrderByDescending(p => p.Employee.Name) : query.OrderBy(p => p.Employee.Name),
                        nameof(User.EmployeeCode) => isDESC ? query.OrderByDescending(p => p.Employee.EmployeeCode) : query.OrderBy(p => p.Employee.EmployeeCode),
                        nameof(StaffRecord.RecordType) => isDESC ? query.OrderByDescending(p => p.RecordType) : query.OrderBy(p => p.RecordType),
                        nameof(StaffRecord.StartDate) => isDESC ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                        nameof(StaffRecord.EndDate) => isDESC ? query.OrderByDescending(p => p.EndDate) : query.OrderBy(p => p.EndDate),
                        nameof(StaffRecord.CreationTime) => isDESC ? query.OrderByDescending(p => p.CreationTime) : query.OrderBy(p => p.CreationTime),
                        nameof(StaffRecord.CreatorUserId) => isDESC ? query.OrderByDescending(p => p.CreatorUserId) : query.OrderBy(p => p.CreatorUserId),
                        _ => isDESC ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
                    };
                    items = await query/*.OrderByDynamic(input.SortBy, input.SortDirection)*/
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
                            sortBy = input.SortBy,
                            sortDirection = input.SortDirection,
                            toTime = input.ToTime,
                            exportLimit = input.RowsPerPage,
                            exportOffset = input.SkipCount
                        },
                        commandType: CommandType.StoredProcedure).ToList();

                    totalCount = items.Any() ? items.FirstOrDefault().TotalCount : 0;
                }

                var creatorIds = items.Select(p => p.CreatorUserId).Distinct().ToList();
                var creators = await db.Users.Where(p => creatorIds.Contains(p.Id)).Select(p => new BaseDropdown { Id = p.Id, Name = p.Name }).ToListAsync();
                foreach (var item in items)
                {
                    item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                    item.CreatorName = creators.FirstOrDefault(p => p.Id == item.CreatorUserId)?.Name;
                }
                return Results.Ok(new PagedResultDto<StaffRecordList>(totalCount, items));
            })
            //.RequireAuthorization(StaffRecordPermissions.View)
            ;

            app.MapGet("StaffRecord/GetEmployeesByCurrentUser", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] ApplicationDbContext db) =>
            {
                var result = new List<EmployeeDropdown>();
                var userId = loggedUser.Id;
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
            .RequireAuthorization(TimeOffPermissions.View)
            ;
        }
    }
}