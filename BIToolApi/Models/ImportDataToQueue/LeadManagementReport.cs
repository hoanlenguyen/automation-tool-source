using BITool.Enums;

namespace BITool.Models.ImportDataToQueue
{
    public class LeadManagementReport
    {
        public int ID { get; set; }

        //Customer
        public string Source { get; set; }
        public long CustomerMobileNo { get; set; }
        public DateTime DateFirstAdded { get; set; }

        //CAMPAIGNS
        public int TotalTimesExported { get; set; }
        public DateTime? DateLastExported { get; set; }
        public int? LastUsedCampaignId { get; set; }
        public int? SecondLastUsedCampaignId { get; set; }
        public int? ThirdLastUsedCampaignId { get; set; }

        // OCCURANCE(INDICATORS)
        public DateTime? DateLastOccurred { get; set; }
        public int OccuranceTotalFirstScore { get; set; }
        public int OccuranceTotalSecondScore { get; set; }
        public int OccuranceTotalThirdScore { get; set; }
        public int OccuranceTotalFourthScore { get; set; }
        public int OccuranceTotalFifthScore { get; set; }

        public int TotalOccurancePoints => OccuranceTotalFirstScore + OccuranceTotalSecondScore +
                                           OccuranceTotalThirdScore + OccuranceTotalFourthScore + OccuranceTotalFifthScore;

        //RESULTS
        public int ResultsTotalFirstScore { get; set; }
        public int ResultsTotalSecondScore { get; set; }
        public int ResultsTotalThirdScore { get; set; }
        public int ResultsTotalFourthScore { get; set; }
        public int TotalResultsPoints => ResultsTotalFirstScore + ResultsTotalSecondScore +
                                         ResultsTotalThirdScore + ResultsTotalFourthScore;

        //ANALYSIS
        public int TotalPoints => TotalOccurancePoints + TotalResultsPoints;
        public string ExportVsPointsPercentage => TotalPoints == 0 ? 
            ExportVsPoints.NoOccurance : 
                TotalTimesExported==0?
                    ExportVsPoints.NoExport:
                    $"{(TotalPoints / TotalTimesExported)*100}%";
        public int ExportVsPointsNumber => TotalPoints - TotalTimesExported;
    }
}