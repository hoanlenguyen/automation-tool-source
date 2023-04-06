using BITool.BackgroundJobs;
using BITool.Enums;
using BITool.Helpers;
using BITool.Models;
using BITool.Models.ImportDataToQueue;
using BITool.Models.SignalR;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace BITool.Services
{
    public interface IImportDataToQueueService
    {
        void InsertImportHistory(string sqlConnectionStr, IEnumerable<ImportDataHistory> input);

        void InsertOrUpdateLeadManagementReport(string sqlConnectionStr, ProcessImportedData input);

        void BulkInsertCleaningDataHistory(string sqlConnectionStr, IEnumerable<CleanDataHistory> input);
    }

    public class ImportDataToQueueService : IImportDataToQueueService
    {
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly ILogger logger;
        private readonly CancellationToken cancellationToken;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;
        private readonly ISendMailService sendMail;
        private readonly IHubContext<HubClient> hubContext;
        private readonly int DEFAULT_BATCH_SIZE;

        public ImportDataToQueueService(
            IBackgroundTaskQueue taskQueue,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ISendMailService sendMail,
            IHubContext<HubClient> hubContext,
            ILogger<ImportDataToQueueService> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            this.taskQueue = taskQueue;
            this.logger = logger;
            this.configuration = configuration;
            this.memoryCache = memoryCache;
            this.sendMail = sendMail;
            this.hubContext = hubContext;
            DEFAULT_BATCH_SIZE = configuration.GetValue<int>("DEFAULT_BATCH_SIZE");
            cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void InsertImportHistory(string connectionStr, IEnumerable<ImportDataHistory> input)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementInsertImportHistory(cancellationToken, connectionStr, input)));
        }

        public void InsertOrUpdateLeadManagementReport(string sqlConnectionStr, ProcessImportedData input)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementInsertOrUpdateLeadManagementReport(cancellationToken, sqlConnectionStr, input)));
        }

        public void BulkInsertCleaningDataHistory(string sqlConnectionStr, IEnumerable<CleanDataHistory> input)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementBulkInsertCleaningDataHistory(cancellationToken, sqlConnectionStr, input)));
        }

        private async ValueTask ImplementInsertImportHistory(CancellationToken token, string connectionStr, IEnumerable<ImportDataHistory> input)
        {
            if (token.IsCancellationRequested) return;
            foreach (var item in input)
            {
                //Console.WriteLine($"ImportDataHistory-FileName: {item.FileName}");
                var dataTable = input.ToDataTable();
                using var connection = new SqlConnection(connectionStr);
                connection.Open();
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = TableName.ImportDataHistory;
                bulkCopy.BulkCopyTimeout = 0;
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }

        private async ValueTask ImplementInsertOrUpdateLeadManagementReport(CancellationToken token, string sqlConnectionStr, ProcessImportedData input)
        {
            if (token.IsCancellationRequested) return;
            if (!input.CustomerImports.Any()) return;
            var watch = Stopwatch.StartNew();
            logger.LogInformation($"customerImports count{input.CustomerImports.Count}");
            input.CustomerImports = input.CustomerImports.GroupBy(p => p.CustomerMobileNo).Select(p =>
               new CustomerImportDto
               {
                   CustomerMobileNo = p.Key,
                   DateOccurred = p.FirstOrDefault().DateOccurred,
                   ScoreIds = p.SelectMany(q => q.ScoreIds).ToList(),
                   Source = p.FirstOrDefault().Source
               }).ToList();
            var listCount = input.CustomerImports.Count;
            var timeCount = watch.Elapsed.TotalSeconds;
            watch.Stop();
            logger.LogInformation($"customerImports after group-by count{listCount} Time: ${watch.Elapsed.TotalSeconds} s");
            watch.Restart();
            List<AdminScoreDto> adminScores = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetAdminScoresKey, out adminScores))
            {
                adminScores = GetAdminScores(sqlConnectionStr);
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                memoryCache.Set(CacheKeys.GetAdminScoresKey, adminScores, cacheOptions);
            }

            var occuranceScoreList = adminScores.Where(p => p.ScoreCategory.Equals(ScoreTitleType.Occurance, StringComparison.OrdinalIgnoreCase))
                                               .ToList();

            var resultsScoreList = adminScores.Where(p => p.ScoreCategory.Equals(ScoreTitleType.Results, StringComparison.OrdinalIgnoreCase))
                                               .ToList();

            var packageCount = (listCount - 1) / DEFAULT_BATCH_SIZE + 1;
            watch.Stop();
            timeCount += watch.Elapsed.TotalSeconds;
            logger.LogInformation($"Complete get all extra data. Time: {watch.Elapsed.TotalSeconds} s");
            for (int i = 0; i < packageCount; i++)
            {
                watch.Restart();
                var itemCount = DEFAULT_BATCH_SIZE;
                if (i == packageCount - 1)
                    itemCount = (listCount - i * DEFAULT_BATCH_SIZE);
                var rangeCustomerImports = input.CustomerImports.GetRange(i * DEFAULT_BATCH_SIZE, itemCount);
                await ProcessRangeReportItems(sqlConnectionStr, adminScores, occuranceScoreList, resultsScoreList, rangeCustomerImports);
                watch.Stop();
                timeCount += watch.Elapsed.TotalSeconds;
                logger.LogInformation($"ProcessRangeReportItems pack: {i}. Time: {watch.Elapsed.TotalSeconds} s");
            }
            if (input.ShouldSendEmail)
            {
                try
                {
                    var content = $"<p>Import from file {input.FileName} has been successful. Total time: {(int)timeCount} s.</p>" +
                              $"<p>The export data function is now updated with this new data.</p>";
                    await sendMail.SendMailAsync(new Models.SendMail.SendMailDto { ToAddresses = input.UserEmail, Subject = "Update LeadManagementReport - Import CustomerScore", HtmlContent = content });
                }
                catch (Exception) { }

                if (hubContext.Clients != null && input.SignalRConnectionId.IsNotNullOrEmpty())
                {
                    var client = hubContext.Clients.Client(input.SignalRConnectionId);
                    if (client != null)
                    {
                        Console.WriteLine($"ClientProxy {input.SignalRConnectionId}");
                        var content = $"Import from file {input.FileName} has been successful. Total time: {(int)timeCount} s.\n" +
                                      $"The export data function is now updated with this new data.";
                        await client.SendAsync(HubClientName.CompleteImport, new HubMessenger { Title = "Update LeadManagementReport", Content = content });
                    }
                }
                else
                    Console.WriteLine($"No client {input.SignalRConnectionId}");
            }
            logger.LogInformation($"Complete {nameof(ImplementInsertOrUpdateLeadManagementReport)}; Total time: {timeCount} s");
        }

        private async ValueTask ImplementBulkInsertCleaningDataHistory(CancellationToken token, string connectionStr, IEnumerable<CleanDataHistory> input)
        {
            if (token.IsCancellationRequested) return;
            try
            {
                Console.WriteLine($"{nameof(ImplementBulkInsertCleaningDataHistory)}: count {input.Count()}");
                if (input.Count() == 0) return;
                var watch = Stopwatch.StartNew();
                var dataTable = input.ToDataTable();
                using var connection = new SqlConnection(connectionStr);
                connection.Open();

                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = TableName.CleanDataHistory;
                bulkCopy.BulkCopyTimeout = 0;
                await bulkCopy.WriteToServerAsync(dataTable);

                watch.Stop();
                Console.WriteLine($"Complete {nameof(ImplementBulkInsertCleaningDataHistory)}: time {watch.Elapsed.TotalSeconds} s");
            }
            catch (Exception ex)
            {
                logger.LogError($"ImplementInsertImportHistory error: {ex.Message}");
            }
        }

        private async Task ProcessRangeReportItems(
            string sqlConnectionStr,
            List<AdminScoreDto> adminScores,
            List<AdminScoreDto> occuranceScoreList,
            List<AdminScoreDto> resultsScoreList,
            List<CustomerImportDto> customerImports)
        {
            int index = 0;
            var leadManagementReports = await GetLeadManagementReportsAsync(sqlConnectionStr, customerImports.Select(p => new TempLeadManagementReport { CustomerMobileNo = p.CustomerMobileNo }));
            var newLeadManagementReports = new List<LeadManagementReport>();
            foreach (var item in customerImports)
            {
                var reportItem = leadManagementReports.FirstOrDefault(p => p.CustomerMobileNo == item.CustomerMobileNo);
                if (reportItem is null)
                {
                    reportItem = new LeadManagementReport
                    {
                        Source = item.Source,
                        CustomerMobileNo = item.CustomerMobileNo,
                        DateFirstAdded = item.DateOccurred,
                    };
                    newLeadManagementReports.Add(reportItem);
                }
                reportItem.DateLastOccurred = item.DateOccurred;
                foreach (var scoreId in item.ScoreIds)
                {
                    var score = adminScores.FirstOrDefault(p => p.ScoreID == scoreId);
                    if (score != null)
                    {
                        if (score.ScoreCategory.Equals(ScoreTitleType.Occurance, StringComparison.OrdinalIgnoreCase))
                        {
                            index = occuranceScoreList.FindIndex(p => p.ScoreID == score.ScoreID);
                            switch (index)
                            {
                                case 0: { reportItem.OccuranceTotalFirstScore += score.Points; break; }
                                case 1: { reportItem.OccuranceTotalSecondScore += score.Points; break; }
                                case 2: { reportItem.OccuranceTotalThirdScore += score.Points; break; }
                                case 3: { reportItem.OccuranceTotalFourthScore += score.Points; break; }
                                case 4: { reportItem.OccuranceTotalFifthScore += score.Points; break; }
                                default: break;
                            }
                        }
                        else
                        {
                            index = resultsScoreList.FindIndex(p => p.ScoreID == score.ScoreID);
                            switch (index)
                            {
                                case 0: { reportItem.ResultsTotalFirstScore += score.Points; break; }
                                case 1: { reportItem.ResultsTotalSecondScore += score.Points; break; }
                                case 2: { reportItem.ResultsTotalThirdScore += score.Points; break; }
                                case 3: { reportItem.ResultsTotalFourthScore += score.Points; break; }
                                default: break;
                            }
                        }
                    }
                }
            }
            if (leadManagementReports.Any())
            {
                var dataTable = leadManagementReports.ToDataTable();
                using var connection = new SqlConnection(sqlConnectionStr);
                connection.Open();
                var commandStr = "CREATE TEMPORARY TABLE IF NOT EXISTS temp_leadmanagementreport SELECT * FROM leadmanagementreport LIMIT 0;";
                using (SqlCommand myCmd = new SqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = "temp_leadmanagementreport";
                bulkCopy.BulkCopyTimeout = 0;
                await bulkCopy.WriteToServerAsync(dataTable);
                commandStr = "UPDATE leadmanagementreport AS l " +
                             "INNER JOIN temp_leadmanagementreport AS t " +
                             "ON l.CustomerMobileNo = t.CustomerMobileNo " +
                             "SET " +
                             "l.OccuranceTotalFirstScore = t.OccuranceTotalFirstScore, " +
                             "l.OccuranceTotalSecondScore = t.OccuranceTotalSecondScore, " +
                             "l.OccuranceTotalThirdScore = t.OccuranceTotalThirdScore, " +
                             "l.OccuranceTotalFourthScore = t.OccuranceTotalFourthScore, " +
                             "l.OccuranceTotalFifthScore = t.OccuranceTotalFifthScore, " +
                             "l.TotalOccurancePoints = t.TotalOccurancePoints, " +
                             "l.DateLastOccurred = t.DateLastOccurred, " +

                             "l.ResultsTotalFirstScore = t.ResultsTotalFirstScore, " +
                             "l.ResultsTotalSecondScore = t.ResultsTotalSecondScore, " +
                             "l.ResultsTotalThirdScore = t.ResultsTotalThirdScore, " +
                             "l.ResultsTotalFourthScore = t.ResultsTotalFourthScore, " +
                             "l.TotalResultsPoints = t.TotalResultsPoints, " +

                             "l.TotalPoints = t.TotalPoints, " +
                             "l.ExportVsPointsPercentage = t.ExportVsPointsPercentage, " +
                             "l.ExportVsPointsNumber = t.ExportVsPointsNumber"
                             ;
                using (SqlCommand myCmd = new SqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                commandStr = "DROP TEMPORARY TABLE IF EXISTS temp_leadmanagementreport";
                using (SqlCommand myCmd = new SqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }

            if (newLeadManagementReports.Any())
            {
                var dataTable = newLeadManagementReports.ToDataTable();
                using var connection = new SqlConnection(sqlConnectionStr);
                connection.Open();
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = TableName.LeadManagementReport;
                //bulkCopy.ColumnMappings.AddRange(dataTable.GetSqlColumnMapping());
                bulkCopy.BulkCopyTimeout = 0;
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }

        private List<AdminScoreDto> GetAdminScores(string sqlConnectionStr)
        {
            using var connection = new SqlConnection(sqlConnectionStr);
            return connection.Query<AdminScoreDto>("SELECT * FROM adminscore").ToList();
        }

        private async Task<List<LeadManagementReport>> GetLeadManagementReportsAsync(string sqlConnectionStr, IEnumerable<TempLeadManagementReport> items)
        {
            if (items is null || !items.Any())
                return new List<LeadManagementReport>();

            var dataTable = items.ToDataTable();
            using var connection = new SqlConnection(sqlConnectionStr);
            connection.Open();
            var commandStr = "create temporary table IF NOT EXISTS TempLeadManagementReport(CustomerMobileNo BIGINT NOT NULL);";
            using (SqlCommand myCmd = new SqlCommand(commandStr, connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            var bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "TempLeadManagementReport";
            await bulkCopy.WriteToServerAsync(dataTable);

            commandStr = "select l.* " +
                         "from leadmanagementreport  l " +
                         "inner join TempLeadManagementReport t " +
                         "on l.CustomerMobileNo = t.CustomerMobileNo";

            var data = connection.Query<LeadManagementReport>(commandStr).ToList();

            commandStr = "DROP TEMPORARY TABLE IF EXISTS TempLeadManagementReport";
            using (SqlCommand myCmd = new SqlCommand(commandStr, connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            return data;
        }
    }
}