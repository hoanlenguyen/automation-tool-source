using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Rank: BaseAuditEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }
    }
}
