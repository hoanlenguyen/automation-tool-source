using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Microsoft.AspNetCore.Authorization;
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

        private static List<BaseDropdown> GetRoleList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, LOWER(Name) from UserRole where IsDeleted = 0").ToList();
        }

        private static List<BaseDropdown> GetBankList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, LOWER(Name) from Bank where IsDeleted = 0").ToList();
        }

        private static List<BaseDropdown> GetBrandList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, LOWER(Name) from Brand where IsDeleted = 0").ToList();
        }

        private static List<BaseDropdown> GetDepartmentList(string sqlConnectionStr)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>("select Id, LOWER(Name) from Department where IsDeleted = 0").ToList();
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

            app.MapPost("employee/create", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromBody] EmployeeCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Employee { Name = input.Name, CreatorUserId = userId };
                db.Add(entity);
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapPut("employee/update", [Authorize]
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

                entity.Name = input.Name;
                entity.Status = input.Status;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                //db.Update(entity);
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

            app.MapPost("employee/list", [Authorize]
            async Task<IResult> (
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
                    var items = new List<EmployeeCreateOrEdit>();
                    while (rdr.Read())
                    {
                        items.Add(new EmployeeCreateOrEdit
                        {
                            Id = CommonHelper.ConvertFromDBVal<int>(rdr["Id"]),
                            Name = CommonHelper.ConvertFromDBVal<string>(rdr["Name"]),
                            Status = CommonHelper.ConvertFromDBVal<bool>(rdr["Status"])
                        });
                    }
                    rdr.Close();
                    conn.Close();

                    return Results.Ok(new PagedResultDto<EmployeeCreateOrEdit>(totalCount, items));
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
                foreach (var formFile in request.Form.Files)
                {
                    if (formFile is null || formFile.Length == 0)
                        continue;
                    var employees = new List<EmployeeCreateOrEdit>();
                    var rowCount = 0;
                    var rowInput= new EmployeeExcelInput();
                    //Process excel file
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null) continue;
                            //throw new Exception("No worksheet found!");

                            rowCount = worksheet.Dimension.Rows; //include header
                            if (rowCount <= 1) continue;
                            // throw new Exception("No Data in worksheet found!");

                            totalRows += rowCount - 1;
                            //read excel file data and add data                             
                            var now = DateTime.Now;
                            var cells = new List<string>();
                            var errorDetails = new List<string>();
                            var brandIds= new List<int>();
                            var brandNames= new List<string>();
                            //int i = 0;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                rowInput.Name = (worksheet.Cells[row, 1]?.Text ?? string.Empty).Trim();
                                rowInput.EmployeeCode = (worksheet.Cells[row, 2]?.Text ?? string.Empty).Trim();
                                rowInput.Role = (worksheet.Cells[row, 3]?.Text ?? string.Empty).Trim();
                                rowInput.Dept = (worksheet.Cells[row, 4]?.Text ?? string.Empty).Trim();
                                rowInput.Status = (worksheet.Cells[row, 5]?.Text ?? string.Empty).Trim();
                                rowInput.Brand = (worksheet.Cells[row, 6]?.Text ?? string.Empty).Trim();
                                rowInput.BankName = (worksheet.Cells[row, 7]?.Text ?? string.Empty).Trim();
                                rowInput.BankAccountNumber = (worksheet.Cells[row, 8]?.Text ?? string.Empty).Trim();
                                rowInput.StartDateStr = (worksheet.Cells[row, 9]?.Text ?? string.Empty).Trim();
                                rowInput.Salary = (worksheet.Cells[row, 10]?.Text ?? string.Empty).Trim();
                                rowInput.BirthDateStr = (worksheet.Cells[row, 11]?.Text ?? string.Empty).Trim();
                                rowInput.IdNumber = (worksheet.Cells[row, 12]?.Text ?? string.Empty).Trim();
                                rowInput.BackendUser = (worksheet.Cells[row, 13]?.Text ?? string.Empty).Trim();
                                rowInput.BackendPass = (worksheet.Cells[row, 14]?.Text ?? string.Empty).Trim();
                                rowInput.Note = (worksheet.Cells[row, 15]?.Text ?? string.Empty).Trim();

                                //i = 0;
                                //check error
                                if (string.IsNullOrEmpty(rowInput.Name))
                                {
                                    cells.Add($"A{row}");
                                    errorDetails.Add("Missing Name");
                                }

                                if (string.IsNullOrEmpty(rowInput.EmployeeCode))
                                {
                                    cells.Add($"B{row}");
                                    errorDetails.Add("Missing Employee Code");
                                }

                                if (string.IsNullOrEmpty(rowInput.Role))
                                {
                                    cells.Add($"C{row}");
                                    errorDetails.Add("Missing (Rank) Role");
                                }
                                else
                                {
                                    rowInput.RoleId = roles.FirstOrDefault(p => p.Name.Equals(rowInput.Role.ToLower(), StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.RoleId == null)
                                    {
                                        cells.Add($"C{row}");
                                        errorDetails.Add("Invalid (Rank) Role");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Dept))
                                {
                                    cells.Add($"D{row}");
                                    errorDetails.Add("Missing Dept");
                                }
                                else
                                {
                                    rowInput.DeptId = departments.FirstOrDefault(p => p.Name.Equals(rowInput.Dept.ToLower(), StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (rowInput.DeptId == null)
                                    {
                                        cells.Add($"D{row}");
                                        errorDetails.Add("Invalid Dept");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.Brand))
                                {
                                    cells.Add($"F{row}");
                                    errorDetails.Add("Missing Brand");
                                }
                                else
                                {
                                    brandNames= rowInput.Brand.Split(',').Select(p=>(p?.ToLower()??string.Empty).Trim()).ToList();
                                    brandIds = brands.Where(p => brandNames.Contains(p.Name)).Select(p => p.Id).ToList();
                                    //rowInput.DeptId = departments.FirstOrDefault(p => p.Name.Equals(rowInput.Dept.ToLower(), StringComparison.OrdinalIgnoreCase))?.Id;
                                    if (brandIds.Count==0)
                                    {
                                        cells.Add($"F{row}");
                                        errorDetails.Add("Invalid Brand");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BirthDateStr))
                                {
                                    cells.Add($"H{row}");
                                    errorDetails.Add("Invalid (Rank) Role");
                                }

                                if (string.IsNullOrEmpty(rowInput.BankName))
                                {
                                    cells.Add($"F{row}");
                                    errorDetails.Add("Missing Bank");
                                }
                                else
                                {
                                    rowInput.BirthDate = CheckValidDate(rowInput.BirthDateStr);
                                    if (rowInput.BirthDate == null)
                                    {
                                        cells.Add($"H{row}");
                                        errorDetails.Add("Invalid (Rank) Role");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.BirthDateStr))
                                {
                                    cells.Add($"H{row}");
                                    errorDetails.Add("Invalid (Rank) Role");
                                }
                                else
                                {
                                    rowInput.BirthDate = CheckValidDate(rowInput.BirthDateStr);
                                    if (rowInput.BirthDate == null)
                                    {
                                        cells.Add($"H{row}");
                                        errorDetails.Add("Invalid (Rank) Role");
                                    }
                                }

                                if (string.IsNullOrEmpty(rowInput.IdNumber))
                                {
                                    cells.Add($"F{row}");
                                    errorDetails.Add("Missing ID Number");
                                }

                                if (string.IsNullOrEmpty(rowInput.BackendUser))
                                {
                                    cells.Add($"H{row}");
                                    errorDetails.Add("Missing BackendUser");
                                }

                                if (string.IsNullOrEmpty(rowInput.BackendPass))
                                {
                                    cells.Add($"G{row}");
                                    errorDetails.Add("Missing BackendPass");
                                }

                                if (string.IsNullOrEmpty(rowInput.BackendPass))
                                {
                                    cells.Add($"G{row}");
                                    errorDetails.Add("Missing BackendPass");
                                }


                                errorList.Add(new EmployeeImportError
                                {
                                    Cells = string.Join(" - ", cells),
                                    ErrorDetails = string.Join(" - ", errorDetails)
                                });

                                if (cells.Count == 0) // if no error , add new record
                                {

                                }
                                cells = new List<string>(); //reset after add
                                errorDetails = new List<string>(); //reset after add
                            }
                        }
                    }
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
        }
    }
}