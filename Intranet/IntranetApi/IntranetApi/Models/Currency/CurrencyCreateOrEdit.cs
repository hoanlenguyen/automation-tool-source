namespace IntranetApi.Models
{
    public class CurrencyCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public bool Status { get; set; }
    }
}