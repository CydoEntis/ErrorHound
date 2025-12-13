namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a generic Not Found error
/// </summary>
public sealed class NotFoundError : ApiError
{
    public NotFoundError(string? details = null)
        : base(ErrorCodes.NotFound, ErrorMessages.NotFound, (int)HttpStatusCode.NotFound, details) { }
}