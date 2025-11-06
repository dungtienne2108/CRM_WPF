namespace CRM.Domain.Filters.Base
{
    public class FilterBase
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool SortDescending { get; set; } = false;
    }
}
