namespace ErrorHound.Core;

/// <summary>
/// Standard error codes used by ErrorHound's built-in error types.
/// </summary>
public static class ErrorCodes
{
    /// <summary>Error code for validation failures.</summary>
    public const string Validation = "VALIDATION";

    /// <summary>Error code for resource not found.</summary>
    public const string NotFound = "NOT_FOUND";

    /// <summary>Error code for authentication required.</summary>
    public const string Unauthorized = "UNAUTHORIZED";

    /// <summary>Error code for internal server errors.</summary>
    public const string InternalServer = "INTERNAL_SERVER";

    /// <summary>Error code for forbidden access.</summary>
    public const string Forbidden = "FORBIDDEN";

    /// <summary>Error code for resource conflicts.</summary>
    public const string Conflict = "CONFLICT";

    /// <summary>Error code for database failures.</summary>
    public const string Database = "DATABASE";

    /// <summary>Error code for malformed requests.</summary>
    public const string BadRequest = "BAD_REQUEST";

    /// <summary>Error code for rate limit exceeded.</summary>
    public const string TooManyRequests = "TOO_MANY_REQUESTS";

    /// <summary>Error code for service unavailable.</summary>
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";

    /// <summary>Error code for request timeouts.</summary>
    public const string Timeout = "TIMEOUT";
}