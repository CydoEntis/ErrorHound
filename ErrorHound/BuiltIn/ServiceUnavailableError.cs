namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a generic Bad Request error
/// </summary>
public sealed class ServiceUnavailableError : ApiError
{
    public ServiceUnavailableError(string? details = null)
        : base(ErrorCodes.ServiceUnavailable, ErrorMessages.ServiceUnavailable, (int)HttpStatusCode.ServiceUnavailable, details) { }
}