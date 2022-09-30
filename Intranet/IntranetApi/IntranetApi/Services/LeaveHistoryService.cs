using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace IntranetApi.Services
{
    public static class LeaveHistoryService
    {
        public static void AddLeaveHistoryService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapPost("LeaveHistory/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromBody] LeaveHistoryFilter input
            ) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);

                List<BaseDropdown> brands = cacheService.GetBrands();
                List<BaseDropdown> departments = cacheService.GetDepartments();
                List<BaseDropdown> ranks = cacheService.GetRanks();
                List<CurrencySimpleDto> currencies = cacheService.GetCurrencies();
                var items = new List<LeaveHistoryList>();
                var totalCount = 0;
                if (!string.IsNullOrEmpty(input.Keyword))
                    input.Keyword = input.Keyword.Trim();

                if (string.IsNullOrEmpty(input.SortBy))
                    input.SortBy = nameof(BaseEntity.Id);

                if (string.IsNullOrEmpty(input.SortDirection))
                    input.SortDirection = SortDirection.DESC;

                if (input.ToTime != null)
                    input.ToTime = input.ToTime.Value.Date.AddDays(1).AddTicks(-1);

                var isSuperAdmin = await db.UserRoles
                                .Include(p => p.Role)
                                .AnyAsync(p => p.UserId == userId && p.Role.IsSuperAddmin && !p.Role.IsDeleted);

                if (isSuperAdmin)
                {
                    var isAllBrand = input.BrandId != null? await db.Brands
                               .AnyAsync(p => p.Id== input.BrandId && p.IsAllBrand) : true;
                    //Console.WriteLine($"isAllBrand {isAllBrand}");
                    totalCount = await db.StaffRecords
                            .Include(p => p.Employee)
                            .Where(p => !p.IsDeleted && p.Employee.UserType == UserType.Employee)
                            .WhereIf(input.FromTime != null, p => p.CreationTime >= input.FromTime.Value)
                            .WhereIf(input.ToTime != null, p => p.CreationTime <= input.ToTime.Value)
                            .WhereIf(!isAllBrand, p => p.Employee.BrandEmployees.Any(p => p.BrandId == input.BrandId ))
                            .Select(p => p.EmployeeId)
                            .Distinct()
                            .CountAsync();
                    var query = db.StaffRecords
                            .Include(p => p.Employee)
                            .ThenInclude(q => q.BrandEmployees)
                            .AsNoTracking()
                            .Where(p => !p.IsDeleted && p.Employee.UserType == UserType.Employee)
                            .WhereIf(input.FromTime != null, p => p.CreationTime >= input.FromTime.Value)
                            .WhereIf(input.ToTime != null, p => p.CreationTime <= input.ToTime.Value)
                            .WhereIf(!isAllBrand, p => p.Employee.BrandEmployees.Any(p => p.BrandId == input.BrandId ))
                            .WhereIf(input.DepartmentId!= null, p => p.Employee.DeptId == input.DepartmentId)
                            .ToList()
                            .GroupBy(p => p.EmployeeId)
                            .Select(p => new LeaveHistoryList
                            {
                                EmployeeId = p.Key,
                                EmployeeName = p.FirstOrDefault().Employee.Name,
                                EmployeeCode = p.FirstOrDefault().Employee.EmployeeCode,
                                DepartmentId = p.FirstOrDefault().Employee.DeptId,
                                RankId = p.FirstOrDefault().Employee.RankId,
                                Country = p.FirstOrDefault().Employee.Country,
                                BrandEmployees = p.FirstOrDefault().Employee.BrandEmployees.Select(p => p.BrandId),
                                SumDaysOfPaidOffs = p.Where(p => p.RecordType == StaffRecordType.PaidOffs).Sum(p => p.NumberOfDays),
                                SumDaysOfPaidMCs = p.Where(p => p.RecordType == StaffRecordType.PaidMCs).Sum(p => p.NumberOfDays),
                                SumDaysOfDeduction = p.Where(p => p.RecordType == StaffRecordType.Deduction).Sum(p => p.NumberOfDays),
                                SumHoursOfDeduction = p.Where(p => p.RecordType == StaffRecordType.Deduction).Sum(p => p.NumberOfHours),
                                SumDaysOfExtraPay = p.Where(p => p.RecordType == StaffRecordType.ExtraPay).Sum(p => p.NumberOfDays),
                                SumHoursOfExtraPay = p.Where(p => p.RecordType == StaffRecordType.ExtraPay).Sum(p => p.NumberOfHours),
                                LateAmount = p.Sum(p => p.LateAmount),
                                SumCalculationAmount= p.Sum(p=>p.CalculationAmount),
                                Fines = p.Sum(p => p.Fine)
                            });
                    ;
                    items = query/*.OrderByDynamic(input.SortBy, input.SortDirection)*/
                                  .Skip(input.SkipCount)
                                  .Take(input.RowsPerPage)
                                  .ToList();

                    foreach (var item in items)
                    {
                        item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                        item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                        item.Brands = brands.Where(p => item.BrandEmployees.Contains(p.Id)).Select(p => p.Name);
                        item.CurrencySymbol = currencies.FirstOrDefault(p=>p.Name.Equals(item.Country, StringComparison.OrdinalIgnoreCase))?.CurrencySymbol;
                    }
                }
                else
                {
                    using var connection = new MySqlConnection(sqlConnectionStr);
                    var records = connection.Query<StaffRecordData>("SP_Filter_Leave_History",
                        new
                        {
                            currentUserId = userId,
                            inputBrandId = input.BrandId,
                            inputDepartmentId = input.DepartmentId,
                            fromTime = input.FromTime,
                            toTime = input.ToTime,
                        },
                        commandType: CommandType.StoredProcedure).ToList();

                    totalCount = records.Select(p => p.EmployeeId).Distinct().Count();
                    items = records.GroupBy(p => p.EmployeeId)
                            .Select(p => new LeaveHistoryList
                            {
                                EmployeeId = p.Key,
                                EmployeeName = p.FirstOrDefault().EmployeeCode,
                                EmployeeCode = p.FirstOrDefault().EmployeeCode,
                                DepartmentId = p.FirstOrDefault().DepartmentId,
                                RankId = p.FirstOrDefault().RankId,
                                Country= p.FirstOrDefault().Country,
                                SumDaysOfPaidOffs = p.Where(p => p.RecordType == StaffRecordType.PaidOffs).Sum(p => p.NumberOfDays),
                                SumDaysOfPaidMCs = p.Where(p => p.RecordType == StaffRecordType.PaidMCs).Sum(p => p.NumberOfDays),
                                SumDaysOfDeduction = p.Where(p => p.RecordType == StaffRecordType.Deduction).Sum(p => p.NumberOfDays),
                                SumHoursOfDeduction = p.Where(p => p.RecordType == StaffRecordType.Deduction).Sum(p => p.NumberOfHours),
                                SumDaysOfExtraPay = p.Where(p => p.RecordType == StaffRecordType.ExtraPay).Sum(p => p.NumberOfDays),
                                SumHoursOfExtraPay = p.Where(p => p.RecordType == StaffRecordType.ExtraPay).Sum(p => p.NumberOfHours),
                                LateAmount = p.Sum(p => p.LateAmount),
                                SumCalculationAmount = p.Sum(p => p.CalculationAmount),
                                Fines = p.Sum(p => p.Fine)
                            })
                            .Skip(input.SkipCount)
                            .Take(input.RowsPerPage)
                            .ToList();
                    var employeeIds = items.Select(p => p.EmployeeId);
                    var brandEmployees = await db.BrandEmployees
                                .Where(p => employeeIds.Contains(p.EmployeeId))
                                .GroupBy(p => p.EmployeeId)
                                .Select(p => new { EmployeeId = p.Key, BrandIds = p.Select(p => p.BrandId).ToList() })
                                .ToListAsync();

                    foreach (var item in items)
                    {
                        item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                        item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                        item.BrandEmployees = brandEmployees.FirstOrDefault(p => p.EmployeeId == item.EmployeeId).BrandIds;
                        item.Brands = brands.Where(p => item.BrandEmployees.Contains(p.Id)).Select(p => p.Name);
                        item.CurrencySymbol = currencies.FirstOrDefault(p => p.Name.Equals(item.Country, StringComparison.OrdinalIgnoreCase))?.CurrencySymbol;
                    }
                }

                return Results.Ok(new PagedResultDto<LeaveHistoryList>(totalCount, items));
            })
            .RequireAuthorization(LeaveHistoryPermissions.View)
            ;

            app.MapGet("LeaveHistory/GetBrandDropdownByUser", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] IHttpContextAccessor httpContextAccessor) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var isSuperAdmin = await db.UserRoles
                                .Include(p => p.Role)
                                .AnyAsync(p => p.UserId == userId && p.Role.IsSuperAddmin && !p.Role.IsDeleted);
                
                if (isSuperAdmin)
                {
                    return Results.Ok(cacheService.GetBrandsDropdown());
                }
                var items = db.BrandEmployees
                            .Include(p => p.Brand)
                            .Where(p => p.EmployeeId == userId && !p.Brand.IsDeleted && p.Brand.Status)
                            .Select(p => new BaseDropdown { Id = p.Brand.Id, Name = p.Brand.Name });
                return Results.Ok(items);
            });

            app.MapGet("LeaveHistory/GetBrandAndDepartmentDropdownByUser", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] IHttpContextAccessor httpContextAccessor) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var role = await db.UserRoles
                                .Include(p => p.Role)
                                .Where(p => p.UserId == userId && !p.Role.IsDeleted)
                                .Select(p=>new { UserId= userId, RoleId= p.RoleId, IsSuperAddmin= p.Role.IsSuperAddmin})
                                .FirstOrDefaultAsync();

                if(role == null || role.RoleId==0)
                    return Results.Ok();

                if (role.IsSuperAddmin)
                {
                    return Results.Ok(new { 
                        brands = cacheService.GetBrands(),
                        departments= cacheService.GetDepartments()
                    });
                }
                var brands = db.BrandEmployees
                           .Include(p => p.Brand)
                           .Where(p => p.EmployeeId == userId && !p.Brand.IsDeleted && p.Brand.Status)
                           .Select(p => new BaseDropdown { Id = p.Brand.Id, Name = p.Brand.Name });

                var query = from rd in db.RoleDepartments
                            join d in db.Departments on rd.DepartmentId equals d.Id
                            where rd.RoleId == role.RoleId && !rd.Department.IsDeleted
                            select new BaseDropdown { Id = d.Id, Name = d.Name };

                var departments = await query.ToListAsync();

                return Results.Ok(new {brands, departments});
            });
        }
    }
}