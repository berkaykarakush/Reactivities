using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;

        }
        // Middleware method to handle exceptions in the pipeline
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware component in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception using the provided logger
                _logger.LogError(ex, ex.Message);
                // Configure the response for an Internal Server Error
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // Create an AppException with details for the response
                var response = _env.IsDevelopment() ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) : new AppException(context.Response.StatusCode, "Intnernal Server Error");
                // Serialize the response to JSON with camelCase property naming
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                // Write the JSON response to the client
                await context.Response.WriteAsync(json);
            }
        }
    }
}