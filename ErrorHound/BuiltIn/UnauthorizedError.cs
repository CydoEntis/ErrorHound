namespace ErrorHound.BuiltIn;

using System.Net;
using Core;

/// <summary>
/// Represents an authentication error where valid credentials are missing or invalid.
/// </summary>
public sealed class UnauthorizedError : ApiError
{
    public UnauthorizedError(string? details = null)
        : base(
            code: ErrorCodes.Unauthorized,
            message: "Authentication is required to access this resource.",
            status: (int)HttpStatusCode.Unauthorized,
            details: details
        )
    {
    }
}