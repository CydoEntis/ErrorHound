using System.Net;
using ErrorHound.Core;

namespace ErrorHound.BuiltIn;

/// <summary>
/// Represents a validation error that can contain multiple errors per field.
/// Intended to be serialized into a consistent API response by ErrorHound middleware.
/// </summary>
public sealed class ValidationError : ApiError
{
    /// <summary>
    /// Collection of validation errors grouped by field name.
    /// Each field may contain one or more error messages.
    /// </summary>
    public IDictionary<string, List<string>> FieldErrors { get; }

    /// <summary>
    /// Creates a new ValidationError.
    /// </summary>
    /// <param name="message">
    /// Optional high-level validation message. Defaults to "Validation failed".
    /// </param>
    /// <param name="fieldErrors">
    /// Optional pre-populated collection of field-level validation errors.
    /// </param>
    public ValidationError(string message = ErrorMessages.Validation,
        IDictionary<string, List<string>>? fieldErrors = null)
        : base(ErrorCodes.Validation, message, (int)HttpStatusCode.BadRequest)
    {
        FieldErrors = fieldErrors ?? new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Adds an error message to a specific field.
    /// </summary>
    /// <param name="field">Field name.</param>
    /// <param name="error">Validation error message.</param>
    public void AddFieldError(string field, string error)
    {
        if (!FieldErrors.TryGetValue(field, out var errors))
        {
            errors = new List<string>();
            FieldErrors[field] = errors;
        }

        errors.Add(error);
    }
}