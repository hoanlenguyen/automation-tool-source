﻿using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        }

        public static void AddStaffRecordDataService(this WebApplication app)
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
                    case StaffRecordDetailType.DeductionLate:
                        {
                            entity.NumberOfHours = (int)Math.Round((entity.EndDate - entity.StartDate).TotalHours);
                            if (entity.RecordDetailType == StaffRecordDetailType.ExtraPayOTs && workingHours > 0)
                            {
                                entity.CalculationAmount = entity.NumberOfHours * (salary / (365 * workingHours));
                            }
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
                    case StaffRecordDetailType.DeductionLate:
                        {
                            entity.NumberOfHours = (int)Math.Round((entity.EndDate - entity.StartDate).TotalHours);
                            if (entity.RecordDetailType == StaffRecordDetailType.ExtraPayOTs && workingHours > 0)
                            {
                                entity.CalculationAmount = entity.NumberOfHours * (salary / (365 * workingHours));
                            }
                            break;
                        }
                    default: break;
                }
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
            [FromServices] IMemoryCacheService cacheService,
            [FromBody] StaffRecordFilter input
            ) =>
            {
                List<BaseDropdown> brands = cacheService.GetBrands();
                List<BaseDropdown> departments = cacheService.GetDepartments();
                List<BaseDropdown> ranks = cacheService.GetRanks();
                ProcessFilterValues(ref input);
                var query = db.StaffRecords
                            .Include(p => p.Employee)
                            .AsNoTracking()
                            .Where(p => !p.IsDeleted)
                             ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                       .Skip(input.SkipCount)
                                       .Take(input.RowsPerPage)
                                       .ProjectToType<StaffRecordList>()
                                       .ToListAsync();
                var creatorIds = items.Select(p => p.CreatorUserId);
                var creators = await db.Users.Where(p => creatorIds.Contains(p.Id)).Select(p => new { p.Id, p.Name }).ToListAsync();
                foreach (var item in items)
                {
                    item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                    item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name ?? item.OtherDepartment;
                    item.CreatorName = creators.FirstOrDefault(p => p.Id == item.CreatorUserId)?.Name;
                }
                return Results.Ok(new PagedResultDto<StaffRecordList>(totalCount, items));
            })
            .RequireAuthorization(StaffRecordPermissions.View)
            ;

            app.MapGet("StaffRecord/GetEmployeeByBrand", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] ApplicationDbContext db) =>
            {
                var result = new List<EmployeeDropdown>();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var brandIds = db.BrandEmployees.Where(p => p.EmployeeId == userId).Select(p => p.BrandId).Distinct();

                var query = from be in db.BrandEmployees
                            join u in db.Users on be.EmployeeId equals u.Id
                            where brandIds.Contains(be.BrandId) /*&& u.UserType == UserType.Employee*/
                            select new StaffRecordDropdown { Id = u.Id, EmployeeCode = u.EmployeeCode, Name = u.Name, DepartmentId= u.DeptId };

                var items = query.ToList().DistinctBy(p => p.Id);
                var depts = cacheService.GetDepartments();                 
                foreach (var item in items)
                {
                    item.DepartmentName = depts.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                }
                return Results.Ok(items);
            })
            .RequireAuthorization(StaffRecordPermissions.View)
            ;

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
            //.RequireAuthorization(StaffRecordPermissions.View)
            ;
        }
    }
}