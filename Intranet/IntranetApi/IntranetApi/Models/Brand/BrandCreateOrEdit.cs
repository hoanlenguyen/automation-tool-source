namespace IntranetApi.Models
{
    public class BrandCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsAllBrand { get; set; } = false;
    }
}