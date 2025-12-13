using System.Net;
using ErrorHound.BuiltIn;
using ErrorHound.Core;

namespace ErrorHound.Tests;

public class ValidationErrorTests
{
    [Fact]
    public void ValidationError_BasicBehavior_WorksCorrectly()
    {
        var defaultError = new ValidationError();
        Assert.NotNull(defaultError.FieldErrors);
        Assert.Empty(defaultError.FieldErrors);
        Assert.Equal(ErrorMessages.Validation, defaultError.Message);
        Assert.Equal(ErrorCodes.Validation, defaultError.Code);
        Assert.Equal((int)HttpStatusCode.BadRequest, defaultError.Status);

        defaultError.AddFieldError("Email", "Email is required");
        defaultError.AddFieldError("Email", "Email must be valid");
        defaultError.AddFieldError("Password", "Password is required");

        Assert.Equal(2, defaultError.FieldErrors["Email"].Count);
        Assert.Single(defaultError.FieldErrors["Password"]);

        var preExisting = new Dictionary<string, List<string>> { { "Name", new() { "Name is required" } } };
        var errorWithPreExisting = new ValidationError(fieldErrors: preExisting);
        errorWithPreExisting.AddFieldError("Name", "Name must be at least 3 characters");
        Assert.Equal(2, errorWithPreExisting.FieldErrors["Name"].Count);
    }
}