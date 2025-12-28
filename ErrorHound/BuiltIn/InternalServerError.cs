namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a generic internal server error.
/// </summary>
public sealed class InternalServerError : ApiError
{
    public InternalServerError(string? details = null)
        : base(ErrorCodes.InternalServer, ErrorMessages.InternalServer, (int)HttpStatusCode.InternalServerError, details)
    {
    }
}