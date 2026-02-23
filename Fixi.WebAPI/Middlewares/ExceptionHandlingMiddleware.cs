using Fixi.Core.DTOs.shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Fixi.WebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            try
            {
                return _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                ApiErrorResponse errorResponse = new ApiErrorResponse
                {
                    Message =  "Exception error",
                    Errors = new List<string> { ex.ToString() },
                    TraceId = httpContext.TraceIdentifier
                };
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return httpContext.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
