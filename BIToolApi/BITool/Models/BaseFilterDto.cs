namespace BITool.Models
{
    public abstract class BaseFilterDto
    {
        public int Page { get; set; } = 1;
        public int RowsPerPage { get; set; } = 1000;
        public int SkipCount => (Page - 1) * RowsPerPage;
        public string SortBy { get; set; } = "id";
        public string SortDirection { get; set; } = "desc";
        //public string Sorting => $"{SortBy} {SortDirection}".Trim();
               
    }
}
