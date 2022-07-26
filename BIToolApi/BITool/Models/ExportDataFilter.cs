namespace BITool.Models
{
    public class ExportDataFilter
    {
        public DateTime? DateFirstAddedFrom { get; set; }
        public DateTime? DateFirstAddedTo { get; set; }

        public int? TotalTimesExportedFrom { get; set; }
        public int? TotalTimesExportedTo { get; set; }

        public DateTime? DateLastExportedFrom { get; set; }
        public DateTime? DateLastExportedTo { get; set; }

        public string? Last3CampaignsUsed { get; set; }

        public DateTime? DateLastOccurredFrom { get; set; }
        public DateTime? DateLastOccurredTo { get; set; }

        public string? OccurredCategories { get; set; }
        public int? TotalOccurancePointsFrom { get; set; }
        public int? TotalOccurancePointsTo { get; set; }

        public string? ResultsCategories { get; set; }
        public int? TotalResultsPointsFrom { get; set; }
        public int? TotalResultsPointsTo { get; set; }

        public int? TotalPointsFrom { get; set; }
        public int? TotalPointsTo { get; set; }

        public int? ExportVsPointsPercentageFrom { get; set; }
        public int? ExportVsPointsPercentageTo { get; set; }
        public string? ExportVsPointsExceptions { get; set; }
        public int? ExportVsPointsNumberFrom { get; set; }
        public int? ExportVsPointsNumberTo { get; set; }

        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int? ExportTop { get; set; }
        public int ExportOffset { get; set; } = 0;
        public int ExportLimit { get; set; } = 0;

        public int? CampaignID { get; set; }
        public int? AssignedCampaignID { get; set; }
        public int? TotalCount { get; set; }

        public string? SignalRConnectionId { get; set; }
        public bool IsRemoveTaggedCampaign { get; set; }=false;
    }
}