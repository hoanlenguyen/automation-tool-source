using BITool.BackgroundJobs;
using BITool.Enums;
using BITool.Helpers;
using BITool.Models;
using BITool.Models.ImportDataToQueue;
using BITool.Models.SignalR;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Data;
using System.Diagnostics;

namespace BITool.Services
{
    public interface IExportDataToQueueService
    {
        void UpdateLastUsedCampaign(string sqlConnectionStr, ProcessLastUsedCampaign input);
        void BulkInsertCustomerCampaignToMySQL(string sqlConnectionStr, IEnumerable<long> customerMobileList, int campaignID, int userId);
        void BulkInsertRecordCustomerExport(string sqlConnectionStr, int userId, CampaignCreateOrEditDto campaign);
        void UpdateLastUsedCampaignOnLeadManagement(string sqlConnectionStr, CampaignCreateOrEditDto campaign);
    }

    public class ExportDataToQueueService : IExportDataToQueueService
    {
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly ILogger logger;
        private readonly CancellationToken cancellationToken;
        private readonly IHubContext<HubClient> hubContext;
        private readonly IMemoryCache memoryCache;
        private readonly ISendMailService sendMail;

        private const int DEFAULT_BATCH_SIZE = 20000;

        public ExportDataToQueueService(
            IBackgroundTaskQueue taskQueue,
            //IConfiguration configuration,
            IMemoryCache memoryCache,
            ISendMailService sendMail,
            IHubContext<HubClient> hubContext,
            ILogger<ImportDataToQueueService> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            this.taskQueue = taskQueue;
            this.logger = logger;
            this.hubContext = hubContext;
            this.memoryCache = memoryCache;
            this.sendMail = sendMail;
            cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void UpdateLastUsedCampaign(string sqlConnectionStr, ProcessLastUsedCampaign input)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementUpdateLastUsedCampaign(cancellationToken, sqlConnectionStr, input)));
        }

        public void BulkInsertCustomerCampaignToMySQL(string sqlConnectionStr, IEnumerable<long> customerMobileList, int campaignID, int userId)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementBulkInsertCustomerCampaignToMySQL(cancellationToken, sqlConnectionStr, customerMobileList, campaignID, userId)));
        }

        public void BulkInsertRecordCustomerExport(string sqlConnectionStr, int userId, CampaignCreateOrEditDto campaign)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementBulkInsertRecordCustomerExport(cancellationToken, sqlConnectionStr, userId, campaign)));
        }

        public void UpdateLastUsedCampaignOnLeadManagement(string sqlConnectionStr, CampaignCreateOrEditDto campaign)
        {
            Task.Run(async () => await taskQueue.QueueBackgroundWorkItemAsync(ct => ImplementUpdateLastUsedCampaignOnLeadManagement(cancellationToken, sqlConnectionStr, campaign)));
        }

        private async ValueTask ImplementBulkInsertRecordCustomerExport(CancellationToken token,string sqlConnectionStr, int userId, CampaignCreateOrEditDto campaign)
        {
            if (token.IsCancellationRequested) return;
            var watch = Stopwatch.StartNew();
            logger.LogInformation($"{nameof(ImplementUpdateLastUsedCampaign) } - CampaignId {campaign.Id}");
            using var connection = new MySqlConnection(sqlConnectionStr);
            connection.Open();
            var nowStr = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (campaign.PointRangeTo == 0) campaign.PointRangeTo = int.MaxValue;//
            var commandStr = $"INSERT INTO recordcustomerexport (CustomerMobileNo, CampaignID, DateExported, Status, LastUpdatedBy, LastUpdatedON) " +
                             $"SELECT l.CustomerMobileNo, {campaign.Id}, '{nowStr}', 1, {userId},'{nowStr}' " +
                             $"FROM leadmanagementreport l " +
                             $"WHERE l.TotalPoints >= {campaign.PointRangeFrom} and l.TotalPoints <= {campaign.PointRangeTo} " +
                             $"order by l.ID desc " +
                             $"limit {campaign.Amount}; ";
            using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }
            connection.Close();
            watch.Stop();
            logger.LogInformation($"Complete {nameof(ImplementUpdateLastUsedCampaign)}; Total time: {watch.Elapsed.TotalSeconds} s");
        }

        private async ValueTask ImplementUpdateLastUsedCampaignOnLeadManagement(CancellationToken token, string sqlConnectionStr, CampaignCreateOrEditDto campaign)
        {
            if (token.IsCancellationRequested) return;
            var watch = Stopwatch.StartNew();
            logger.LogInformation($"{nameof(ImplementUpdateLastUsedCampaignOnLeadManagement) } - CampaignId {campaign.Id}");
            using var connection = new MySqlConnection(sqlConnectionStr);
            connection.Open();
            var nowStr = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (campaign.PointRangeTo == 0) campaign.PointRangeTo = int.MaxValue;//
             var commandStr =   $"update leadmanagementreport " +
                                $"set ThirdLastUsedCampaignId = SecondLastUsedCampaignId, SecondLastUsedCampaignId = LastUsedCampaignId, LastUsedCampaignId = {campaign.Id}, " +
                                    $"DateLastExported = '{nowStr}', TotalTimesExported = TotalTimesExported + 1, " +
                                    $"ExportVsPointsPercentage = if (TotalPoints = 0, 'No Occurance', CONCAT(CEILING(TotalPoints / TotalTimesExported) * 100, '%')), " +
                                    $"ExportVsPointsNumber = TotalPoints - TotalTimesExported " +
                                $"order by ID desc " +
                                $"limit {campaign.Amount};";
            using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }
            connection.Close();
            watch.Stop();
            logger.LogInformation($"Complete {nameof(ImplementUpdateLastUsedCampaignOnLeadManagement)}; Total time: {watch.Elapsed.TotalSeconds} s");
        }


        private async ValueTask ImplementUpdateLastUsedCampaign(CancellationToken token, string sqlConnectionStr, ProcessLastUsedCampaign input)
        {
            if (token.IsCancellationRequested) return;
            if (!input.CustomerList.Any()) return;
            var listCount = input.CustomerList.Count();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            logger.LogInformation($"records count{listCount}");
            var timeCount = watch.Elapsed.TotalSeconds;
            var packageCount = (listCount - 1) / DEFAULT_BATCH_SIZE + 1;
            for (int i = 0; i < packageCount; i++)
            {
                watch.Restart();
                var itemCount = DEFAULT_BATCH_SIZE;
                if (i == packageCount - 1)
                    itemCount = listCount - i * DEFAULT_BATCH_SIZE;
                var rangeCustomerImports = input.CustomerList.GetRange(i * DEFAULT_BATCH_SIZE, itemCount);
                ProcessRangeReportItemsLastUsedCampaign(sqlConnectionStr, rangeCustomerImports, input.CampaignID);
                watch.Stop();
                timeCount += watch.Elapsed.TotalSeconds;
                logger.LogInformation($"ProcessRangeReportItems pack: {i}. Time: {watch.Elapsed.TotalSeconds} s");
            }
            if (input.ShouldSendEmail)
            {
                try
                {
                    var content = $"<p>Last Used Campaign has been successfully updated for {listCount} Customer Mobile Nos. Total time: {(int)timeCount} s.</p>" +
                                $"<p>The export data function is now updated with this new data.</p>";
                    await sendMail.SendMailAsync(new Models.SendMail.SendMailDto { ToAddresses = input.UserEmail, Subject = "Update LeadManagementReport - Assign Campaign", HtmlContent = content });
                }
                catch (Exception) { }
                if (hubContext.Clients != null && input.SignalRConnectionId.IsNotNullOrEmpty())
                {
                    var client = hubContext.Clients.Client(input.SignalRConnectionId);
                    if (client != null)
                    {
                        Console.WriteLine($"ClientProxy {client}");
                        var content = $"Last Used Campaign has been successfully updated for {listCount} Customer Mobile Nos. Total time: {(int)timeCount} s. " +
                                      $"The export data function is now updated with this new data.";
                        await client.SendAsync(HubClientName.CompleteAssignCampaign, new HubMessenger { Title = "Update LeadManagementReport", Content = content });
                    }
                }
                else
                    Console.WriteLine("No client");
            }
            logger.LogInformation($"Complete {nameof(ImplementUpdateLastUsedCampaign)}; Total time: {timeCount} s");
        }

        private void ProcessRangeReportItemsLastUsedCampaign(string sqlConnectionStr, List<long> customerMobileList, int campaignId)
        {
            var leadManagementReports = GetLeadManagementReports(sqlConnectionStr, customerMobileList.Select(p => new TempLeadManagementReport { CustomerMobileNo = p }));
            int? lastUsedCampaignId = null;
            int? secondLastUsedCampaignId = null;
            var now = DateTime.Now;
            foreach (var item in leadManagementReports)
            {
                item.DateLastExported = now;
                item.TotalTimesExported++;

                if (item.LastUsedCampaignId != campaignId)
                {
                    lastUsedCampaignId = item.LastUsedCampaignId;
                    secondLastUsedCampaignId = item.SecondLastUsedCampaignId;

                    item.LastUsedCampaignId = campaignId;
                    item.SecondLastUsedCampaignId = lastUsedCampaignId;
                    item.ThirdLastUsedCampaignId = secondLastUsedCampaignId;
                };
            }

            if (leadManagementReports.Any())
            {
                var dataTable = leadManagementReports.ToDataTable();
                using var connection = new MySqlConnection(sqlConnectionStr);
                connection.Open();
                var commandStr = "CREATE TEMPORARY TABLE IF NOT EXISTS temp_leadmanagementreport SELECT * FROM leadmanagementreport LIMIT 0;";
                using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                var bulkCopy = new MySqlBulkCopy(connection);
                bulkCopy.DestinationTableName = "temp_leadmanagementreport";
                bulkCopy.BulkCopyTimeout = 0;
                var result = bulkCopy.WriteToServer(dataTable);
                commandStr = "UPDATE leadmanagementreport AS l " +
                             "INNER JOIN temp_leadmanagementreport AS t " +
                             "ON l.CustomerMobileNo = t.CustomerMobileNo " +
                             "SET " +
                             "l.TotalTimesExported = t.TotalTimesExported, " +
                             "l.DateLastExported = t.DateLastExported, " +
                             "l.LastUsedCampaignId = t.LastUsedCampaignId, " +
                             "l.SecondLastUsedCampaignId = t.SecondLastUsedCampaignId, " +
                             "l.ThirdLastUsedCampaignId = t.ThirdLastUsedCampaignId, " +

                             "l.ExportVsPointsPercentage = t.ExportVsPointsPercentage, " +
                             "l.ExportVsPointsNumber = t.ExportVsPointsNumber"
                             ;
                using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                commandStr = "DROP TEMPORARY TABLE IF EXISTS temp_leadmanagementreport";
                using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private List<LeadManagementReport> GetLeadManagementReports(string sqlConnectionStr, IEnumerable<TempLeadManagementReport> items)
        {
            if (items is null || !items.Any())
                return new List<LeadManagementReport>();

            var dataTable = items.ToDataTable();
            using var connection = new MySqlConnection(sqlConnectionStr);
            connection.Open();
            var commandStr = "create temporary table IF NOT EXISTS TempLeadManagementReport(CustomerMobileNo BIGINT NOT NULL);";
            using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            var bulkCopy = new MySqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "TempLeadManagementReport";
            var result = bulkCopy.WriteToServer(dataTable);

            commandStr = "select l.* " +
                         "from leadmanagementreport  l " +
                         "inner join TempLeadManagementReport t " +
                         "on l.CustomerMobileNo = t.CustomerMobileNo";

            var data = connection.Query<LeadManagementReport>(commandStr).ToList();

            commandStr = "DROP TEMPORARY TABLE IF EXISTS TempLeadManagementReport";
            using (MySqlCommand myCmd = new MySqlCommand(commandStr, connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }
            connection.Close();
            return data;
        }

        private async ValueTask ImplementBulkInsertCustomerCampaignToMySQL(CancellationToken token, string sqlConnectionStr, IEnumerable<long> customerMobileList, int campaignID, int userId)
        {
            if (token.IsCancellationRequested) return;
            var now = DateTime.Now;
            //Console.WriteLine($"BulkInsertCustomerCampaignToMySQL: count {customerMobileList.Count()}");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var items = customerMobileList.Select(x => new RecordCustomerExport
            {
                CustomerMobileNo = x,
                CampaignID = campaignID,
                DateExported = now,
                Status = 1,
                LastUpdatedBy = userId,
                LastUpdatedON = now
            });
            var dataTable = items.ToDataTable();
            using var connection = new MySqlConnection(sqlConnectionStr);
            connection.Open();

            var bulkCopy = new MySqlBulkCopy(connection);
            bulkCopy.DestinationTableName = TableName.RecordCustomerExport;
            bulkCopy.BulkCopyTimeout = 0;
            await bulkCopy.WriteToServerAsync(dataTable);
            watch.Stop();
            Console.WriteLine($"Complete ImplementBulkInsertCustomerCampaignToMySQL: time {watch.Elapsed.TotalSeconds} s");
            await connection.CloseAsync();
        }
    }
}