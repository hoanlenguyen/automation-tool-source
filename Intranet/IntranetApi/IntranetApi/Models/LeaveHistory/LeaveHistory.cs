using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class LeaveHistory : BaseAuditEntity
    {
        [MaxLength(50)]
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ImportedDate { get; set; }
        public float PaidMCs { get; set; }
        public float PaidOffs { get; set; }
        public float Late { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual User Employee { get; set; }
    }
}