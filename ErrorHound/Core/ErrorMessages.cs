namespace ErrorHound.Core;

/// <summary>
/// Default error messages used by ErrorHound's built-in error types.
/// </summary>
public static class ErrorMessages
{
    /// <summary>Default message for validation failures.</summary>
    public const string Validation = "Validation failed";

    /// <summary>Default message for malformed requests.</summary>
    public const string BadRequest = "The request was invalid or malformed.";

    /// <summary>Default message for resource conflicts.</summary>
    public const string Conflict = "The request could not be completed due to a conflict.";

    /// <summary>Default message for database failures.</summary>
    public const string Database = "A server error occurred while accessing the database.";

    /// <summary>Default message for forbidden access.</summary>
    public const string Forbidden = "You do not have permission to access this resource.";

    /// <summary>Default message for internal server errors.</summary>
    public const string InternalServer = "An unexpected internal server error occurred.";

    /// <summary>Default message for resource not found.</summary>
    public const string NotFound = "The requested resource could not be found.";

    /// <summary>Default message for service unavailable.</summary>
    public const string ServiceUnavailable = "The service is unavailable.";

    /// <summary>Default message for request timeouts.</summary>
    public const string Timeout = "The request timed out while processing.";

    /// <summary>Default message for rate limit exceeded.</summary>
    public const string TooManyRequests = "Too many requests have been made. Please try again later.";

    /// <summary>Default message for authentication required.</summary>
    public const string Unauthorized = "Authentication is required to access this resource.";
}