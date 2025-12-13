namespace ErrorHound.Core;

/// <summary>
/// Base class for all errors in ErrorHound.
/// </summary>
public abstract class ApiError : Exception
{
    /// <summary>
    /// A unique error code to identify this error type.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// The HTTP status code associated with this error.
    /// </summary>
    public int Status { get; }

    /// <summary>
    /// Optional additional details about the error.
    /// </summary>
    public object? Details { get; }

    /// <summary>
    /// Constructs a new ApiError.
    /// </summary>
    /// <param name="code">Unique error code.</param>
    /// <param name="message">User-friendly error message.</param>
    /// <param name="status">HTTP status code.</param>
    /// <param name="details">Optional additional details.</param>
    protected ApiError(
        string code,
        string message,
        int status,
        object? details = null
    ) : base(message)
    {
        Code = code;
        Status = status;
        Details = details;
    }
}