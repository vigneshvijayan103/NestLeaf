using NestLeaf.Response;
using System.Net;
using System.Text.Json;

namespace NestLeaf.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
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
                _logger.LogError(ex, $"Unhandled Exception: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                StatusCode = (int)statusCode,
                ExceptionType = exception.GetType().Name,
                Message = $"Error: {exception.Message}"
            };

            var apiResponse = new ApiResponse<object>(
                false,
                errorResponse.Message,
                new
                {
                    errorResponse.StatusCode,
                    errorResponse.ExceptionType
                }
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(apiResponse);
            return context.Response.WriteAsync(json);
        }
    }
}
