using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class BrandEmployee
    {
        public int BrandId { get; set; }
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(BrandId))]
        public virtual Brand Brand { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
    }
}
