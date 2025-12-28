namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a resource not found error.
/// </summary>
public sealed class NotFoundError : ApiError
{
    public NotFoundError(string? details = null)
        : base(ErrorCodes.NotFound, ErrorMessages.NotFound, (int)HttpStatusCode.NotFound, details)
    {
    }
}