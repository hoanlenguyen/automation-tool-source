using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class StaffRecordDocument : BaseEntity
    {
        public int StaffRecordId { get; set; }
        public string FileUrl { get; set; } = string.Empty;

        [ForeignKey(nameof(StaffRecordId))]
        public virtual StaffRecord StaffRecord { get; set; }
    }
}