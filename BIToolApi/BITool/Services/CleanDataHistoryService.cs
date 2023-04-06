using BITool.Enums;
using BITool.Helpers;
using BITool.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BITool.Services
{
    public static class CleanDataHistoryService
    {
        public static void AddCleanDataHistoryService(this WebApplication app, string SqlConnectionStr)
        {
            static void ProcessInputValues(ref CleanDataHistoryFilter input)
            {
                if (input.CleanTimeFrom != null)
                    input.CleanTimeFrom = input.CleanTimeFrom.Value.Date;

                if (input.CleanTimeTo != null)
                    input.CleanTimeTo = input.CleanTimeTo.Value.Date.AddDays(1).AddMilliseconds(-1);

                if (string.IsNullOrEmpty(input.SortBy))
                    input.SortBy = "ID";
                if (string.IsNullOrEmpty(input.SortDirection))
                    input.SortDirection = "desc";
            }

            static int GetTotalCountByFilter(string SqlConnectionStr, ref CleanDataHistoryFilter input)
            {
                using (var conn = new SqlConnection(SqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(StoredProcedureName.GetCleanDataHistoryCountByFilter, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@source", input.Source);

                    cmd.Parameters.AddWithValue("@cleanTimeFrom", input.CleanTimeFrom);
                    cmd.Parameters.AddWithValue("@cleanTimeTo", input.CleanTimeTo);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    int count = 0;
                    while (rdr.Read())
                        count = (int)rdr.GetInt64(0);
                    rdr.Close();
                    conn.Close();
                    return count;
                }
            }

            app.MapGet("cleanDataHistory/getSource", [Authorize] async Task<IResult> () =>
            {
                using var connection = new SqlConnection(SqlConnectionStr);
                var result = connection.Query<string>("select distinct Source from CleanDataHistory where Source is not null;");
                return Results.Ok(result);
            });

            app.MapPost("cleanDataHistory/paging", [AllowAnonymous] async Task<IResult> ([FromBody] CleanDataHistoryFilter input) =>
            {
                ProcessInputValues(ref input);
                var totalCount = GetTotalCountByFilter(SqlConnectionStr, ref input);
                using (var conn = new SqlConnection(SqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(StoredProcedureName.GetCleanDataHistoryByFilter, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@source", input.Source);

                    cmd.Parameters.AddWithValue("@cleanTimeFrom", input.CleanTimeFrom);
                    cmd.Parameters.AddWithValue("@cleanTimeTo", input.CleanTimeTo);

                    cmd.Parameters.AddWithValue("@sortBy", input.SortBy);
                    cmd.Parameters.AddWithValue("@sortDirection", input.SortDirection);

                    cmd.Parameters.AddWithValue("@exportOffset", input.SkipCount);
                    cmd.Parameters.AddWithValue("@exportLimit", input.RowsPerPage);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    var items = new List<CleanDataHistory>();
                    while (rdr.Read())
                    {
                        items.Add(new CleanDataHistory
                        {
                            ID = CommonHelper.ConvertFromDBVal<int>(rdr["ID"]),
                            FileName = CommonHelper.ConvertFromDBVal<string>(rdr["FileName"]),
                            CleanTime = CommonHelper.ConvertFromDBVal<DateTime>(rdr["CleanTime"]),
                            Source = CommonHelper.ConvertFromDBVal<string>(rdr["Source"]),
                            TotalRows = CommonHelper.ConvertFromDBVal<int>(rdr["TotalRows"]),
                            TotalInvalidNumbers = CommonHelper.ConvertFromDBVal<int>(rdr["TotalInvalidNumbers"]),
                            TotalDuplicateNumbersInFile = CommonHelper.ConvertFromDBVal<int>(rdr["TotalDuplicateNumbersInFile"]),
                            TotalDuplicateNumbersWithSystem = CommonHelper.ConvertFromDBVal<int>(rdr["TotalDuplicateNumbersWithSystem"])
                        });
                    }
                    rdr.Close();
                    conn.Close();

                    return Results.Ok(new PagedResultDto<CleanDataHistory>(totalCount, items));
                }
            });
        }
    }
}