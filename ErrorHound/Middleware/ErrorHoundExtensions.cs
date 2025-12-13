using Microsoft.AspNetCore.Builder;

namespace ErrorHound.Middleware;

public static class ErrorHoundExtensions
{
    /// <summary>
    /// Registers ErrorHound middleware for classic ASP.NET Core applications (IApplicationBuilder).
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configureOptions">Optional configuration for ErrorHoundOptions.</param>
    /// <returns>The same application builder.</returns>
    public static IApplicationBuilder UseErrorHound(
        this IApplicationBuilder app,
        Action<ErrorHoundOptions>? configureOptions = null)
    {
        var options = new ErrorHoundOptions();
        configureOptions?.Invoke(options);

        return app.UseMiddleware<ErrorHoundMiddleware>(options);
    }

    /// <summary>
    /// Registers ErrorHound middleware for Minimal APIs (WebApplication).
    /// </summary>
    /// <param name="app">The minimal API application.</param>
    /// <param name="configureOptions">Optional configuration for ErrorHoundOptions.</param>
    /// <returns>The same WebApplication instance.</returns>
    public static WebApplication UseErrorHound(
        this WebApplication app,
        Action<ErrorHoundOptions>? configureOptions = null)
    {
        var options = new ErrorHoundOptions();
        configureOptions?.Invoke(options);

        app.UseMiddleware<ErrorHoundMiddleware>(options);
        return app;
    }
}