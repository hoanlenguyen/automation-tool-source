using BITool.Enums;
using BITool.Helpers;
using BITool.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace BITool.Services
{
    public static class OverallReportService
    {
        public static void AddOverallReportService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("overallReport/getTotalLeads", [Authorize] async Task<IResult> () =>
            {
                using var connection = new MySqlConnection(sqlConnectionStr);
                var totalCount = connection.Query<int>("select count(1) from leadmanagementreport ;").FirstOrDefault();
                return Results.Ok(new { totalCount });
            });

            app.MapGet("overallReport/getTotalCountByRange", [Authorize] async Task<IResult> ([FromQuery] int? totalPointsFrom) =>
            {
                int totalCount = 0;

                using (var conn = new MySqlConnection(sqlConnectionStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(StoredProcedureName.GetTotalCountByPointsRange, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@totalPointsFrom", totalPointsFrom); 

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    var items = new List<CleanDataHistory>();
                    while (rdr.Read())
                    {
                        totalCount = (int)rdr.GetInt64(0);
                    }
                    rdr.Close();
                    conn.Close();

                    return Results.Ok(new { totalCount });
                }
            });
        }
    }
}