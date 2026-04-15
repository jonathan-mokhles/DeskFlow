using Fixi.Core.DTOs.shared;
using Fixi.Core.Exceptions;
using System.ComponentModel.DataAnnotations;


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

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case TicketNotFoundException :
                    context.Response.StatusCode = 404;
                    response.Message = "Not found";
                    response.Errors = new List<string> { exception.Message };
                    break;

                case UnauthorizedTicketAccessException:
                    context.Response.StatusCode = 403;
                    response.Message = "UnAuthorized";
                    response.Errors = new List<string> { exception.Message };
                    break;

                case NotFoundException:
                    context.Response.StatusCode = 404;
                    response.Message = "Not found";
                    response.Errors = new List<string> { exception.Message };
                    break;

                case BusinessRuleViolationException:
                    context.Response.StatusCode = 400;
                    response.Message = "Business rule violation";
                    response.Errors = new List<string> { exception.Message };
                    break;

                case UnauthorizedOperationException:
                    context.Response.StatusCode = 403;
                    response.Message = "UnAuthorized";
                    response.Errors = new List<string> { exception.Message };
                    break;

                case ValidationException:
                    context.Response.StatusCode = 400;
                    response.Message = "Validation error";
                    response.Errors = new List<string> { exception.Message };
                    break;

                default:
                    context.Response.StatusCode = 500;
                    response.Message = "Internal server error";
                    response.Errors = new List<string> { exception.Message };
                    break;
            }
            return context.Response.WriteAsJsonAsync(response);
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
