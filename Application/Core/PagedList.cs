using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
    // Represents a paged list, extending the List<T> class
    public class PagedList<T> : List<T>
    {
        // Constructor for creating a paged list with provided items, count, page number, and page size
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }
        public int CurrentPage { get; set; } // The current page number in the paged list
        public int TotalPages { get; set; } // The total number of pages in the paged list

        public int PageSize { get; set; } // The total count of items in the paged list
        public int TotalCount { get; set; } // The total count of items in the paged list
        // Asynchronous method to create a paged list from a source IQueryable with a given page number and page size
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // Count the total number of items in the source IQueryable
            var count = await source.CountAsync();
            // Retrieve items for the specified page and page size from the source IQueryable
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            // Create and return a new instance of the PagedList<T> class with the retrieved items, count, page number, and page size
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}