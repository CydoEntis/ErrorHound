namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a rate limiting error where too many requests were sent in a given time window.
/// </summary>
public sealed class TooManyRequestsError : ApiError
{
    public TooManyRequestsError(string? details = null)
        : base(
            code: ErrorCodes.TooManyRequests,
            message: "Too many requests have been made. Please try again later.",
            status: (int)HttpStatusCode.TooManyRequests,
            details: details
        )
    {
    }
}