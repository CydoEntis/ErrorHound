namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a rate limiting error where too many requests were sent in a given time window.
/// </summary>
public sealed class TooManyRequestsError : ApiError
{
    public TooManyRequestsError(string? details = null)
        : base(ErrorCodes.TooManyRequests, ErrorMessages.TooManyRequests, (int)HttpStatusCode.TooManyRequests, details) { }
}
