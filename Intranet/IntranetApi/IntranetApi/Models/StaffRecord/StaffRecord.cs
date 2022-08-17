using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class StaffRecord : BaseAuditEntity
    {
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public int RecordType { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public virtual ICollection<StaffRecordDocument> StaffRecordDocuments { get; set; }
    }
}