namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a generic internal server error.
/// </summary>
public class InternalServerError : ApiError
{
    public InternalServerError(string? details = null)
        : base(
            code: ErrorCodes.InternalServerError,
            message: "An unexpected error occurred",
            status: (int)HttpStatusCode.InternalServerError,
            details: details
        )
    {
    }
}