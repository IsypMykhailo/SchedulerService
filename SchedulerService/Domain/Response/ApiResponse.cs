using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SchedulerService.Domain.Response;

public struct ApiResponse
{
    public HttpStatusCode Status { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public object? Data { get; set; }
    
    public static ApiResponse ValidationFailed(ModelStateDictionary? modelState = null)
    {
        return new ApiResponse()
        {
            Status = HttpStatusCode.BadRequest,
            Success = false,
            Data = modelState?
                .Where(v => v.Value?.Errors.Count > 0)
                .Select(x => new
                {
                    x.Key, 
                    Errors = x.Value?.Errors.Select(err => err.ErrorMessage)
                })
        };
    }
}

public static class ResponseExtensions 
{
    public static ApiResponse Ok(this HttpResponse response, object? data = null)
    {
        response.StatusCode = (int)HttpStatusCode.OK;
        return new ApiResponse()
        {
            Status = HttpStatusCode.OK,
            Success = true,
            Data = data
        };
    }

    public static ApiResponse Created(this HttpResponse response, object? data = null)
    {
        response.StatusCode = (int)HttpStatusCode.Created;
        return new ApiResponse()
        {
            Status = HttpStatusCode.Created,
            Success = true,
            Data = data
        };
    }

    public static ApiResponse NotFound(this HttpResponse response, string? error = null)
    {
        response.StatusCode = (int)HttpStatusCode.NotFound;
        return new ApiResponse()
        {
            Status = HttpStatusCode.NotFound,
            Success = false,
            Error = error
        };
    }

    public static ApiResponse Forbid(this HttpResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.Forbidden;
        return new ApiResponse()
        {
            Status = HttpStatusCode.Forbidden,
            Success = false,
        };
    }

    public static ApiResponse ValidationFailed(this HttpResponse response, ModelStateDictionary? modelState = null)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        return ApiResponse.ValidationFailed(modelState);
    }

    public static ApiResponse Custom(this HttpResponse response, HttpStatusCode statusCode, object? data = null)
    {
        response.StatusCode = (int)statusCode;
        return new ApiResponse()
        {
            Status = statusCode,
            Success = (int)statusCode is >= 200 and <= 299,
            Data = data
        };
    }
}