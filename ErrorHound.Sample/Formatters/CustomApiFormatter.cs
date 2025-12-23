using ErrorHound.Abstractions;
using ErrorHound.BuiltIn;
using ErrorHound.Core;

namespace ErrorHound.Sample.Formatters;

/// <summary>
/// Custom formatter that wraps errors in an envelope with metadata
/// Demonstrates dependency injection support
/// </summary>
public sealed class CustomApiFormatter : IErrorResponseFormatter
{
    private readonly ILogger<CustomApiFormatter> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomApiFormatter(
        ILogger<CustomApiFormatter> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public object Format(ApiError error)
    {
        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "unknown";

        _logger.LogDebug(
            "Formatting error {Code} with trace ID {TraceId}",
            error.Code,
            traceId);

        if (error is ValidationError validationError)
        {
            return new
            {
                success = false,
                error = new
                {
                    code = error.Code,
                    message = error.Message,
                    validationErrors = validationError.FieldErrors
                },
                meta = new
                {
                    timestamp = DateTime.UtcNow,
                    traceId,
                    version = "v1.0"
                }
            };
        }

        return new
        {
            success = false,
            error = new
            {
                code = error.Code,
                message = error.Message,
                details = error.Details
            },
            meta = new
            {
                timestamp = DateTime.UtcNow,
                traceId,
                version = "v1.0"
            }
        };
    }
}
