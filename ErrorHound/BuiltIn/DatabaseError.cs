namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a server-side database failure.
/// </summary>
public sealed class DatabaseError : ApiError
{
    public DatabaseError(string? details = null)
        : base(ErrorCodes.Database, ErrorMessages.Database, (int)HttpStatusCode.InternalServerError, details)
    {
    }
}