namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents an authorization error where access is denied despite valid credentials.
/// </summary>
public sealed class ForbiddenError : ApiError
{
    public ForbiddenError(string? details = null)
        : base(
            code: ErrorCodes.Forbidden,
            message: "You do not have permission to access this resource.",
            status: (int)HttpStatusCode.Forbidden,
            details: details
        )
    {
    }
}