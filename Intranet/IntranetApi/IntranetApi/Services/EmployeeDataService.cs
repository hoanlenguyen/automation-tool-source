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
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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

        private static List<BaseDropdown> GetRoleList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from UserRole").ToList();
        }

        private static List<BaseDropdown> GetBankList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Bank where IsDeleted = 0").ToList();
        }

        private static List<BaseDropdown> GetBrandList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Brand where IsDeleted = 0").ToList();
        }

        private static List<BaseDropdown> GetDepartmentList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Department where IsDeleted = 0").ToList();
        }

        private static void ProcessFilterValues(ref EmployeeFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        private static int GetTotalCountByFilter(string sqlConnectionStr, ref EmployeeFilterDto input)
        {
            using (var conn = new MySqlConnection(sqlConnectionStr))
            {
                conn.Open();
                var cmd = new MySqlCommand(StoredProcedureName.GetEmployeeTotal, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@keyword", input.Keyword);
                cmd.Parameters.AddWithValue("@status", input.Status);

                MySqlDataReader rdr = cmd.ExecuteReader();
                int count = 0;
                while (rdr.Read())
                    count = (int)rdr.GetInt64(0);
                rdr.Close();
                conn.Close();
                return count;
            }
        }

        private static List<BaseDropdown> GetBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Name from Employee where IsDeleted = 0").ToList();
        }

        private static List<BaseDropdown> GetAdminBaseDropdown(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, Email as 'Name' from User where IsDeleted = 0").ToList();
        }


        public static void AddEmployeeDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("employee/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Employee.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            });

            app.MapPost("employee", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = input.Adapt<Employee>();
                entity.CreatorUserId = userId;
                db.Add(entity);
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapPut("employee", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Employee.FirstOrDefault(x => x.Id == input.Id);
                if (entity == null)
                    return Results.NotFound();

                Console.WriteLine($"BirthDate {input.BirthDate}");
                input.Adapt(entity);
                Console.WriteLine($"BirthDate {entity.BirthDate}");
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapDelete("employee/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Employee.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapPost("employee/list", [AllowAnonymous]
            async Task<IResult> (
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ApplicationDbContext db,
            [FromBody] EmployeeFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var totalCount = GetTotalCountByFilter(sqlConnectionStr, ref input);
                using (var conn = new MySqlConnection(sqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(StoredProcedureName.GetEmployeeList, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyword", input.Keyword);
                    cmd.Parameters.AddWithValue("@status", input.Status);
                    cmd.Parameters.AddWithValue("@sortBy", input.SortBy);
                    cmd.Parameters.AddWithValue("@sortDirection", input.SortDirection);

                    cmd.Parameters.AddWithValue("@exportOffset", input.SkipCount);
                    cmd.Parameters.AddWithValue("@exportLimit", input.RowsPerPage);

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    var parser = rdr.GetRowParser<EmployeeExcelInput>(typeof(EmployeeExcelInput));
                    var items = new List<EmployeeExcelInput>();
                    while (rdr.Read())
                    {
                        items.Add(parser(rdr));                         
                    }
                    rdr.Close();
                    conn.Close();
                    List<BaseDropdown> roles = null;
                    List<BaseDropdown> banks = null;
                    List<BaseDropdown> brands = null;
                    List<BaseDropdown> departments = null;
                    var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                    if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out roles))
                    {
                        roles = GetRoleList(sqlConnectionStr);
                        memoryCache.Set(CacheKeys.GetRolesDropdown, roles, cacheOptions);
                    }

                    if (!memoryCache.TryGetValue(CacheKeys.GetBanksDropdown, out banks))
                    {
                        banks = GetBankList(sqlConnectionStr);
                        memoryCache.Set(CacheKeys.GetBanksDropdown, banks, cacheOptions);
                    }

                    if (!memoryCache.TryGetValue(CacheKeys.GetBrandsDropdown, out brands))
                    {
                        brands = GetBrandList(sqlConnectionStr);
                        memoryCache.Set(CacheKeys.GetBrandsDropdown, brands, cacheOptions);
                    }

                    if (!memoryCache.TryGetValue(CacheKeys.GetDepartmentsDropdown, out departments))
                    {
                        departments = GetDepartmentList(sqlConnectionStr);
                        memoryCache.Set(CacheKeys.GetDepartmentsDropdown, departments, cacheOptions);
                    }
                    var employeeIds = items.Select(p => p.Id);
                    foreach (var item in items)
                    {
                        //Console.WriteLine($"item.BrandIds {item.BrandIds}");
                        item.Role = roles.FirstOrDefault(p => p.Id == item.RoleId)?.Name;
                        item.Dept = departments.FirstOrDefault(p => p.Id == item.DeptId)?.Name;
                        item.BankName = banks.FirstOrDefault(p => p.Id == item.BankId)?.Name;
                        if(!item.BrandIds.Equals(BrandValue.AllBrands, StringComparison.OrdinalIgnoreCase))
                        {
                            item.BrandIdList = item.BrandIds.Split(',').Select(p=>int.Parse(p)).ToList();
                            item.Brand = string.Join(", ", brands.Where(p => item.BrandIdList.Contains(p.Id)).Select(p => p.Name));
                        }                       
                    }
                    return Results.Ok(new PagedResultDto<EmployeeExcelInput>(totalCount, items));
                }
            });

            app.MapPost("employee/importExcel", [Authorize][DisableRequestSizeLimit]
            async Task<IResult> (
                [FromServices] IMemoryCache memoryCache,
                [FromServices] IHttpContextAccessor httpContextAccessor,
                [FromServices] IConfiguration config,
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
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out roles))
                {
                    roles = GetRoleList(sqlConnectionStr);                    
                    memoryCache.Set(CacheKeys.GetRolesDropdown, roles, cacheOptions);                    
                }

                if (!memoryCache.TryGetValue(CacheKeys.GetBanksDropdown, out banks))
                {
                    banks = GetBankList(sqlConnectionStr);
                    memoryCache.Set(CacheKeys.GetBanksDropdown, banks, cacheOptions);
                }

                if (!memoryCache.TryGetValue(CacheKeys.GetBrandsDropdown, out brands))
                {
                    brands = GetBrandList(sqlConnectionStr);
                    memoryCache.Set(CacheKeys.GetBrandsDropdown, brands, cacheOptions);
                }

                if (!memoryCache.TryGetValue(CacheKeys.GetDepartmentsDropdown, out departments))
                {
                    departments = GetDepartmentList(sqlConnectionStr);
                    memoryCache.Set(CacheKeys.GetDepartmentsDropdown, departments, cacheOptions);
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
                    var rowInput= new EmployeeExcelInput();
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
                            var brandNames= new List<string>();
                            EmployeeBulkInsert newEmployee;
                            EmployeeImportError importError;
                            //int i = 0;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                rowInput.Name = (worksheet.Cells[row, 1]?.Text ?? string.Empty).Trim();//A
                                rowInput.EmployeeCode = (worksheet.Cells[row, 2]?.Text ?? string.Empty).Trim();//B
                                rowInput.Role = (worksheet.Cells[row, 3]?.Text ?? string.Empty).Trim();//C
                                rowInput.Dept = (worksheet.Cells[row, 4]?.Text ?? string.Empty).Trim();//D
                                rowInput.StatusStr = (worksheet.Cells[row, 5]?.Text ?? string.Empty).Trim();//E
                                rowInput.Brand = (worksheet.Cells[row, 6]?.Text ?? string.Empty).Trim();//F
                                rowInput.BankName = (worksheet.Cells[row, 7]?.Text ?? string.Empty).Trim();//G
                                rowInput.BankAccountNumber = (worksheet.Cells[row, 8]?.Text ?? string.Empty).Trim();//H
                                rowInput.StartDateStr = (worksheet.Cells[row, 9]?.Text ?? string.Empty).Trim();//I
                                rowInput.SalaryStr = (worksheet.Cells[row, 10]?.Text ?? string.Empty).Trim();//J
                                rowInput.BirthDateStr = (worksheet.Cells[row, 11]?.Text ?? string.Empty).Trim();//K
                                rowInput.IdNumber = (worksheet.Cells[row, 12]?.Text ?? string.Empty).Trim();//L
                                rowInput.BackendUser = (worksheet.Cells[row, 13]?.Text ?? string.Empty).Trim();//M
                                rowInput.BackendPass = (worksheet.Cells[row, 14]?.Text ?? string.Empty).Trim();//N
                                rowInput.Note = (worksheet.Cells[row, 15]?.Text ?? string.Empty).Trim();//O

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

                                if (string.IsNullOrEmpty(rowInput.Role))//C
                                {
                                    cells.Add($"C{row}");
                                    errorDetails.Add("Missing (Rank) Role");
                                }
                                else
                                {
                                    rowInput.RoleId = roles.FirstOrDefault(p => p.Name.Equals(rowInput.Role, StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.RoleId == null)
                                    {
                                        cells.Add($"C{row}");
                                        errorDetails.Add("Invalid (Rank) Role");
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
                                    brandNames = rowInput.Brand.Split(',').Select(p=>(p?.ToLower()??string.Empty).Trim()).ToList();
                                    rowInput.BrandIdList = brands.Where(p => brandNames.Contains(p.Name.ToLower())).Select(p => p.Id).ToList();
                                    
                                    if (rowInput.BrandIdList.Count == 0)
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
                                    if (!int.TryParse(rowInput.SalaryStr,out salary))
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

                                if (string.IsNullOrEmpty(rowInput.BackendUser))//M
                                {
                                    cells.Add($"M{row}");
                                    errorDetails.Add("Missing BackendUser");
                                }

                                if (string.IsNullOrEmpty(rowInput.BackendPass))//N
                                {
                                    cells.Add($"N{row}");
                                    errorDetails.Add("Missing BackendPass");
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
                            }
                        }
                    }
                    Console.WriteLine($"employees count {employees.Count}");
                    await BulkInsertEmployeesToDB(employees);
                    //Console.WriteLine($"Complete valid data from excel: time {watch.Elapsed.TotalSeconds} s");
                    //Task insertCustomerModelTask = Task.Run(() => BulkInsertCustomerModelToMySQL(customerRows));
                    //Task insertCustomerScoreTask = Task.Run(() => BulkInsertCustomerScoreToMySQL(customerScoreRows));
                    //await Task.WhenAll(insertCustomerModelTask, insertCustomerScoreTask);
                    //var importHistories = customerRows.GroupBy(p => p.Source)
                    //                .Select(p => new ImportDataHistory { ImportName = ImportNames.ImportCustomerScore, Source = p.Key, FileName = formFile.FileName, ImportByEmail = userEmail, TotalRows = p.Count() });
                    //importDataToQueueService.InsertImportHistory(sqlConnectionStr, importHistories);
                    //importProcess.ShouldSendEmail = importProcess.CustomerImports.Count > shouldSendEmailWhenReachLimit;
                    //shouldSendEmail = shouldSendEmail || importProcess.ShouldSendEmail;
                    //importDataToQueueService.InsertOrUpdateLeadManagementReport(sqlConnectionStr, importProcess);
                    watch.Stop();
                    Console.WriteLine($"Complete Import data: time {watch.Elapsed.TotalSeconds} s");
                }
                return Results.Ok(new { totalRows, errorList, shouldSendEmail });
            });

            app.MapGet("employee/dropdown", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db /*,[FromServices] IMemoryCache memoryCache*/) =>
            {
                List<BaseDropdown> items = null;
                //if (memoryCache.TryGetValue(CacheKeys.GetAdminScoresKey, out items))
                //    return Results.Ok(items);

                items = GetBaseDropdown(sqlConnectionStr);
                //var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                //memoryCache.Set(CacheKeys.GetAdminScoresKey, items, cacheOptions);
                return Results.Ok(items);
            });

            async Task BulkInsertEmployeesToDB(IEnumerable<EmployeeBulkInsert> items)
            {
                if (items.Count() == 0) return;
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
        }
    }
}