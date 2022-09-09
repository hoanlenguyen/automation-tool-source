namespace IntranetApi.Models
{
    public class CurrencyList : BaseAuditEntity
    {
        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencySymbol { get; set; }
    }
}
