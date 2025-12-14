using System.Net;
using Microsoft.AspNetCore.Http;
using ErrorHound.BuiltIn;
using ErrorHound.Core;
using Microsoft.Extensions.Logging;

namespace ErrorHound.Middleware;

public class ErrorHoundMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHoundMiddleware> _logger;
    private readonly ErrorHoundOptions _options;

    public ErrorHoundMiddleware(
        RequestDelegate next,
        ILogger<ErrorHoundMiddleware> logger,
        ErrorHoundOptions? options = null)
    {
        _next = next;
        _logger = logger;
        _options = options ?? new ErrorHoundOptions();
    }

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

            var response = _options.ResponseWrapper?.Invoke(ex) ?? new
            {
                code = ex.Code,
                message = ex.Message,
                status = ex.Status,
                details = ex.FieldErrors
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (ApiError ex)
        {
            _logger.LogError("ApiError: {Code} - {Message}", ex.Code, ex.Message);

            context.Response.StatusCode = ex.Status;
            context.Response.ContentType = "application/json";

            var response = _options.ResponseWrapper?.Invoke(ex) ?? new
            {
                code = ex.Code,
                message = ex.Message,
                status = ex.Status,
                details = ex.Details
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled exception occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var genericError = new InternalServerError(ex.Message);

            var response = _options.ResponseWrapper?.Invoke(genericError) ?? new
            {
                code = genericError.Code,
                message = genericError.Message,
                status = genericError.Status,
                details = genericError.Details
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}