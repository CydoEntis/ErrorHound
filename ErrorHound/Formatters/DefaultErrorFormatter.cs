namespace ErrorHound.Formatters;

using ErrorHound.Abstractions;
using ErrorHound.BuiltIn;
using ErrorHound.Core;

/// <summary>
/// The default error response formatter used by ErrorHound.
/// Formats errors into a consistent envelope structure with success, error, and meta properties.
/// </summary>
public sealed class DefaultErrorFormatter : IErrorResponseFormatter
{
    /// <summary>
    /// Formats an ApiError into the default response structure.
    /// Returns a consistent envelope format with success, error, and meta properties
    /// to match SuccessHound response structure.
    /// </summary>
    /// <param name="error">The error to format.</param>
    /// <returns>An anonymous object with success, error, and meta properties.</returns>
    public object Format(ApiError error)
    {
        if (error is ValidationError validationError)
        {
            return new
            {
                success = false,
                error = new
                {
                    code = error.Code,
                    message = error.Message,
                    details = validationError.FieldErrors
                },
                meta = new
                {
                    timestamp = DateTime.UtcNow,
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
                version = "v1.0"
            }
        };
    }
}
