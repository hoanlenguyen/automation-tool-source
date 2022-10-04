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
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace IntranetApi.Services
{
    public static class LeaveHistoryService
    {
        private static readonly IReadOnlyList<string> Valid_DateTime_Formats = new List<string>
        {
            "MM/yyyy",
            "MM-yyyy",
            "MMM/yyyy",
            "MMM-yyyy",
            "dd/MM/yyyy",
            "d/MM/yyyy",
            "dd/M/yyyy",
            "d/M/yyyy",
            "dd/MM/yyyy hh:mm:ss",
            "dd-MM-yyyy",
            "d-MM-yyyy",
            "dd-M-yyyy",
            "d-M-yyyy",
            "dd-MM-yyyy hh:mm:ss",
            "dd/MMM/yyyy",
            "dd-MMM-yyyy"
        };

        private static DateTime? CheckValidDate(string input)
        {
            DateTime result;
            foreach (var format in Valid_DateTime_Formats)
            {
                if (DateTime.TryParseExact(input, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                    return result;
            }
            return null;
        }

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

                if (input.FromTime != null)
                    input.FromTime = input.FromTime.Value.Date;

                if (input.ToTime != null)
                    input.ToTime = input.ToTime.Value.Date.AddDays(1).AddTicks(-1);

                var isSuperAdmin = await db.UserRoles
                                .Include(p => p.Role)
                                .AnyAsync(p => p.UserId == userId && p.Role.IsSuperAddmin && !p.Role.IsDeleted);

                if (isSuperAdmin)
                {
                    var isAllBrand = input.BrandId != null ? await db.Brands
                               .AnyAsync(p => p.Id == input.BrandId && p.IsAllBrand) : true;

                    totalCount = await db.StaffRecords
                            .Include(p => p.Employee)
                            .Where(p => !p.IsDeleted && p.Employee.UserType == UserType.Employee)
                            .WhereIf(input.FromTime != null, p => p.CreationTime >= input.FromTime.Value)
                            .WhereIf(input.ToTime != null, p => p.CreationTime <= input.ToTime.Value)
                            .WhereIf(!isAllBrand, p => p.Employee.BrandEmployees.Any(p => p.BrandId == input.BrandId))
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
                            .WhereIf(!isAllBrand, p => p.Employee.BrandEmployees.Any(p => p.BrandId == input.BrandId))
                            .WhereIf(input.DepartmentId != null, p => p.Employee.DeptId == input.DepartmentId)
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
                                SumCalculationAmount = p.Sum(p => p.CalculationAmount),
                                Fines = p.Sum(p => p.Fine)
                            });

                    items = query.Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ToList();

                    //add import leave history by excel
                    var importLeaveHistories = db.LeaveHistories
                                        .Include(p => p.Employee)
                                        .ThenInclude(q => q.BrandEmployees)
                                        .AsNoTracking()
                                        .WhereIf(input.FromTime != null, p => p.ImportedDate >= input.FromTime.Value)
                                        .WhereIf(input.ToTime != null, p => p.ImportedDate <= input.ToTime.Value)
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
                                            SumDaysOfPaidOffs = p.Sum(q => q.PaidOffs),
                                            SumDaysOfPaidMCs = p.Sum(q => q.PaidMCs),
                                            SumHoursOfDeduction = p.Sum(q => q.Late)
                                        })
                                        .ToList();
                    //Console.WriteLine($"FromTime {input.FromTime.Value.ToString("yyyy-MM-dd hh:mm:ss")}");
                    //Console.WriteLine($"ToTime {input.ToTime.Value.ToString("yyyy-MM-dd hh:mm:ss")}");
                    //Console.WriteLine($"importLeaveHistories {importLeaveHistories.Count}");
                    if (importLeaveHistories.Any())
                    {
                        foreach (var item in importLeaveHistories)
                        {
                            item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                            item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                            item.CurrencySymbol = currencies.FirstOrDefault(p => p.Name.Equals(item.Country, StringComparison.OrdinalIgnoreCase))?.CurrencySymbol;
                            item.Brands = brands.Where(p => item.BrandEmployees.Contains(p.Id)).Select(p => p.Name);
                        }
                        totalCount += importLeaveHistories.Count;
                    }


                    foreach (var item in items)
                    {
                        item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                        item.Department = departments.FirstOrDefault(p => p.Id == item.DepartmentId)?.Name;
                        item.Brands = brands.Where(p => item.BrandEmployees.Contains(p.Id)).Select(p => p.Name);
                        item.CurrencySymbol = currencies.FirstOrDefault(p => p.Name.Equals(item.Country, StringComparison.OrdinalIgnoreCase))?.CurrencySymbol;
                        var leaveHistory = importLeaveHistories.FirstOrDefault(p=>p.EmployeeId == item.EmployeeId);
                        if (leaveHistory != null)
                        {
                            item.SumDaysOfPaidOffs += leaveHistory.SumDaysOfPaidOffs;
                            item.SumDaysOfPaidMCs += leaveHistory.SumDaysOfPaidMCs;
                            item.SumHoursOfDeduction += leaveHistory.SumHoursOfDeduction;
                        }
                    }

                    var existEmployeeIds = items.Select(p => p.EmployeeId);
                    items.AddRange(importLeaveHistories.Where(p => !existEmployeeIds.Contains(p.EmployeeId)));
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
                                Country = p.FirstOrDefault().Country,
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
                                .Select(p => new { UserId = userId, RoleId = p.RoleId, IsSuperAddmin = p.Role.IsSuperAddmin })
                                .FirstOrDefaultAsync();

                if (role == null || role.RoleId == 0)
                    return Results.Ok();

                if (role.IsSuperAddmin)
                {
                    return Results.Ok(new
                    {
                        brands = cacheService.GetBrands(),
                        departments = cacheService.GetDepartments()
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

                return Results.Ok(new { brands, departments });
            });

            app.MapPost("LeaveHistory/importExcel", [Authorize][DisableRequestSizeLimit]
            async Task<IResult> (
                [FromServices] IMemoryCacheService cacheService,
                [FromServices] IHttpContextAccessor httpContextAccessor,
                [FromServices] IConfiguration config,
                [FromServices] ApplicationDbContext db,
                HttpRequest request) =>
            {
                var watch = Stopwatch.StartNew();

                if (!request.Form.Files.Any())
                    throw new Exception("No file found!");

                var userEmail = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                var userIdSr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdSr, out var userId);

                var totalRows = 0;
                var inputList = new List<LeaveHistoryImportDto>();
                var leaveHistories = new List<LeaveHistory>();

                //var shouldSendEmail = false;
                var formFile = request.Form.Files.FirstOrDefault();
                //foreach (var formFile in request.Form.Files)
                //{
                if (formFile is null || formFile.Length == 0)
                    throw new Exception("No file found!");

                var rowCount = 0;
                //Process excel file
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            throw new Exception("No worksheet found!");

                        rowCount = worksheet.Dimension.Rows; //include header
                        if (rowCount <= 2)
                            throw new Exception("No Data in worksheet found!");

                        //read excel file data and add data
                        float parsedValue = 0;
                        var errorCells = new List<string>();
                        var errorDetails = new List<string>();
                        var importMonthStr = (worksheet.Cells[1, 2]?.Text ?? string.Empty).Trim();
                        var importMonth = CheckValidDate(importMonthStr);
                        if (importMonth is null)
                            throw new Exception("Import month not found!");

                        for (int row = 3; row <= rowCount; row++)
                        {
                            var rowInput = new LeaveHistoryImportDto() { ImportedDate = importMonth.GetValueOrDefault(), CreatorUserId = userId };
                            rowInput.EmployeeCode = (worksheet.Cells[row, 1]?.Text ?? string.Empty).Trim();//A
                            rowInput.PaidMCsStr = (worksheet.Cells[row, 2]?.Text ?? string.Empty).Trim();//B
                            rowInput.PaidOffsStr = (worksheet.Cells[row, 3]?.Text ?? string.Empty).Trim();//C
                            rowInput.LateStr = (worksheet.Cells[row, 4]?.Text ?? string.Empty).Trim();//D

                            if (rowInput.EmployeeCode.IsNullOrEmpty()
                                && rowInput.PaidMCsStr.IsNullOrEmpty()
                                && rowInput.PaidOffsStr.IsNullOrEmpty()
                                && rowInput.LateStr.IsNullOrEmpty())
                                continue; //not clear cell

                            //check error
                            if (string.IsNullOrEmpty(rowInput.EmployeeCode))//A
                            {
                                errorCells.Add($"A{row}");
                                errorDetails.Add("Missing Employee Code");
                            }

                            if (string.IsNullOrEmpty(rowInput.PaidMCsStr))//B
                            {
                                errorCells.Add($"B{row}");
                                errorDetails.Add("Missing PaidMCs");
                            }
                            else
                            {
                                if (float.TryParse(rowInput.PaidMCsStr, out parsedValue))
                                {
                                    rowInput.PaidMCs = parsedValue;
                                }
                                else
                                {
                                    errorCells.Add($"B{row}");
                                    errorDetails.Add("Invalid PaidMCs");
                                }
                            }

                            if (string.IsNullOrEmpty(rowInput.PaidOffsStr))//C
                            {
                                errorCells.Add($"C{row}");
                                errorDetails.Add("Missing PaidOffs");
                            }
                            else
                            {
                                if (float.TryParse(rowInput.PaidOffsStr, out parsedValue))
                                {
                                    rowInput.PaidMCs = parsedValue;
                                }
                                else
                                {
                                    errorCells.Add($"C{row}");
                                    errorDetails.Add("Invalid PaidOff");
                                }
                            }

                            if (string.IsNullOrEmpty(rowInput.LateStr))//D
                            {
                                errorCells.Add($"D{row}");
                                errorDetails.Add("Missing Late");
                            }
                            else
                            {
                                if (float.TryParse(rowInput.LateStr, out parsedValue))
                                {
                                    rowInput.Late = parsedValue;
                                }
                                else
                                {
                                    errorCells.Add($"D{row}");
                                    errorDetails.Add("Invalid Late");
                                }
                            }

                            if (errorDetails.Any())
                            {
                                rowInput.ErrorCells = string.Join('-', errorCells);
                                rowInput.ErrorDetails = string.Join('-', errorDetails);
                            }
                            inputList.Add(rowInput);
                            errorCells = new List<string>(); //reset after add
                            errorDetails = new List<string>(); //reset after add
                        }
                    }
                }
                Console.WriteLine($"inputList count {inputList.Count}");
                var employeeCodes = inputList.Select(p => p.EmployeeCode);
                var employees = await db.Users.Where(p => employeeCodes.Contains(p.EmployeeCode) && !p.IsDeleted)
                                       .Select(p => new EmployeeSimpleDto { Id = p.Id, EmployeeCode = p.EmployeeCode })
                                       .ToListAsync();

                foreach (var item in inputList)
                {
                    item.EmployeeId = employees.FirstOrDefault(p => p.EmployeeCode.Equals(item.EmployeeCode, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                    if (item.EmployeeId == 0)
                    {
                        item.ErrorDetails += $"- No employee found with EmployeeCode: {item.EmployeeCode}";
                    }
                    if (item.ErrorDetails.IsNullOrEmpty())
                        leaveHistories.Add(item.Adapt<LeaveHistory>());
                }
                await db.LeaveHistories.AddRangeAsync(leaveHistories);
                await db.SaveChangesAsync();
                watch.Stop();
                Console.WriteLine($"Complete Import data: time {watch.Elapsed.TotalSeconds} s");
                //}
                return Results.Ok(new { totalRows = inputList.Count, errorList = inputList.Where(p => p.ErrorDetails.IsNotNullOrEmpty()), /*shouldSendEmail= false*/ });
            })
            .RequireAuthorization(LeaveHistoryPermissions.Create)
            ;
        }
    }
}