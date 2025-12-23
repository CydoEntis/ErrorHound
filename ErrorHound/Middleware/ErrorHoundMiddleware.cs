using System.Net;
using Microsoft.AspNetCore.Http;
using ErrorHound.Abstractions;
using ErrorHound.BuiltIn;
using ErrorHound.Core;
using Microsoft.Extensions.Logging;

namespace ErrorHound.Middleware;

/// <summary>
/// Middleware that catches and formats all exceptions in the ASP.NET Core pipeline.
/// </summary>
public sealed class ErrorHoundMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHoundMiddleware> _logger;
    private readonly IErrorResponseFormatter _formatter;

    /// <summary>
    /// Initializes a new instance of the ErrorHoundMiddleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger for recording error information.</param>
    /// <param name="formatter">The formatter to use for structuring error responses.</param>
    public ErrorHoundMiddleware(
        RequestDelegate next,
        ILogger<ErrorHoundMiddleware> logger,
        IErrorResponseFormatter formatter)
    {
        _next = next;
        _logger = logger;
        _formatter = formatter;
    }

    /// <summary>
    /// Processes the HTTP request and catches any exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationError ex)
        {
            _logger.LogWarning("Validation error: {Code} - {Message}", ex.Code, ex.Message);

            context.Response.StatusCode = ex.Status;
            context.Response.ContentType = "application/json";

            var response = _formatter.Format(ex);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (ApiError ex)
        {
            _logger.LogError("ApiError: {Code} - {Message}", ex.Code, ex.Message);

            context.Response.StatusCode = ex.Status;
            context.Response.ContentType = "application/json";

            var response = _formatter.Format(ex);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled exception occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var genericError = new InternalServerError(ex.Message);
            var response = _formatter.Format(genericError);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}