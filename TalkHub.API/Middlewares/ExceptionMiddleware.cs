using System.Net;
using System.Text.Json;
using TalkHub.Application.Common.Models;

namespace TalkHub.API.Middlewares;

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
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Đã xảy ra lỗi hệ thống.";
        var errors = new List<ApiError>();

        if (exception is KeyNotFoundException)
        {
            statusCode = (int)HttpStatusCode.NotFound;
            message = "Không tìm thấy tài nguyên yêu cầu.";
        }
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = (int)HttpStatusCode.Unauthorized;
            message = "Bạn không có quyền truy cập.";
        }
        else
        {
            errors.Add(new ApiError { Field = "System", Message = exception.Message });
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailureResponse(statusCode, message, errors);
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
