namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a generic Not Found error
/// </summary>
public sealed class NotFoundError : ApiError
{
    public NotFoundError(string? details = null)
        : base(
            code: ErrorCodes.NotFound,
            message: "The data could not be found.",
            status: (int)HttpStatusCode.NotFound,
            details: details
        )
    {
    }
}