namespace ErrorHound.Abstractions;

using ErrorHound.Core;

/// <summary>
/// Defines a contract for formatting ApiError objects into response objects.
/// Implement this interface to customize how error responses are structured.
/// </summary>
public interface IErrorResponseFormatter
{
    /// <summary>
    /// Formats an ApiError into a response object that will be serialized to JSON.
    /// </summary>
    /// <param name="error">The error to format.</param>
    /// <returns>An object that will be serialized as the JSON response body.</returns>
    object Format(ApiError error);
}
