namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents an authorization error where access is denied despite valid credentials.
/// </summary>
public sealed class ForbiddenError : ApiError
{
    public ForbiddenError(string? details = null)
        : base(ErrorCodes.Forbidden, ErrorMessages.Forbidden, (int)HttpStatusCode.Forbidden, details)
    {
    }
}