using BITool.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;

namespace BITool.Services
{
    public static class SourceReportService
    {
        public static void AddSourceReportService(this WebApplication app, string sqlConnectionStr)
        {
            app.MapGet("sourceReport/getSummary", [AllowAnonymous] async Task<IResult> () =>
            {
                using var connection = new SqlConnection(sqlConnectionStr);
                var items = connection.Query<SourceReport>(
                    "select Source, count(1) as 'TotalNumbers', AVG(TotalPoints) as 'AveragePoints' " +
                    "from LeadManagementReport " +
                    "group by Source ;");
                return Results.Ok(items);
            });
        }
    }
}