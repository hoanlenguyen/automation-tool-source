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
                var totalCount = connection.QueryFirstOrDefault<int>("select count(1) from leadmanagementreport ;");
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

            app.MapGet("overallReport/getTotalCountByLimitedRange/{limitedRange:int}", [AllowAnonymous] async Task<IResult> (int limitedRange) =>
            {
                using var connection = new MySqlConnection(sqlConnectionStr);
                var counts = await connection.QueryAsync<OverallReportPointsCount>(
                    $"select TotalPoints, count(1) as 'Count' " +
                    $"from leadmanagementreport " +
                    $"group by TotalPoints " +
                    $"having TotalPoints <= {limitedRange}");

                var result = new List<OverallReportPointsCountView>();
                for (int i = 0; i <= limitedRange; i++)
                {
                    result.Add(new OverallReportPointsCountView { TotalPoints = i.ToString(), Count = counts.FirstOrDefault(p => p.TotalPoints == i)?.Count ?? 0 });
                }
                var overLimit = await connection.QueryFirstOrDefaultAsync<int>($"select count(1) from leadmanagementreport where TotalPoints > {limitedRange}");
                result.Add(new OverallReportPointsCountView { TotalPoints = $">{limitedRange}", Count= overLimit });
                return Results.Ok(result);
            });
        }
    }
}