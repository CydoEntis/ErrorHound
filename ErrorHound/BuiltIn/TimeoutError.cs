namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a timeout while waiting for an upstream service or dependency.
/// </summary>
public sealed class TimeoutError : ApiError
{
    public TimeoutError(string? details = null)
        : base(
            code: ErrorCodes.Timeout,
            message: "The request timed out while processing.",
            status: (int)HttpStatusCode.GatewayTimeout,
            details: details
        )
    {
    }
}