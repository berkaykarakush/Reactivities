namespace Application.Core
{
    // Represents parameters for paging in a query
    public class PagingParams
    {
        private const int MaxPageSize = 50;
        // The current page number, defaults to 1 if not specified
        public int pageNumber { get; set; } = 1;
        // The page size with a default value of 10, capped at a maximum value of MaxPageSize
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

    }
}