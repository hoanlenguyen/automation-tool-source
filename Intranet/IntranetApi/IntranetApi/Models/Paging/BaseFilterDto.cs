namespace IntranetApi.Models
{
    public abstract class BaseFilterDto
    {
        public string? Keyword { get; set; }
        public bool?  Status { get; set; }
        public int Page { get; set; } = 1;
        public int RowsPerPage { get; set; } = int.MaxValue;
        public int SkipCount => (Page - 1) * RowsPerPage;
        public string SortBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "desc";
    }
}
