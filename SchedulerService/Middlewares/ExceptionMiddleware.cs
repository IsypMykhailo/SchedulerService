using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SchedulerService.Domain.Response;

namespace SchedulerService.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred while executing the request.");
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var apiResponse = new ApiResponse
        {
            Success = false,
            Error = exception.Message,
            Status = HttpStatusCode.InternalServerError
        };
        
        await context.Response.WriteAsJsonAsync(apiResponse);
    }
}