using System.Text.Json;
using CleanArchitecture.Application.Common;

namespace CleanArchitecture.Api.Middleware;

/// <summary>
/// Global error handling: translates application exceptions into consistent
/// JSON error responses instead of leaking stack traces to clients.
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
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, "Resource not found", ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Validation failed",
                string.Join(" ", ex.Errors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                "Internal server error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string title, string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = JsonSerializer.Serialize(new
        {
            title,
            status = statusCode,
            detail
        });

        await context.Response.WriteAsync(payload);
    }
}
