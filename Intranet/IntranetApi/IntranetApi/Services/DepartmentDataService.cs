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
    public static class DepartmentDataService
    {
        private static void ProcessFilterValues(ref DepartmentFilterDto input)
        {
            if (string.IsNullOrEmpty(input.SortBy))
                input.SortBy = "Id";
            if (string.IsNullOrEmpty(input.SortDirection))
                input.SortDirection = "desc";
        }

        private static int GetTotalCountByFilter(string sqlConnectionStr, ref DepartmentFilterDto input)
        {
            using (var conn = new MySqlConnection(sqlConnectionStr))
            {
                conn.Open();
                var cmd = new MySqlCommand(StoredProcedureName.GetDepartmentTotal, conn);
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

        public static void AddDepartmentDataService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("Department/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var entity = db.Department.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();
                return Results.Ok(entity);
            });

            app.MapPost("Department", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromBody] DepartmentCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = new Department { Name = input.Name, CreatorUserId = userId };
                db.Add(entity);
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapPut("Department", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            [FromBody] DepartmentCreateOrEdit input) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Department.FirstOrDefault(x => x.Id == input.Id);
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

            app.MapDelete("Department/{id:int}", [Authorize]
            async Task<IResult> (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] ApplicationDbContext db,
            int id) =>
            {
                var userIdStr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);
                var entity = db.Department.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return Results.NotFound();

                entity.IsDeleted = true;
                entity.LastModifierUserId = userId;
                entity.LastModificationTime = DateTime.Now;
                db.SaveChanges();
                return Results.Ok();
            });

            app.MapPost("Department/list", [Authorize]
            async Task<IResult> (
            [FromServices] ApplicationDbContext db,
            [FromBody] DepartmentFilterDto input) =>
            {
                ProcessFilterValues(ref input);
                var totalCount = GetTotalCountByFilter(sqlConnectionStr, ref input);
                using (var conn = new MySqlConnection(sqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(StoredProcedureName.GetDepartmentList, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyword", input.Keyword);
                    cmd.Parameters.AddWithValue("@status", input.Status);
                    cmd.Parameters.AddWithValue("@sortBy", input.SortBy);
                    cmd.Parameters.AddWithValue("@sortDirection", input.SortDirection);

                    cmd.Parameters.AddWithValue("@exportOffset", input.SkipCount);
                    cmd.Parameters.AddWithValue("@exportLimit", input.RowsPerPage);

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    var items = new List<DepartmentCreateOrEdit>();
                    while (rdr.Read())
                    {
                        items.Add(new DepartmentCreateOrEdit
                        {
                            Id = CommonHelper.ConvertFromDBVal<int>(rdr["Id"]),
                            Name = CommonHelper.ConvertFromDBVal<string>(rdr["Name"]),
                            Status = CommonHelper.ConvertFromDBVal<bool>(rdr["Status"]),
                            CreationTime = CommonHelper.ConvertFromDBVal<DateTime>(rdr["CreationTime"])
                        });
                    }
                    rdr.Close();
                    conn.Close();

                    return Results.Ok(new PagedResultDto<DepartmentCreateOrEdit>(totalCount, items));
                }
            });
        }
    }
}