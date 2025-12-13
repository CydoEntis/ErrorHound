using System.Net;
using ErrorHound.Core;

namespace ErrorHound.BuiltIn;

/// <summary>
/// Represents a malformed or invalid client request.
/// </summary>
public sealed class BadRequestError : ApiError
{
    public BadRequestError(string? details = null)
        : base(ErrorCodes.BadRequest, ErrorMessages.BadRequest, (int)HttpStatusCode.BadRequest, details)
    {
    }
}