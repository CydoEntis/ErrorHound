namespace ErrorHound.Formatters;

using ErrorHound.Abstractions;
using ErrorHound.BuiltIn;
using ErrorHound.Core;

/// <summary>
/// The default error response formatter used by ErrorHound.
/// Formats errors into a consistent JSON structure with code, message, status, and details.
/// </summary>
public sealed class DefaultErrorFormatter : IErrorResponseFormatter
{
    /// <summary>
    /// Formats an ApiError into the default response structure.
    /// ValidationError instances use FieldErrors as details, while other errors use the Details property.
    /// </summary>
    /// <param name="error">The error to format.</param>
    /// <returns>An anonymous object with code, message, status, and details properties.</returns>
    public object Format(ApiError error)
    {
        // ValidationError has special handling to use FieldErrors as details
        if (error is ValidationError validationError)
        {
            return new
            {
                code = error.Code,
                message = error.Message,
                status = error.Status,
                details = validationError.FieldErrors
            };
        }

        return new
        {
            code = error.Code,
            message = error.Message,
            status = error.Status,
            details = error.Details
        };
    }
}
