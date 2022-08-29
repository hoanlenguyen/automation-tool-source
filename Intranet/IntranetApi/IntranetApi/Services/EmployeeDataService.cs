using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
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
            "dd/MM/yyyy hh:mm:ss",
            "dd-MM-yyyy",
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

        private static List<BaseDropdown> GetDataList(string sqlConnectionStr, string tableName)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>($"select Id, Name from {tableName}s where IsDeleted = 0").ToList();
        }

        private static void ProcessFilterValues(ref EmployeeFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }
         
        private static List<BaseDropdown> GetBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Employee where IsDeleted = 0").ToList();
        }

        public static void AddEmployeeDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("employee/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Users.Include(p=>p.BrandEmployees).AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity.Adapt<EmployeeCreateOrEdit>());
            })
            .RequireAuthorization(EmployeePermissions.View)
            ;

            app.MapPost("employee", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager <User> userManager,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (input.EmployeeCode.IsNullOrEmpty())
                    throw new Exception("Missing EmployeeCode!");

                if (input.IntranetPassword.IsNullOrEmpty())
                    throw new Exception("Missing IntranetPassword!");
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<User>();
                entity.CreatorUserId = userId;                 
                var result = await userManager.CreateAsync(entity, entity.IntranetPassword);
                Console.WriteLine($"UserId: {entity.Id}");
                //await db.UserRoles.AddAsync(new UserRole { UserId = entity.Id, RoleId = entity.RoleId });
                return Results.Ok();
            })
            .RequireAuthorization(EmployeePermissions.Create)
            ;

            app.MapPut("employee", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromServices] UserManager<User> userManager,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var user = db.Users.Include(p=>p.BrandEmployees)
                                   .Include(p=>p.UserRoles)
                                   .FirstOrDefault(x => x.Id == input.Id);
                if (user == null)
                    return Results.NotFound();

                Console.WriteLine($"EmployeeCode { input.EmployeeCode}");
                Console.WriteLine($"IntranetPassword { input.IntranetPassword}");

                var currentPassword = user.IntranetPassword;
                user.BrandEmployees.Clear();
                user.UserRoles.Clear();
                input.Adapt(user);
                user.LastModifierUserId = userId;
                user.LastModificationTime = DateTime.Now;
                //user.UserName = user.EmployeeCode;
                user.NormalizedUserName= user.UserName.ToUpperInvariant();
                if (!user.IntranetPassword.Equals(currentPassword)
                                    && user.IntranetPassword.IsNotNullOrEmpty())
                {
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await userManager.ResetPasswordAsync(user, code, user.IntranetPassword);
                }
                
                await db.SaveChangesAsync();
                return Results.Ok();
            })
            //.RequireAuthorization(EmployeePermissions.Update)
            ;

            app.MapDelete("employee/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Users.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(EmployeePermissions.Delete)
            ;

            app.MapPost("employee/list", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ApplicationDbContext db,
            [FromBody] EmployeeFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                List<BaseDropdown> roles = null;
                List<BaseDropdown> banks = null;
                List<BaseDropdown> brands = null;
                List<BaseDropdown> departments = null;
                List<BaseDropdown> ranks = null;
                List<BaseDropdown> adminUsers = null;
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out roles))
                {
                    roles = GetDataList(sqlConnectionStr, nameof(Role));
                    memoryCache.Set(CacheKeys.GetRolesDropdown, roles, cacheOptions);
                }

                if (!memoryCache.TryGetValue(CacheKeys.GetBanksDropdown, out banks))
                {
                    banks = GetDataList(sqlConnectionStr, nameof(Bank));
                    memoryCache.Set(CacheKeys.GetBanksDropdown, banks, cacheOptions);
                }

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

                if (!memoryCache.TryGetValue(CacheKeys.GetAdminUserDropdown, out adminUsers))
                {
                    adminUsers = GetDataList(sqlConnectionStr, nameof(User));
                    memoryCache.Set(CacheKeys.GetAdminUserDropdown, adminUsers, cacheOptions);
                }
                var query = db.Users
                           .Include(p=>p.BrandEmployees)
                           .ThenInclude(p=>p.Brand)
                           .AsNoTracking()
                           .Where(p => !p.IsDeleted && p.UserType== UserType.Employee)
                           .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Name.Contains(input.Keyword))
                           ;
                var totalCount = await query.CountAsync();
                var items = await query.OrderByDynamic(input.SortBy, input.SortDirection)
                                .Skip(input.SkipCount)
                                .Take(input.RowsPerPage)
                                .ProjectToType<EmployeeExcelInput>()
                                .ToListAsync();

                foreach (var item in items)
                {
                    item.Role = roles.FirstOrDefault(p => p.Id == item.RoleId)?.Name;
                    item.Rank = ranks.FirstOrDefault(p => p.Id == item.RankId)?.Name;
                    item.Dept = departments.FirstOrDefault(p => p.Id == item.DeptId)?.Name;
                    item.BankName = banks.FirstOrDefault(p => p.Id == item.BankId)?.Name;
                    item.LastModifierUser = adminUsers.FirstOrDefault(p => p.Id == (item.LastModifierUserId ?? item.CreatorUserId).GetValueOrDefault())?.Name;                    
                }
                return Results.Ok(new PagedResultDto<EmployeeExcelInput>(totalCount, items));
            })
            .RequireAuthorization(EmployeePermissions.View)
            ;

            app.MapPost("employee/importExcel", [Authorize][DisableRequestSizeLimit]
            async Task<IResult> (
                [FromServices] IMemoryCache memoryCache,
                [FromServices] IHttpContextAccessor httpContextAccessor,
                [FromServices] IConfiguration config,
                [FromServices] UserManager<User> userManager,
                [FromServices] ApplicationDbContext db,
                HttpRequest request) =>
            {
                var watch = Stopwatch.StartNew();

                if (!request.Form.Files.Any())
                    throw new Exception("No file found!");

                //var shouldSendEmailWhenReachLimit = config.GetValue<int>("ShouldSendEmailWhenReachLimit");
                var userEmail = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                var userIdSr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdSr, out var userId);
                List<BaseDropdown> roles = null;
                List<BaseDropdown> banks = null;
                List<BaseDropdown> brands = null;
                List<BaseDropdown> departments = null;
                List<BaseDropdown> ranks = null;
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                //if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out roles))
                //{
                //    roles = GetDataList(sqlConnectionStr, nameof(Role));
                //    memoryCache.Set(CacheKeys.GetRolesDropdown, roles, cacheOptions);
                //}
                roles = GetDataList(sqlConnectionStr, nameof(Role));

                if (!memoryCache.TryGetValue(CacheKeys.GetBanksDropdown, out banks))
                {
                    banks = GetDataList(sqlConnectionStr, nameof(Bank));
                    memoryCache.Set(CacheKeys.GetBanksDropdown, banks, cacheOptions);
                }

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
                var totalRows = 0;
                var shouldSendEmail = false;
                var errorList = new List<EmployeeImportError>();
                var formFile = request.Form.Files.FirstOrDefault();
                //foreach (var formFile in request.Form.Files)
                {
                    if (formFile is null || formFile.Length == 0)
                        //continue;
                        throw new Exception("No file found!");
                    var employees = new List<EmployeeBulkInsert>();
                    var brandEmployees = new List<BrandEmployee>();
                    var rowCount = 0;
                    var rowInput = new EmployeeExcelInput();
                    //var rowInputList = new List<EmployeeExcelInput>();
                    //Process excel file
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null) //continue;
                                throw new Exception("No worksheet found!");

                            rowCount = worksheet.Dimension.Rows; //include header
                            if (rowCount <= 1) //continue;
                                throw new Exception("No Data in worksheet found!");

                            totalRows += rowCount - 1;
                            //read excel file data and add data
                            var now = DateTime.Now;
                            var cells = new List<string>();
                            var errorDetails = new List<string>();
                            var brandNames = new List<string>();
                            EmployeeBulkInsert newEmployee;
                            EmployeeImportError importError;
                            //int i = 0;
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
                                rowInput.StartDateStr = (worksheet.Cells[row, 9]?.Text ?? string.Empty).Trim();//I
                                rowInput.SalaryStr = (worksheet.Cells[row, 10]?.Text ?? string.Empty).Trim();//J
                                rowInput.BirthDateStr = (worksheet.Cells[row, 11]?.Text ?? string.Empty).Trim();//K
                                rowInput.IdNumber = (worksheet.Cells[row, 12]?.Text ?? string.Empty).Trim();//L
                                rowInput.Country = (worksheet.Cells[row, 13]?.Text ?? string.Empty).Trim();//M
                                rowInput.BackendUser = (worksheet.Cells[row, 14]?.Text ?? string.Empty).Trim();//N
                                rowInput.BackendPass = (worksheet.Cells[row, 15]?.Text ?? string.Empty).Trim();//O
                                rowInput.Role = (worksheet.Cells[row, 16]?.Text ?? string.Empty).Trim();//P
                                //rowInput.IntranetUsername = (worksheet.Cells[row, 17]?.Text ?? string.Empty).Trim();//Q
                                rowInput.IntranetPassword = (worksheet.Cells[row, 17]?.Text ?? string.Empty).Trim();//Q
                                rowInput.Note = (worksheet.Cells[row, 18]?.Text ?? string.Empty).Trim();//R

                                //i = 0;
                                //check error
                                if (string.IsNullOrEmpty(rowInput.Name))//A
                                {
                                    cells.Add($"A{row}");
                                    errorDetails.Add("Missing Name");
                                }

                                if (string.IsNullOrEmpty(rowInput.EmployeeCode))//B
                                {
                                    cells.Add($"B{row}");
                                    errorDetails.Add("Missing Employee Code");
                                }

                                if (string.IsNullOrEmpty(rowInput.Rank))//C
                                {
                                    cells.Add($"C{row}");
                                    errorDetails.Add("Missing Rank");
                                }
                                else
                                {
                                    Console.WriteLine($"rowInput.Rank {rowInput.Rank}");
                                    rowInput.RankId = ranks.FirstOrDefault(p => p.Name.Equals(rowInput.Rank, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.RankId == null)
                                    {
                                        cells.Add($"C{row}");
                                        errorDetails.Add("Invalid Rank");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Dept))//D
                                {
                                    cells.Add($"D{row}");
                                    errorDetails.Add("Missing Dept");
                                }
                                else
                                {
                                    Console.WriteLine($"rowInput.Dept {rowInput.Dept}");
                                    rowInput.DeptId = departments.FirstOrDefault(p => p.Name.Equals(rowInput.Dept, StringComparison.OrdinalIgnoreCase))?.Id;
                                    Console.WriteLine($"rowInput.DeptId {rowInput.DeptId}");
                                    if (rowInput.DeptId == null)
                                    {
                                        cells.Add($"D{row}");
                                        errorDetails.Add("Invalid Dept");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Brand))//F
                                {
                                    cells.Add($"F{row}");
                                    errorDetails.Add("Missing Brand");
                                }
                                else
                                {
                                    brandNames = rowInput.Brand.Split(',').Select(p => (p?.ToLower() ?? string.Empty).Trim()).ToList();
                                    rowInput.BrandIds = brands.Where(p => brandNames.Contains(p.Name.ToLower())).Select(p => p.Id).ToList();

                                    if (rowInput.BrandIds.Count == 0)
                                    {
                                        cells.Add($"F{row}");
                                        errorDetails.Add("Invalid Brand");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BankName))//G
                                {
                                    cells.Add($"F{row}");
                                    errorDetails.Add("Missing Bank");
                                }
                                else
                                {
                                    rowInput.BankId = banks.FirstOrDefault(p => p.Name.Equals(rowInput.BankName, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.BankId == null)
                                    {
                                        cells.Add($"F{row}");
                                        errorDetails.Add("Invalid Bank");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BankAccountNumber))//H
                                {
                                    cells.Add($"H{row}");
                                    errorDetails.Add("Missing BankAccountNumber");
                                }

                                if (string.IsNullOrEmpty(rowInput.StartDateStr)) //I
                                {
                                    cells.Add($"K{row}");
                                    errorDetails.Add("Missing StartDate");
                                }
                                else
                                {
                                    rowInput.StartDate = CheckValidDate(rowInput.StartDateStr);
                                    if (rowInput.StartDate == null)
                                    {
                                        cells.Add($"K{row}");
                                        errorDetails.Add("Invalid StartDate");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.SalaryStr)) //J
                                {
                                    cells.Add($"J{row}");
                                    errorDetails.Add("Missing Salary");
                                }
                                else
                                {
                                    var salary = 0;
                                    if (!int.TryParse(rowInput.SalaryStr, out salary))
                                    {
                                        cells.Add($"K{row}");
                                        errorDetails.Add("Invalid Salary");
                                    }
                                    else
                                    {
                                        rowInput.Salary = salary;
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BirthDateStr)) //K
                                {
                                    cells.Add($"K{row}");
                                    errorDetails.Add("Missing BirthDate");
                                }
                                else
                                {
                                    rowInput.BirthDate = CheckValidDate(rowInput.BirthDateStr);
                                    if (rowInput.BirthDate == null)
                                    {
                                        cells.Add($"K{row}");
                                        errorDetails.Add("Invalid BirthDate");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.IdNumber)) //L
                                {
                                    cells.Add($"F{row}");
                                    errorDetails.Add("Missing ID Number");
                                }

                                //if (string.IsNullOrEmpty(rowInput.BackendUser))//M
                                //{
                                //    cells.Add($"M{row}");
                                //    errorDetails.Add("Missing BackendUser");
                                //}

                                //if (string.IsNullOrEmpty(rowInput.BackendPass))//N
                                //{
                                //    cells.Add($"N{row}");
                                //    errorDetails.Add("Missing BackendPass");
                                //}

                                if (string.IsNullOrEmpty(rowInput.Role)) //P
                                {
                                    cells.Add($"P{row}");
                                    errorDetails.Add("Missing Role");
                                }
                                else
                                {
                                    Console.WriteLine($"roles {roles.Count}");
                                    Console.WriteLine($"rowInput.Role {rowInput.Role}");
                                    rowInput.RoleId = roles.FirstOrDefault(p => p.Name.Equals(rowInput.Role, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.RoleId == null)
                                    {
                                        cells.Add($"O{row}");
                                        errorDetails.Add("Invalid Role");
                                    }
                                }

                                //if (string.IsNullOrEmpty(rowInput.IntranetUsername))//Q
                                //{
                                //    cells.Add($"Q{row}");
                                //    errorDetails.Add("Missing Intranet Username");
                                //}

                                if (string.IsNullOrEmpty(rowInput.IntranetPassword))//Q
                                {
                                    cells.Add($"R{row}");
                                    errorDetails.Add("Missing Intranet Password");
                                }

                                if (cells.Count == 0) // if no error , add new record
                                {
                                    newEmployee = rowInput.Adapt<EmployeeBulkInsert>();
                                    newEmployee.CreatorUserId = userId;
                                    employees.Add(newEmployee);
                                }
                                else //add error list
                                {
                                    importError = rowInput.Adapt<EmployeeImportError>();
                                    importError.Cells = string.Join(" - ", cells);
                                    importError.ErrorDetails = string.Join(" - ", errorDetails);
                                    errorList.Add(importError);
                                }
                                cells = new List<string>(); //reset after add
                                errorDetails = new List<string>(); //reset after add
                                //rowInputList.Add(rowInput);
                            }
                        }
                    }
                    Console.WriteLine($"employees count {employees.Count}");
                    //var duplicateResult = await CheckUniqueValue(employees);
                    var duplicateResult = new EmployeeCheckUnique();
                    var employeeCodes = employees.Select(p => p.EmployeeCode.ToUpperInvariant());
                    duplicateResult.EmployeeCodes = db.Users.Where(p => employeeCodes.Contains(p.NormalizedUserName))
                                                            .Select(p=>p.EmployeeCode)
                                                            .ToList();
                    var index = 0;
                    //Console.WriteLine($"employees.Count {employees.Count}");
                    //Console.WriteLine($"employees.0 {employees[0]?.EmployeeCode}");
                    //Console.WriteLine($"employees.1 {employees[1]?.EmployeeCode}");
                    foreach (var value in duplicateResult.EmployeeCodes)
                    {
                        index = employees.FindIndex(p => p.EmployeeCode.Equals(value, StringComparison.OrdinalIgnoreCase));
                        Console.WriteLine($"index {index}");
                        if (index > -1)
                        {
                            Console.WriteLine($"duplicateResult.EmployeeCodes {value}");
                            var employee = employees[index].Adapt<EmployeeImportError>();
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
                return Results.Ok(new { totalRows, errorList, shouldSendEmail });
            })
            .RequireAuthorization(EmployeePermissions.Create)
            ;

            app.MapGet("employee/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db) =>
            {
                List<BaseDropdown> items = null;

                items = GetBaseDropdown(sqlConnectionStr);

                return Results.Ok(items);
            });

            app.MapGet("employee/GetByBrand", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var result = new List<EmployeeDropdown>();
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var employeeManager = db.Users
                .Include(p=>p.BrandEmployees.DefaultIfEmpty())
                .FirstOrDefault(p => p.Id == userId)
                ;
                if (employeeManager == null)
                    return Results.Ok(result);
                var brandIds = employeeManager.BrandEmployees.Select(p => p.BrandId).ToList();
                //var brandIds = employeeManager.BrandIds;

                //entity.IsDeleted = true;
                //entity.LastModifierUserId = userId;
                //entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            })
            .RequireAuthorization(EmployeePermissions.View)
            ;

            async Task BulkInsertEmployeesToDB(IEnumerable<EmployeeBulkInsert> items)
            {
                if (items.Count() == 0)
                {
                    Console.WriteLine($"BulkInsertEmployeesToDB -no new data");
                    return;
                }
                var watch = Stopwatch.StartNew();
                var dataTable = items.ToDataTable();
                using var connection = new MySqlConnection(sqlConnectionStr);
                connection.Open();

                var bulkCopy = new MySqlBulkCopy(connection);
                bulkCopy.DestinationTableName = TableName.Employee;
                bulkCopy.BulkCopyTimeout = 0;
                await bulkCopy.WriteToServerAsync(dataTable);

                watch.Stop();
                Console.WriteLine($"Complete BulkInsertCustomerScoreToMySQL: time {watch.Elapsed.TotalSeconds} s");
            }

            async Task<EmployeeCheckUnique> CheckUniqueValue(IEnumerable<EmployeeBulkInsert> items)
            {
                var result = new EmployeeCheckUnique();
                if (items.Count() == 0) return result;
                var watch = Stopwatch.StartNew();
                //var dataTable = items.ToDataTable();
                //using var connection = new MySqlConnection(sqlConnectionStr);
                //connection.Open();
                //var commandStr = $"create temporary table IF NOT EXISTS TempEmployeeList SELECT * FROM users LIMIT 0;";
                //using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                //{
                //    myCmd.CommandType = CommandType.Text;
                //    myCmd.ExecuteNonQuery();
                //}
                //Console.WriteLine($"create temporary table TempEmployeeList");
                //var bulkCopy = new MySqlBulkCopy(connection);
                //bulkCopy.DestinationTableName = "TempEmployeeList";
                //bulkCopy.BulkCopyTimeout = 0;
                //await bulkCopy.WriteToServerAsync(dataTable);
                //watch.Stop();
                //Console.WriteLine($"WriteToServerAsync TempEmployeeList time: {watch.Elapsed.TotalSeconds}");
                //watch.Restart();
                //commandStr = "select t.EmployeeCode " +
                //             "from TempEmployeeList  t " +
                //             "inner join Employee c " +
                //             "on t.EmployeeCode = c.EmployeeCode ";
                //result.EmployeeCodes = connection.Query<string>(commandStr).ToList();
                //watch.Stop();

                //watch.Restart();
                //commandStr = "select t.BackendUser " +
                //             "from TempEmployeeList  t " +
                //             "inner join Employee c " +
                //             "on t.BackendUser = c.BackendUser ";
                //result.BackendUsers = connection.Query<string>(commandStr).ToList();
                //Console.WriteLine($"query join table TempEmployeeList - BackendUser, time: {watch.Elapsed.TotalSeconds}");

                //watch.Restart();
                //commandStr = "select t.IdNumber " +
                //             "from TempEmployeeList  t " +
                //             "inner join Employee c " +
                //             "on t.IdNumber = c.IdNumber ";
                //result.IdNumbers = connection.Query<string>(commandStr).ToList();
                //Console.WriteLine($"query join table TempEmployeeList - IdNumber, time: {watch.Elapsed.TotalSeconds}"); 

                //watch.Restart();
                //commandStr = "select t.IntranetUsername " +
                //             "from TempEmployeeList  t " +
                //             "inner join Employee c " +
                //             "on t.IntranetUsername = c.IntranetUsername ";
                //result.IntranetUsernames = connection.Query<string>(commandStr).ToList();
                //Console.WriteLine($"query join table TempEmployeeList - IntranetUsername, time: {watch.Elapsed.TotalSeconds}");

                //commandStr = "DROP TEMPORARY TABLE IF EXISTS TempEmployeeList";
                //using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                //{
                //    myCmd.CommandType = CommandType.Text;
                //    myCmd.ExecuteNonQuery();
                //}
                return result;
            }


        }
    }
}