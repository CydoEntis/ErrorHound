using System.Net;
using ErrorHound.BuiltIn;
using ErrorHound.Core;

namespace ErrorHound.Tests;

public class BuiltInErrorTests
{
    [Theory]
    [InlineData(typeof(BadRequestError), ErrorCodes.BadRequest, ErrorMessages.BadRequest, HttpStatusCode.BadRequest)]
    [InlineData(typeof(ConflictError), ErrorCodes.Conflict, ErrorMessages.Conflict, HttpStatusCode.Conflict)]
    [InlineData(typeof(DatabaseError), ErrorCodes.Database, ErrorMessages.Database, HttpStatusCode.InternalServerError)]
    [InlineData(typeof(ForbiddenError), ErrorCodes.Forbidden, ErrorMessages.Forbidden, HttpStatusCode.Forbidden)]
    [InlineData(typeof(InternalServerError), ErrorCodes.InternalServer, ErrorMessages.InternalServer,
        HttpStatusCode.InternalServerError)]
    [InlineData(typeof(NotFoundError), ErrorCodes.NotFound, ErrorMessages.NotFound, HttpStatusCode.NotFound)]
    [InlineData(typeof(ServiceUnavailableError), ErrorCodes.ServiceUnavailable, ErrorMessages.ServiceUnavailable,
        HttpStatusCode.ServiceUnavailable)]
    [InlineData(typeof(TimeoutError), ErrorCodes.Timeout, ErrorMessages.Timeout, HttpStatusCode.GatewayTimeout)]
    [InlineData(typeof(TooManyRequestsError), ErrorCodes.TooManyRequests, ErrorMessages.TooManyRequests,
        HttpStatusCode.TooManyRequests)]
    [InlineData(typeof(UnauthorizedError), ErrorCodes.Unauthorized, ErrorMessages.Unauthorized,
        HttpStatusCode.Unauthorized)]
    public void BuiltInError_SetsPropertiesCorrectly(Type errorType, string expectedCode, string expectedMessage,
        HttpStatusCode expectedStatus)
    {
        string details = "Test details";
        var error = (ApiError)Activator.CreateInstance(errorType, details)!;

        Assert.Equal(expectedCode, error.Code);
        Assert.Equal(expectedMessage, error.Message);
        Assert.Equal((int)expectedStatus, error.Status);
        Assert.Equal(details, error.Details);
    }
}