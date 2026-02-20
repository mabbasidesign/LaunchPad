using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Middleware
{
    /// <summary>
    /// Global exception handling middleware that catches all unhandled exceptions
    /// and returns a consistent error response with appropriate HTTP status codes.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. Path: {Path}, Method: {Method}", 
                    context.Request.Path, context.Request.Method);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Message = "An internal server error occurred.",
                Details = exception.Message,
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case DbUpdateConcurrencyException dbConcurrencyEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Message = "Concurrency conflict occurred.";
                    response.Details = "The record has been modified by another user. Please refresh and try again.";
                    break;

                case ArgumentNullException argNullEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Invalid argument provided.";
                    response.Details = argNullEx.Message;
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Invalid argument provided.";
                    response.Details = argEx.Message;
                    break;

                case InvalidOperationException invOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Invalid operation.";
                    response.Details = invOpEx.Message;
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Resource not found.";
                    response.Details = keyNotFoundEx.Message;
                    break;

                case UnauthorizedAccessException unauthEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Unauthorized access.";
                    response.Details = unauthEx.Message;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An unexpected error occurred.";
                    response.Details = exception.InnerException?.Message ?? exception.Message;
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Standard error response model for API exceptions.
    /// </summary>
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Extension method to register the exception handling middleware.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
