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

    private ErrorHoundMiddleware CreateMiddleware(RequestDelegate next)
        => new ErrorHoundMiddleware(next, null);

    private async Task TestMiddlewareThrowsAsync(
        HttpStatusCode expectedStatus,
        string expectedMessage,
        string expectedCode,
        Func<Exception> createException,
        string? expectedDetails = null)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream(); // ensure body is readable

        RequestDelegate next = (ctx) => throw createException();

        var middleware = CreateMiddleware(next);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)expectedStatus, context.Response.StatusCode);

        var body = await GetResponseBodyAsync(context);

        Assert.Contains(expectedCode, body);
        Assert.Contains(expectedMessage, body);

        if (expectedDetails is not null)
            Assert.Contains(expectedDetails, body);
    }

    [Fact]
    public async Task Middleware_Catches_BadRequestError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.BadRequest, ErrorMessages.BadRequest, ErrorCodes.BadRequest,
            () => new BadRequestError());

    [Fact]
    public async Task Middleware_Catches_ConflictError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.Conflict, ErrorMessages.Conflict, ErrorCodes.Conflict,
            () => new ConflictError());

    [Fact]
    public async Task Middleware_Catches_DatabaseError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.InternalServerError, ErrorMessages.Database,
            ErrorCodes.Database,
            () => new DatabaseError());

    [Fact]
    public async Task Middleware_Catches_ForbiddenError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.Forbidden, ErrorMessages.Forbidden, ErrorCodes.Forbidden,
            () => new ForbiddenError());

    [Fact]
    public async Task Middleware_Catches_InternalServerError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.InternalServerError, ErrorMessages.InternalServer,
            ErrorCodes.InternalServer,
            () => new InternalServerError("Something went wrong"), expectedDetails: "Something went wrong");

    [Fact]
    public async Task Middleware_Catches_NotFoundError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.NotFound, ErrorMessages.NotFound, ErrorCodes.NotFound,
            () => new NotFoundError());

    [Fact]
    public async Task Middleware_Catches_ServiceUnavailableError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.ServiceUnavailable, ErrorMessages.ServiceUnavailable,
            ErrorCodes.ServiceUnavailable,
            () => new ServiceUnavailableError());

    [Fact]
    public async Task Middleware_Catches_TimeoutError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.GatewayTimeout, ErrorMessages.Timeout, ErrorCodes.Timeout,
            () => new TimeoutError());

    [Fact]
    public async Task Middleware_Catches_TooManyRequestsError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.TooManyRequests, ErrorMessages.TooManyRequests,
            ErrorCodes.TooManyRequests,
            () => new TooManyRequestsError());

    [Fact]
    public async Task Middleware_Catches_UnauthorizedError()
        => await TestMiddlewareThrowsAsync(HttpStatusCode.Unauthorized, ErrorMessages.Unauthorized,
            ErrorCodes.Unauthorized,
            () => new UnauthorizedError());

    [Fact]
    public async Task Middleware_Catches_ValidationError()
        => await TestMiddlewareThrowsAsync(
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