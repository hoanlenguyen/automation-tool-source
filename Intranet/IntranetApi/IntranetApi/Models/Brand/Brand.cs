using System.ComponentModel.DataAnnotations;

namespace IntranetApi.Models
{
    public class Brand : BaseAuditEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }
        public bool IsAllBrand { get; set; } = false;
        public virtual ICollection<BrandEmployee> BrandEmployees { get; set; } =new HashSet<BrandEmployee>();
    }
}
