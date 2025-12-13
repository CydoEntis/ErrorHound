namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a conflict with the current state of the resource.
/// </summary>
public sealed class ConflictError : ApiError
{
    public ConflictError(string? details = null)
        : base(
            code: ErrorCodes.Conflict,
            message: "The request could not be completed due to a conflict.",
            status: (int)HttpStatusCode.Conflict,
            details: details
        )
    {
    }
}