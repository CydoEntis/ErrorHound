namespace ErrorHound.Core;

public static class ErrorMessages
{
    public const string Validation = "Validation failed";
    public const string BadRequest = "The request was invalid or malformed.";
    public const string Conflict = "The request could not be completed due to a conflict.";
    public const string Database = "A server error occurred while accessing the database.";
    public const string Forbidden = "You do not have permission to access this resource.";
    public const string InternalServer = "An unexpected internal server error occurred.";
    public const string NotFound = "The requested resource could not be found.";
    public const string ServiceUnavailable = "The service is unavailable.";
    public const string Timeout = "The request timed out while processing.";
    public const string TooManyRequests = "Too many requests have been made. Please try again later.";
    public const string Unauthorized = "Authentication is required to access this resource.";
}