using ErrorHound.Abstractions;
using ErrorHound.Middleware;
using ErrorHound.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorHound.Extensions;

public static class ErrorHoundExtensions
{
    /// <summary>
    /// Registers ErrorHound services in the dependency injection container.
    /// This must be called before UseErrorHound in your application startup.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for ErrorHoundOptions.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no formatter is configured.</exception>
    public static IServiceCollection AddErrorHound(
        this IServiceCollection services,
        Action<ErrorHoundOptions> configure)
    {
        var options = new ErrorHoundOptions();
        configure(options);

        if (options.FormatterType is null)
            throw new InvalidOperationException(
                "ErrorHound requires a formatter. Call options.UseFormatter<T>() in the configuration action.");

        services.AddSingleton(
            typeof(IErrorResponseFormatter),
            options.FormatterType);

        return services;
    }

    /// <summary>
    /// Registers ErrorHound middleware in the ASP.NET Core pipeline.
    /// Must be called after AddErrorHound during service registration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same application builder for chaining.</returns>
    public static IApplicationBuilder UseErrorHound(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHoundMiddleware>();
    }
}