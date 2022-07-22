namespace BITool.Enums
{
    public static class SortDirection
    {
        public const string ASC = "asc";
        public const string DESC = "desc";
    }

    public static class CacheKeys
    {
        public const string GetAdminScoresKey = "GetAdminScoresKey";
        public const string GetCampaignsDropdown = "GetCampaignsDropdown";
    }

    public static class ScoreTitleType
    {
        public const string Occurance = "Occurance";
        public const string Results = "Results";
    }

    public static class ImportNames
    {
        public const string ImportCustomerScore = "ImportCustomerScore";
        public const string ImportCleanedCustomerMobileNumber = "ImportCleanedCustomerMobileNumber";
    }

    public static class ExportVsPoints
    {
        public const string NoOccurance = "No Occurance";
        public const string NoExport = "No Export";
    }

    public static class TableName
    {
        public const string AdminScore = "AdminScore";
        public const string AdminCampaign = "AdminCampaign";
        public const string Customer = "Customer";
        public const string CustomerScore = "CustomerScore";
        public const string RecordCustomerExport = "RecordCustomerExport";
        public const string ImportDataHistory = "ImportDataHistory";
        public const string CleanDataHistory = "CleanDataHistory";
        public const string LeadManagementReport = "LeadManagementReport";
    }

    public static class StoredProcedureName
    {
        public const string GetCustomersByFilter = "SP_Filter_LeadManagementReport";
        public const string GetCustomerCountByFilter = "SP_Filter_LeadManagementReport_CountTotal";

        public const string GetImportDataHistoryByFilter = "SP_Filter_ImportDataHistory";
        public const string GetImportDataHistoryCountByFilter = "SP_Filter_ImportDataHistory_CountTotal";

        public const string GetCleanDataHistoryByFilter = "SP_Filter_CleanDataHistory";
        public const string GetCleanDataHistoryCountByFilter = "SP_Filter_CleanDataHistory_CountTotal";

        public const string GetTotalCountByPointsRange = "SP_Filter_GetTotalCountByRange";

        public const string GetSummarySourceReport = "SP_Filter_GetSummarySourceReport";

        public const string GetRecordCustomerExport = "SP_Filter_RecordCustomerExport";
    }

    public static class AzureBlobConfig
    {
        public const string ContainerName = "download";//only lowercase with no special characters.
        public const string ExportDataFolder = "exportdata";
    }

    public static class HubClientName
    {
        public const string GetConnectionId = "GetConnectionId";
        public const string CompleteImport = "CompleteImport";
        public const string CompleteAssignCampaign = "CompleteAssignCampaign";
    }
}