using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;


namespace DeskFlow.WebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
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
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", httpContext.TraceIdentifier);
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
                    break;

                case UnauthorizedTicketAccessException:
                    context.Response.StatusCode = 403;
                    response.Message = "UnAuthorized";
                    break;

                case NotFoundException:
                    context.Response.StatusCode = 404;
                    response.Message = "Not found";
                    break;

                case BusinessRuleViolationException:
                    context.Response.StatusCode = 400;
                    response.Message = "Business rule violation";
                    break;

                case UnauthorizedOperationException:
                    context.Response.StatusCode = 403;
                    response.Message = "Unauthorized";
                    break;

                case ValidationException:
                    context.Response.StatusCode = 400;
                    response.Message = "Validation error";
                    break;
                case ArgumentException:
                    context.Response.StatusCode = 400;
                    response.Message = "Invalid argument";
                    break;
                case InvalidOperationException:
                    context.Response.StatusCode = 400;
                    response.Message = "Invalid operation";
                    break;

                default:
                    context.Response.StatusCode = 500;
                    response.Message = "Internal server error";
                    response.Errors = new List<string> { "Internal server error occurred." };
                    break;
            }
            if(context.Response.StatusCode != 500)
            {
                response.Errors = new List<string> { exception.Message };
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
