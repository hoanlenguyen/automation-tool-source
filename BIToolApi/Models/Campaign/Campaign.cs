using System.ComponentModel.DataAnnotations;

namespace BITool.Models
{
    public class Campaign : BaseAuditEntity
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        [MaxLength(150)]
        public string? Brand { get; set; }
        [MaxLength(150)]
        public string? Channel { get; set; }
        public int Amount { get; set; }
        public int PointRangeFrom { get; set; }
        public int PointRangeTo { get; set; }
        [MaxLength(200)]
        public string? Remarks { get; set; }
    }
}
