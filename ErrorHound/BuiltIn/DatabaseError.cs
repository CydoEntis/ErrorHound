namespace ErrorHound.BuiltIn;

using System.Net;
using ErrorHound.Core;

/// <summary>
/// Represents a server-side database failure.
/// </summary>
public sealed class DatabaseError : ApiError
{
    public DatabaseError(string? details = null)
        : base(
            code: ErrorCodes.Database,
            message: "A server error occurred while accessing the database.",
            status: (int)HttpStatusCode.InternalServerError,
            details: details
        )
    {
    }
}