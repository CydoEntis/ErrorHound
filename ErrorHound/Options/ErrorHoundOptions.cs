using ErrorHound.Abstractions;

namespace ErrorHound.Options;

/// <summary>
/// Configuration options for ErrorHound middleware.
/// </summary>
public sealed class ErrorHoundOptions
{
    /// <summary>
    /// Gets the type of the error response formatter.
    /// Internal use only - set via UseFormatter method.
    /// </summary>
    internal Type? FormatterType { get; private set; }

    /// <summary>
    /// Configures ErrorHound to use a specific formatter implementation.
    /// </summary>
    /// <typeparam name="T">The formatter type that implements IErrorResponseFormatter.</typeparam>
    public void UseFormatter<T>()
        where T : class, IErrorResponseFormatter
    {
        FormatterType = typeof(T);
    }
}