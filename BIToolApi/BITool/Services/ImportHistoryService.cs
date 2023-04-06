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
    public static class ImportHistoryService
    {
        public static void AddImportHistoryService(this WebApplication app, string sqlConnectionStr)
        {
            static void ProcessInputValues(ref ImportDataHistoryFilter input)
            {
                if (input.ImportTimeFrom != null)
                    input.ImportTimeFrom = input.ImportTimeFrom.Value.Date;

                if (input.ImportTimeTo != null)
                    input.ImportTimeTo = input.ImportTimeTo.Value.Date.AddDays(1).AddMilliseconds(-1);

                if (string.IsNullOrEmpty(input.SortBy))
                    input.SortBy = "ID";
                if (string.IsNullOrEmpty(input.SortDirection))
                    input.SortDirection = "desc";
            }

            static int GetTotalCountByFilter(string sqlConnectionStr, ref ImportDataHistoryFilter input)
            {
                using (var conn = new SqlConnection(sqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(StoredProcedureName.GetImportDataHistoryCountByFilter, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@source", input.Source);

                    cmd.Parameters.AddWithValue("@importTimeFrom", input.ImportTimeFrom);
                    cmd.Parameters.AddWithValue("@importTimeTo", input.ImportTimeTo);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    int count = 0;
                    while (rdr.Read())
                        count = (int)rdr.GetInt64(0);
                    rdr.Close();
                    conn.Close();
                    return count;
                }
            }

            app.MapGet("importHistory/getSource", [Authorize] async Task<IResult> () =>
            {
                using var connection = new SqlConnection(sqlConnectionStr);
                var result = connection.Query<string>("select distinct Source from importdatahistory where Source is not null;");
                return Results.Ok(result);
            });

            app.MapPost("importHistory/paging", [Authorize] async Task<IResult> ([FromBody] ImportDataHistoryFilter input) =>
            {
                ProcessInputValues(ref input);
                //Console.WriteLine(input.SortBy);
                //Console.WriteLine(input.SortDirection);
                var totalCount = GetTotalCountByFilter(sqlConnectionStr, ref input);
                using (var conn = new SqlConnection(sqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(StoredProcedureName.GetImportDataHistoryByFilter, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@source", input.Source);

                    cmd.Parameters.AddWithValue("@importTimeFrom", input.ImportTimeFrom);
                    cmd.Parameters.AddWithValue("@importTimeTo", input.ImportTimeTo);

                    cmd.Parameters.AddWithValue("@sortBy", input.SortBy);
                    cmd.Parameters.AddWithValue("@sortDirection", input.SortDirection);

                    cmd.Parameters.AddWithValue("@exportOffset", input.SkipCount);
                    cmd.Parameters.AddWithValue("@exportLimit", input.RowsPerPage);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    var items = new List<ImportDataHistory>();
                    while (rdr.Read())
                    {
                        items.Add(new ImportDataHistory
                        {
                            ID = CommonHelper.ConvertFromDBVal<int>(rdr["ID"]),
                            FileName = CommonHelper.ConvertFromDBVal<string>(rdr["FileName"]),
                            ImportTime = CommonHelper.ConvertFromDBVal<DateTime>(rdr["ImportTime"]),
                            TotalRows = CommonHelper.ConvertFromDBVal<int>(rdr["TotalRows"]),
                            Source = CommonHelper.ConvertFromDBVal<string>(rdr["Source"]),
                            ImportByEmail = CommonHelper.ConvertFromDBVal<string>(rdr["ImportByEmail"])
                        });
                    }
                    rdr.Close();
                    conn.Close();

                    return Results.Ok(new PagedResultDto<ImportDataHistory>(totalCount, items));
                }
            });
        }
    }
}