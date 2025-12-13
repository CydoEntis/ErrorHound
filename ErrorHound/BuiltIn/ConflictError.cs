namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a conflict with the current state of the resource.
/// </summary>
public sealed class ConflictError : ApiError
{
    public ConflictError(string? details = null)
        : base(ErrorCodes.Conflict, ErrorMessages.Conflict, (int)HttpStatusCode.Conflict, details)
    {
    }
}