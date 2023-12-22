using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        // Adds pagination information to the HTTP response headers
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int TotalPages)
        {
            // Create an anonymous object representing pagination information
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                TotalPages
            };
            // Serialize the pagination information and add it to the "Pagination" header
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
            // Ensure that the "Pagination" header is exposed to clients
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}