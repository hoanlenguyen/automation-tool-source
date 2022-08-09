namespace IntranetApi.Models
{
    public class EmployeeCheckUnique
    {
        public List<string> EmployeeCodes { get; set; }=new List<string>();
        public List<string> BackendUsers { get; set; } = new List<string>();
        public List<string> IdNumbers { get; set; } = new List<string>();
        public List<string> IntranetUsernames { get; set; } = new List<string>();

    }
}
