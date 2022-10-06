namespace IntranetApi.Models
{
    public class PermissionBaseUIDto
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }

    public class PermissionUIDto : PermissionBaseUIDto
    {
        public IEnumerable<PermissionBaseUIDto> Children { get; set; }
    }
}