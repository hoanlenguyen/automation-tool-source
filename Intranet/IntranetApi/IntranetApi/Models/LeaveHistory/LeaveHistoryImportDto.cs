using System.Text.Json.Serialization;

namespace IntranetApi.Models
{
    public class LeaveHistoryImportDto
    {
        [JsonPropertyName("Error Cells")]
        public string ErrorCells { get; set; }

        [JsonPropertyName("Error Details")]
        public string ErrorDetails { get; set; }

        [JsonPropertyName("Employee ID")]
        public string EmployeeCode { get; set; }

        [JsonIgnore]
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public int? CreatorUserId { get; set; }

        [JsonIgnore]
        public DateTime ImportedDate { get; set; }

        [JsonPropertyName("Paid MCs")]
        public string PaidMCsStr { get; set; }

        [JsonIgnore]
        public float PaidMCs { get; set; }


        [JsonPropertyName("Paid-Offs")]
        public string PaidOffsStr { get; set; }

        [JsonIgnore]
        public float PaidOffs { get; set; }


        [JsonPropertyName("Late")]
        public string LateStr { get; set; }

        [JsonIgnore]
        public float Late { get; set; }
    }
}