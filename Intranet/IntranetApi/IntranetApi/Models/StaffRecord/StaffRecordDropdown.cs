namespace IntranetApi.Models
{
    public class StaffRecordDropdown: BaseDropdown
    {
        public string EmployeeCode { get; set; }
        public string FullName => $"{EmployeeCode} - {Name}";
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
