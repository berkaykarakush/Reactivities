namespace Application.Core
{
    // Custom exception class for representing application-specific exceptions: AppException
    public class AppException
    {
        public AppException(int statusCode, string message, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
        // Property to hold the HTTP status code associated with the exception
        public int StatusCode { get; set; }
        // Property to hold the main error message associated with the exception
        public string Message { get; set; }
        // Property to hold additional details or context about the exception (optional)
        public string Details { get; set; }
    }
}