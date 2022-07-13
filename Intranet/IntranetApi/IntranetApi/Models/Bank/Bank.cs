using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Bank: BaseAuditEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }
    }
}
