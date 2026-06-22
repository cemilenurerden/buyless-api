using System.Text.Json;
using FluentValidation;

namespace InventoryService.API.Middleware;

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
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, new { errors });
        }
        catch (InvalidOperationException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status401Unauthorized, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen bir hata oluştu.");
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError,
                new { message = "Beklenmeyen bir hata oluştu." });
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, object body)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}