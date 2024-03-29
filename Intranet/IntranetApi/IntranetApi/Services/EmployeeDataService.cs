﻿using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace IntranetApi.Services
{
    public static class EmployeeDataService
    {
        private static readonly IReadOnlyList<string> Valid_DateTime_Formats = new List<string>
        {
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

        private static void ProcessFilterValues(ref EmployeeFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        public static void AddEmployeeDataService(this WebApplication app)
        {
            app.MapGet("employee/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Users.Include(p => p.BrandEmployees).AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity.Adapt<EmployeeCreateOrEdit>());
            })
            .RequireAuthorization(EmployeePermissions.View)
            ;

            app.MapPost("employee", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<User> userManager,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                if (string.IsNullOrEmpty(input.EmployeeCode))
                    throw new Exception("No valid Employee code!");

                if (input.RoleId == 0)
                    throw new Exception("Missing Intranet Role!");

                var checkExisted = await db.Users.AnyAsync(p => p.EmployeeCode == input.EmployeeCode && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception($"{input.EmployeeCode} existed!");

                if (input.EmployeeCode.IsNullOrEmpty())
                    throw new Exception("Missing EmployeeCode!");

                if (input.IntranetPassword.IsNullOrEmpty())
                    throw new Exception("Missing Intranet Password!");

                var entity = input.Adapt<User>();
                entity.CreatorUserId = loggedUser.Id;
                entity.IsFirstTimeLogin = true;
                var result = await userManager.CreateAsync(entity, entity.IntranetPassword);
                Console.WriteLine($"UserId: {entity.Id}");
                return Results.Ok();
            })
            .RequireAuthorization(EmployeePermissions.Create)
            ;

            app.MapPut("employee", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<User> userManager,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception("No valid name!");

                if (string.IsNullOrEmpty(input.EmployeeCode))
                    throw new Exception("No valid Employee code!");

                var checkExisted = await db.Users.AnyAsync(p => p.EmployeeCode == input.EmployeeCode && p.Id != input.Id && !p.IsDeleted);
                if (checkExisted)
                    throw new Exception($"Employee code already exists");

                var user = db.Users.Include(p => p.BrandEmployees)
                                   .Include(p => p.UserRoles)
                                   .FirstOrDefault(x => x.Id == input.Id);
                if (user == null)
                    return Results.NotFound();

                var currentPassword = user.IntranetPassword;
                user.BrandEmployees.Clear();
                user.UserRoles.Clear();
                input.Adapt(user);
                user.LastModifierUserId = loggedUser.Id;
                user.LastModificationTime = DateTime.UtcNow.AddHours(1);
                user.NormalizedUserName = user.UserName.ToUpperInvariant();
                if (!user.IntranetPassword.Equals(currentPassword)
                                    && user.IntranetPassword.IsNotNullOrEmpty())
                {
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await userManager.ResetPasswordAsync(user, code, user.IntranetPassword);
                }

                await db.SaveChangesAsync();
                return Results.Ok();
            })
            .RequireAuthorization(EmployeePermissions.Update)
            ;

            app.MapDelete("employee/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IUserPrincipal loggedUser,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Users.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = loggedUser.Id;
                entity.LastModificationTime = DateTime.UtcNow.AddHours(1);
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(EmployeePermissions.Delete)
            ;

            app.MapPost("employee/list", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCacheService cacheService,
            [FromServices] ApplicationDbContext db,
            [FromBody] EmployeeFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                List<BaseDropdown> banks = cacheService.GetBanks();
                List<BaseDropdown> brands = cacheService.GetBrands();
                List<BaseDropdown> departments = cacheService.GetDepartments();
                List<BaseDropdown> ranks = cacheService.GetRanks();
                List<CurrencySimpleDto> currencies = cacheService.GetCurrencies();
                List<BaseDropdown> adminUsers = null;

                var query = db.Users
                           .Include(p => p.BrandEmployees)
                           .AsNoTracking()
                           .Where(p => !p.IsDeleted && p.UserType == UserType.Employee)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                .Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ProjectToType<EmployeeExcelInput>()
                                .ToListAsync();
                var adminUserIds = items.Select(p => p.CreatorUserId).ToList();
                adminUserIds.AddRange(items.Select(p => p.LastModifierUserId).ToList());
                adminUserIds = adminUserIds.Where(p => p != null).Distinct().ToList();
                adminUsers = await db.Users.Where(p => adminUserIds.Contains(p.Id)).Select(p => new BaseDropdown { Id = p.Id, Name = p.Name }).ToListAsync();
                foreach (var item in items)
                {
                    item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                    item.Dept = departments.FirstOrDefault(p => p.Id == item.DeptId)?.Name;
                    item.BankName = banks.FirstOrDefault(p => p.Id == item.BankId)?.Name;
                    item.Brands = brands.Where(p => item.BrandIds.Contains(p.Id)).Select(p => p.Name);
                    item.LastModifierUser = adminUsers.FirstOrDefault(p => p.Id == (item.LastModifierUserId ?? item.CreatorUserId).GetValueOrDefault())?.Name;
                    item.CurrencySymbol = currencies.FirstOrDefault(p => p.Name.Equals(item.Country, StringComparison.OrdinalIgnoreCase))?.CurrencySymbol;
                }
                return Results.Ok(new PagedResultDto<EmployeeExcelInput>(totalCount, items));
            })
            .RequireAuthorization(EmployeePermissions.View)
            ;

            app.MapPost("employee/importExcel", [Authorize][DisableRequestSizeLimit]
            async Task<IResult> (
                [FromServices] IMemoryCacheService cacheService,
                [FromServices] IUserPrincipal loggedUser,
                [FromServices] IConfiguration config,
                [FromServices] UserManager<User> userManager,
                [FromServices] ApplicationDbContext db,
                HttpRequest request) =>
            {
                var watch = Stopwatch.StartNew();

                if (!request.Form.Files.Any())
                    throw new Exception("No file found!");

                var userId = loggedUser.Id;
                List<BaseDropdown> roles = cacheService.GetRoles();
                List<BaseDropdown> banks = cacheService.GetBanks();
                List<BaseDropdown> brands = cacheService.GetBrands();
                List<BaseDropdown> departments = cacheService.GetDepartments();
                List<BaseDropdown> ranks = cacheService.GetRanks();
                var totalRows = 0;
                var rowInputList = new List<EmployeeExcelInput>();
                var shouldSendEmail = false;
                var errorList = new List<EmployeeImportError>();
                var formFile = request.Form.Files.FirstOrDefault();
                //foreach (var formFile in request.Form.Files)
                {
                    if (formFile is null || formFile.Length == 0)
                        throw new Exception("No file found!");
                    var employees = new List<EmployeeBulkInsert>();
                    var brandEmployees = new List<BrandEmployee>();
                    var rowCount = 0;
                    var rowInput = new EmployeeExcelInput();
                    //Process excel file
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null)
                                throw new Exception("No worksheet found!");

                            rowCount = worksheet.Dimension.Rows;
                            if (rowCount <= 1)
                                throw new Exception("No Data in worksheet found!");

                            totalRows += rowCount - 1;
                            //read excel file data and add data
                            var now = DateTime.UtcNow.AddHours(1);
                            var errorCells = new List<string>();
                            var errorDetails = new List<string>();
                            var brandNames = new List<string>();
                            EmployeeBulkInsert newEmployee;
                            EmployeeImportError importError;
                            worksheet.Columns[10].Style.Numberformat.Format= "@";
                            worksheet.Columns[12].Style.Numberformat.Format= "@";
                            for (int row = 2; row <= rowCount; row++)
                            {
                                rowInput.Name = (worksheet.Cells[row, 1]?.Text ?? string.Empty).Trim();//A
                                rowInput.EmployeeCode = (worksheet.Cells[row, 2]?.Text ?? string.Empty).Trim();//B
                                rowInput.Rank = (worksheet.Cells[row, 3]?.Text ?? string.Empty).Trim();//C
                                rowInput.Dept = (worksheet.Cells[row, 4]?.Text ?? string.Empty).Trim();//D
                                rowInput.StatusStr = (worksheet.Cells[row, 5]?.Text ?? string.Empty).Trim();//E
                                rowInput.Brand = (worksheet.Cells[row, 6]?.Text ?? string.Empty).Trim();//F
                                rowInput.BankName = (worksheet.Cells[row, 7]?.Text ?? string.Empty).Trim();//G
                                rowInput.BankAccountNumber = (worksheet.Cells[row, 8]?.Text ?? string.Empty).Trim();//H
                                rowInput.BankAccountName = (worksheet.Cells[row, 9]?.Text ?? string.Empty).Trim();//I
                                rowInput.StartDateStr = (worksheet.Cells[row, 10]?.Text ?? string.Empty).Trim();//J
                                rowInput.SalaryStr = (worksheet.Cells[row, 11]?.Text ?? string.Empty).Trim();//K
                                rowInput.BirthDateStr = (worksheet.Cells[row, 12]?.Text ?? string.Empty).Trim();//L
                                rowInput.IdNumber = (worksheet.Cells[row, 13]?.Text ?? string.Empty).Trim();//M
                                rowInput.Country = (worksheet.Cells[row, 14]?.Text ?? string.Empty).Trim();//N
                                rowInput.BackendUser = (worksheet.Cells[row, 15]?.Text ?? string.Empty).Trim();//O
                                rowInput.BackendPass = (worksheet.Cells[row, 16]?.Text ?? string.Empty).Trim();//P
                                rowInput.Role = (worksheet.Cells[row, 17]?.Text ?? string.Empty).Trim();//Q
                                rowInput.IntranetPassword = (worksheet.Cells[row, 18]?.Text ?? string.Empty).Trim();//R
                                rowInput.Note = (worksheet.Cells[row, 19]?.Text ?? string.Empty).Trim();//S

                                if (rowInput.Name.IsNullOrEmpty() && rowInput.EmployeeCode.IsNullOrEmpty() && rowInput.Rank.IsNullOrEmpty())
                                    continue;
                                //check error
                                if (string.IsNullOrEmpty(rowInput.Name))//A
                                {
                                    errorCells.Add($"A{row}");
                                    errorDetails.Add("Missing Name");
                                }

                                if (string.IsNullOrEmpty(rowInput.EmployeeCode))//B
                                {
                                    errorCells.Add($"B{row}");
                                    errorDetails.Add("Missing Employee Code");
                                }

                                if (string.IsNullOrEmpty(rowInput.Rank))//C
                                {
                                    errorCells.Add($"C{row}");
                                    errorDetails.Add("Missing Rank");
                                }
                                else
                                {
                                    rowInput.RankId = ranks.FirstOrDefault(p => p.Name.Equals(rowInput.Rank, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.RankId == null)
                                    {
                                        errorCells.Add($"C{row}");
                                        errorDetails.Add("Invalid Rank");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Dept))//D
                                {
                                    errorCells.Add($"D{row}");
                                    errorDetails.Add("Missing Dept");
                                }
                                else
                                {
                                    rowInput.DeptId = departments.FirstOrDefault(p => p.Name.Equals(rowInput.Dept, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.DeptId == null)
                                    {
                                        errorCells.Add($"D{row}");
                                        errorDetails.Add("Invalid Dept");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Brand))//F
                                {
                                    errorCells.Add($"F{row}");
                                    errorDetails.Add("Missing Brand");
                                }
                                else
                                {
                                    brandNames = rowInput.Brand.Split(',').Select(p => (p?.ToLower() ?? string.Empty).Trim()).ToList();
                                    rowInput.BrandIds = brands.Where(p => brandNames.Contains(p.Name.ToLower())).Select(p => p.Id).ToList();

                                    if (rowInput.BrandIds.Count == 0)
                                    {
                                        errorCells.Add($"F{row}");
                                        errorDetails.Add("Invalid Brand");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BankName))//G
                                {
                                    errorCells.Add($"G{row}");
                                    errorDetails.Add("Missing Bank");
                                }
                                else
                                {
                                    rowInput.BankId = banks.FirstOrDefault(p => p.Name.Equals(rowInput.BankName, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.BankId == null)
                                    {
                                        errorCells.Add($"G{row}");
                                        errorDetails.Add("Invalid Bank");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BankAccountNumber))//H
                                {
                                    errorCells.Add($"H{row}");
                                    errorDetails.Add("Missing BankAccountNumber");
                                }

                                if (string.IsNullOrEmpty(rowInput.StartDateStr)) //J
                                {
                                    errorCells.Add($"J{row}");
                                    errorDetails.Add("Missing StartDate");
                                }
                                else
                                {
                                    rowInput.StartDate = CheckValidDate(rowInput.StartDateStr);
                                    if (rowInput.StartDate == null)
                                    {
                                        errorCells.Add($"J{row}");
                                        errorDetails.Add("Invalid StartDate");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.SalaryStr)) //K
                                {
                                    errorCells.Add($"K{row}");
                                    errorDetails.Add("Missing Salary");
                                }
                                else
                                {
                                    var salary = 0;
                                    if (!int.TryParse(rowInput.SalaryStr, out salary))
                                    {
                                        errorCells.Add($"K{row}");
                                        errorDetails.Add("Invalid Salary");
                                    }
                                    else
                                    {
                                        rowInput.Salary = salary;
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BirthDateStr)) //L
                                {
                                    errorCells.Add($"L{row}");
                                    errorDetails.Add("Missing BirthDate");
                                }
                                else
                                {
                                    rowInput.BirthDate = CheckValidDate(rowInput.BirthDateStr);
                                    if (rowInput.BirthDate == null)
                                    {
                                        errorCells.Add($"L{row}");
                                        errorDetails.Add("Invalid BirthDate");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Role)) //Q
                                {
                                    errorCells.Add($"Q{row}");
                                    errorDetails.Add("Missing Role");
                                }
                                else
                                {
                                    rowInput.RoleId = roles.FirstOrDefault(p => p.Name.Equals(rowInput.Role, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.RoleId == null)
                                    {
                                        errorCells.Add($"Q{row}");
                                        errorDetails.Add("Invalid Role");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.IntranetPassword))//R
                                {
                                    errorCells.Add($"R{row}");
                                    errorDetails.Add("Missing Intranet Password");
                                }

                                if (errorCells.Count == 0) // if no error , add new record
                                {
                                    newEmployee = rowInput.Adapt<EmployeeBulkInsert>();
                                    newEmployee.CreatorUserId = userId;
                                    employees.Add(newEmployee);
                                }
                                else //add error list
                                {
                                    importError = rowInput.Adapt<EmployeeImportError>();
                                    importError.Cells = string.Join(" - ", errorCells);
                                    importError.ErrorDetails = string.Join(" - ", errorDetails);
                                    errorList.Add(importError);
                                }
                                errorCells = new List<string>(); //reset after add
                                errorDetails = new List<string>(); //reset after add
                                rowInputList.Add(rowInput.Adapt<EmployeeExcelInput>());
                            }
                        }
                    }
                    Console.WriteLine($"employees count {employees.Count}");
                    var duplicateResult = new EmployeeCheckUnique();
                    var employeeCodes = employees.Select(p => p.EmployeeCode.ToUpperInvariant());
                    duplicateResult.EmployeeCodes = db.Users.Where(p => employeeCodes.Contains(p.NormalizedUserName))
                                                            .Select(p => p.EmployeeCode)
                                                            .ToList();
                    var index = 0;
                    foreach (var value in duplicateResult.EmployeeCodes)
                    {
                        Console.WriteLine($"value {value}");
                        var item = rowInputList.FirstOrDefault(p => p.EmployeeCode.Equals(value, StringComparison.OrdinalIgnoreCase));
                        if (item != null)
                        {
                            Console.WriteLine($"duplicateResult.EmployeeCodes {value}");
                            var employee = item.Adapt<EmployeeImportError>();
                            employee.Cells = $"B{index + 1}";
                            employee.ErrorDetails = "EmployeeCode existed";
                            errorList.Add(employee);
                        }
                    }

                    employees = employees.Where(
                        p => !duplicateResult.EmployeeCodes.Contains(p.EmployeeCode)
                        ).ToList();
                    var userRoles = new List<UserRole>();
                    foreach (var item in employees)
                    {
                        var user = item.Adapt<User>();
                        user.IsFirstTimeLogin = true;
                        var result = await userManager.CreateAsync(user, item.IntranetPassword);
                        item.UserId = user.Id;
                        userRoles.Add(new UserRole { RoleId = item.RoleId, UserId = user.Id });
                    }

                    if (userRoles.Any())
                    {
                        await db.UserRoles.AddRangeAsync(userRoles);
                        await db.SaveChangesAsync();
                    }
                    watch.Stop();
                    Console.WriteLine($"Complete Import data: time {watch.Elapsed.TotalSeconds} s");
                }
                return Results.Ok(new { totalRows = rowInputList.Count, errorList, shouldSendEmail });
            })
            .RequireAuthorization(EmployeePermissions.Create)
            ;

            app.MapGet("employee/getRelatedData", [Authorize]
            async Task<IResult> (
            [FromServices] IMemoryCacheService cacheService) =>
            {
                return Results.Ok(new
                {
                    roles = cacheService.GetRoles(),
                    departments = cacheService.GetDepartments(),
                    banks = cacheService.GetBanks(),
                    brands = cacheService.GetBrands(),
                    ranks = cacheService.GetRanks()
                });
            });
        }
    }
}