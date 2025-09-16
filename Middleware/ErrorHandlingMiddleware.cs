using System.Net;
using System.Text.Json;

namespace LeaveManagementSystem.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                break;
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;
        }

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        if (_env.IsDevelopment())
        {
            result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Message = exception.Message,
                Detail = exception.StackTrace
            });
        }
        else
        {
            result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Message = "An error occurred. Please try again later."
            });
        }

        return context.Response.WriteAsync(result);
    }
}