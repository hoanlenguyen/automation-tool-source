namespace BITool.Models
{
    public abstract class BasePaging<T> where T : class
    {
        public BasePaging()
        {
        }

        public BasePaging(int totalItems, IReadOnlyList<T> items)
        {
            TotalItems = totalItems;
            Items = items;
        }

        public int TotalItems { get; set; }
        public IReadOnlyList<T> Items { get; set; }
    }

    public class PagedResultDto<T> : BasePaging<T> where T : class
    {
        public PagedResultDto():base()
        {
        }

        public PagedResultDto(int totalItems, IReadOnlyList<T> items) : base(totalItems, items)
        {
        }
    }
}