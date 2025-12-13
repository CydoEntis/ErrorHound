using ErrorHound.Core;

namespace ErrorHound.Middleware;

public class ErrorHoundOptions
{
    /// <summary>
    /// Optional delegate to wrap ApiError objects into a custom response object.
    /// If null, the default response structure is used.
    /// </summary>
    public Func<ApiError, object>? ResponseWrapper { get; set; }
}