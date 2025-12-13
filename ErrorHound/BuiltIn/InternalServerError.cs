namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a generic internal server error.
/// </summary>
public sealed class InternalServerError : ApiError
{
    public InternalServerError(string? details = null)
        : base(
            code: ErrorCodes.InternalServer,
            message: "An unexpected internal server error occurred.",
            status: (int)HttpStatusCode.InternalServerError,
            details: details
        )
    {
    }
}