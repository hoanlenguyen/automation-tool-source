using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntranetApi.Services
{
    public static class LeaveHistoryService
    {
        public static void AddLeaveHistoryService(this WebApplication app)
        {
            app.MapPost("LeaveHistory/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromServices] IMemoryCacheService cacheService,
            [FromBody] LeaveHistoryFilter input
            ) =>
            {
                List<BaseDropdown> brands = cacheService.GetBrands();
                List<BaseDropdown> departments = cacheService.GetDepartments();
                List<BaseDropdown> ranks = cacheService.GetRanks();

                if (!string.IsNullOrEmpty(input.Keyword))
                    input.Keyword = input.Keyword.Trim();

                if (string.IsNullOrEmpty(input.SortBy))
                    input.SortBy = nameof(BaseEntity.Id);

                if (string.IsNullOrEmpty(input.SortDirection))
                    input.SortDirection = SortDirection.DESC;

                var totalCount = await db.StaffRecords.Include(p => p.Employee)
                            .Where(p => !p.IsDeleted && p.Employee.UserType == UserType.Employee)
                            .Select(p => p.EmployeeId).Distinct().CountAsync();

                var query = db.StaffRecords
                            .Include(p => p.Employee)
                            .ThenInclude(q => q.BrandEmployees)
                            .AsNoTracking()
                            .Where(p => !p.IsDeleted && p.Employee.UserType == UserType.Employee)
                            .ToList()
                            .GroupBy(p => p.EmployeeId)
                            .Select(p => new LeaveHistoryList
                            {
                                EmployeeId = p.Key,
                                EmployeeName = p.FirstOrDefault().Employee.Name,
                                EmployeeCode = p.FirstOrDefault().Employee.EmployeeCode,
                                DepartmentId = p.FirstOrDefault().Employee.DeptId,
                                RankId = p.FirstOrDefault().Employee.RankId,
                                BrandEmployees = p.FirstOrDefault().Employee.BrandEmployees.Select(p => p.BrandId),
                                SumDaysOfPaidOffs = p.Where(p => p.RecordType == StaffRecordType.PaidOffs).Sum(p => p.NumberOfDays),
                                SumDaysOfPaidMCs = p.Where(p => p.RecordType == StaffRecordType.PaidMCs).Sum(p => p.NumberOfDays),
                                SumDaysOfDeduction = p.Where(p => p.RecordType == StaffRecordType.Deduction).Sum(p => p.NumberOfDays),
                                SumHoursOfDeduction = p.Where(p => p.RecordType == StaffRecordType.Deduction).Sum(p => p.NumberOfHours),
                                SumDaysOfExtraPay = p.Where(p => p.RecordType == StaffRecordType.ExtraPay).Sum(p => p.NumberOfDays),
                                SumHoursOfExtraPay = p.Where(p => p.RecordType == StaffRecordType.ExtraPay).Sum(p => p.NumberOfHours),
                                LateAmount = p.Sum(p => p.LateAmount),
                                Fines = p.Sum(p => p.Fine)
                            });
                //.WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                ;
                var items = /*await*/ query/*.OrderByDynamic(input.SortBy, input.SortDirection)*/
                                       .Skip(input.SkipCount)
                                       .Take(input.RowsPerPage)
                                       .ToList();
                //.ProjectToType<LeaveHistoryList>()
                //.ToListAsync()
                ;

                foreach (var item in items)
                {
                    item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                    item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                    item.Brand = string.Join(',', brands.Where(p => item.BrandEmployees.Contains(p.Id)).Select(p => p.Name));
                    item.Brands = brands.Where(p => item.BrandEmployees.Contains(p.Id)).Select(p => p.Name);
                }
                return Results.Ok(new PagedResultDto<LeaveHistoryList>(totalCount, items));
            })
            .RequireAuthorization(LeaveHistoryPermissions.View)
            ;
        }
    }
}