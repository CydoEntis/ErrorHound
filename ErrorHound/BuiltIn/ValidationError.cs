using System.Collections.Generic;
using System.Net;
using ErrorHound.Core;

namespace ErrorHound.BuiltIn;

/// <summary>
/// Represents a validation error with support for multiple errors per field.
/// Standardized for consistent API responses.
/// </summary>
public sealed class ValidationError : ApiError
{
    /// <summary>
    /// Dictionary of field names to lists of error messages.
    /// </summary>
    public IDictionary<string, List<string>> FieldErrors { get; }

    /// <summary>
    /// Creates a new ValidationError.
    /// </summary>
    /// <param name="message">Optional overall message. Defaults to "Validation failed".</param>
    /// <param name="fieldErrors">Optional dictionary of field errors.</param>
    public ValidationError(
        string message = "Validation failed",
        IDictionary<string, List<string>>? fieldErrors = null
    ) : base(
        ErrorCodes.ValidationError,
        message,
        (int)HttpStatusCode.BadRequest,
        fieldErrors
    )
    {
        FieldErrors = fieldErrors ?? new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Adds an error message to a specific field.
    /// </summary>
    /// <param name="field">Field name</param>
    /// <param name="error">Error message</param>
    public void AddFieldError(string field, string error)
    {
        if (!FieldErrors.ContainsKey(field))
        {
            FieldErrors[field] = new List<string>();
        }

        FieldErrors[field].Add(error);
    }
}