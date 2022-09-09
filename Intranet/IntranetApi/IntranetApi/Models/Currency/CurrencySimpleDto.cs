namespace IntranetApi.Models
{
    public class CurrencySimpleDto : BaseEntity
    {
        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencySymbol { get; set; }
    }
}