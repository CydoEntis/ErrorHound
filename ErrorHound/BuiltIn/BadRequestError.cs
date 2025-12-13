namespace ErrorHound.BuiltIn;

using System.Net;
using Core;

/// <summary>
/// Represents a malformed or invalid client request.
/// </summary>
public sealed class BadRequestError : ApiError
{
    public BadRequestError(string? details = null)
        : base(
            code: ErrorCodes.BadRequest,
            message: "The request was invalid or malformed.",
            status: (int)HttpStatusCode.BadRequest,
            details: details
        )
    {
    }
}