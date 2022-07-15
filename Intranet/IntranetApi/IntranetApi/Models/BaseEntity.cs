using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }

    public abstract class BaseAuditEntity : BaseEntity
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}