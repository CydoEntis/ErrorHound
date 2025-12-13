using System.Net;
using ErrorHound.BuiltIn;
using ErrorHound.Core;
using ErrorHound.Middleware;
using Microsoft.AspNetCore.Http;

namespace ErrorHound.Tests;

public class MiddlewareTests
{
    private async Task<string> GetResponseBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    private ErrorHoundMiddleware CreateMiddleware(RequestDelegate next, ErrorHoundOptions? options = null)
        => new ErrorHoundMiddleware(next, null, options);

    private async Task TestMiddlewareThrowsAsync(
        HttpStatusCode expectedStatus,
        string expectedMessage,
        string expectedCode,
        Func<Exception> createException,
        string? expectedDetails = null,
        ErrorHoundOptions? options = null)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) => throw createException();

        var middleware = CreateMiddleware(next, options);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)expectedStatus, context.Response.StatusCode);
        var body = await GetResponseBodyAsync(context);
        Assert.Contains(expectedCode, body);
        Assert.Contains(expectedMessage, body);
        if (expectedDetails is not null)
            Assert.Contains(expectedDetails, body);
    }

    [Theory]
    [InlineData(typeof(BadRequestError), HttpStatusCode.BadRequest, ErrorCodes.BadRequest, ErrorMessages.BadRequest)]
    [InlineData(typeof(ConflictError), HttpStatusCode.Conflict, ErrorCodes.Conflict, ErrorMessages.Conflict)]
    [InlineData(typeof(DatabaseError), HttpStatusCode.InternalServerError, ErrorCodes.Database, ErrorMessages.Database)]
    [InlineData(typeof(ForbiddenError), HttpStatusCode.Forbidden, ErrorCodes.Forbidden, ErrorMessages.Forbidden)]
    [InlineData(typeof(InternalServerError), HttpStatusCode.InternalServerError, ErrorCodes.InternalServer,
        ErrorMessages.InternalServer)]
    [InlineData(typeof(NotFoundError), HttpStatusCode.NotFound, ErrorCodes.NotFound, ErrorMessages.NotFound)]
    [InlineData(typeof(ServiceUnavailableError), HttpStatusCode.ServiceUnavailable, ErrorCodes.ServiceUnavailable,
        ErrorMessages.ServiceUnavailable)]
    [InlineData(typeof(TimeoutError), HttpStatusCode.GatewayTimeout, ErrorCodes.Timeout, ErrorMessages.Timeout)]
    [InlineData(typeof(TooManyRequestsError), HttpStatusCode.TooManyRequests, ErrorCodes.TooManyRequests,
        ErrorMessages.TooManyRequests)]
    [InlineData(typeof(UnauthorizedError), HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized,
        ErrorMessages.Unauthorized)]
    public async Task Middleware_Catches_AllBuiltInErrors(Type errorType, HttpStatusCode status, string code,
        string message)
    {
        await TestMiddlewareThrowsAsync(status, message, code,
            () => (Exception)Activator.CreateInstance(errorType, "details")!, "details");
    }

    [Fact]
    public async Task Middleware_Catches_ValidationError()
    {
        await TestMiddlewareThrowsAsync(
            HttpStatusCode.BadRequest,
            ErrorMessages.Validation,
            ErrorCodes.Validation,
            () =>
            {
                var v = new ValidationError();
                v.AddFieldError("Email", "Email is required");
                return v;
            },
            expectedDetails: "\"Email\":[\"Email is required\"]");
    }

    [Fact]
    public async Task Middleware_Catches_UnhandledException()
    {
        await TestMiddlewareThrowsAsync(
            HttpStatusCode.InternalServerError,
            ErrorMessages.InternalServer,
            ErrorCodes.InternalServer,
            () => new Exception("Something broke"),
            expectedDetails: "Something broke");
    }

    [Fact]
    public async Task Middleware_UsesCustomResponseWrapper()
    {
        var options = new ErrorHoundOptions
        {
            ResponseWrapper = (ex) => new
            {
                customCode = "CUSTOM_CODE",
                customMessage = "This is a custom message",
                status = 999
            }
        };

        await TestMiddlewareThrowsAsync(
            HttpStatusCode.InternalServerError,
            "This is a custom message",
            "CUSTOM_CODE",
            () => new NotFoundError("Extra details"),
            expectedDetails: null,
            options: options
        );
    }
}