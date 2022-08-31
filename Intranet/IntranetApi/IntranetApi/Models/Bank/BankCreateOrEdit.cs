namespace IntranetApi.Models
{
    public class BankCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreationTime { get; set; }
    }
}