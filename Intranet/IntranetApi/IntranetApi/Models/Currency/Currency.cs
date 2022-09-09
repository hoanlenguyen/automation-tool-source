using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Currency : BaseAuditEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string? CurrencyCode { get; set; }

        [MaxLength(10)]
        public string CurrencySymbol { get; set; }
    }
}
