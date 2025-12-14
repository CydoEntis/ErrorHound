<div align="center">
  <img src="./.github/assets/logo.png" alt="ErrorHound Logo" width="200"/>

  # ErrorHound

  ![License](https://img.shields.io/badge/license-MIT-green)
  ![NuGet](https://img.shields.io/nuget/v/ErrorHound)
  ![Downloads](https://img.shields.io/nuget/dt/ErrorHound)
</div>

**ErrorHound** is a lightweight, flexible error-handling middleware for ASP.NET Core applications. It provides consistent, standardized error responses across your entire API, with built-in support for common HTTP errors, field-level validation, and fully customizable response formatting.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Setup Guide](#setup-guide)
  - [Minimal APIs](#minimal-apis-webapplication)
  - [Classic ASP.NET Core](#classic-aspnet-core-iapplicationbuilder)
- [Built-in Errors](#built-in-errors)
- [Usage Examples](#usage-examples)
  - [Basic Error Handling](#basic-error-handling)
  - [Validation Errors](#validation-errors)
  - [Adding Details to Errors](#adding-details-to-errors)
  - [Custom Response Wrapper](#custom-response-wrapper)
- [Real-World Scenarios](#real-world-scenarios)
- [API Reference](#api-reference)
- [Testing](#testing)
- [Best Practices](#best-practices)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- **11 Built-in API errors** covering common HTTP status codes (400, 401, 403, 404, 409, 429, 500, 503, 504)
- **Field-level validation errors** with support for multiple errors per field
- **Automatic logging** of all exceptions with appropriate log levels
- **Consistent JSON responses** across your entire API
- **Custom response wrappers** for complete control over error response format
- **Minimal setup** - works with both Minimal APIs and classic ASP.NET Core
- **Zero dependencies** beyond ASP.NET Core
- **Fully tested** with comprehensive xUnit test coverage

---

## Installation

Install ErrorHound via NuGet Package Manager:

```bash
dotnet add package ErrorHound
```

Or via Package Manager Console:

```powershell
Install-Package ErrorHound
```

---

## Quick Start

Here's the simplest way to get started with ErrorHound:

```csharp
using ErrorHound.Middleware;
using ErrorHound.BuiltIn;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Add ErrorHound middleware (must be early in the pipeline)
app.UseErrorHound();

app.MapGet("/users/{id}", (int id) =>
{
    if (id <= 0)
        throw new BadRequestError("User ID must be greater than 0");

    if (id > 1000)
        throw new NotFoundError($"User with ID {id} not found");

    return new { Id = id, Name = "John Doe" };
});

app.Run();
```

**Response when `id = -1`:**
```json
{
  "code": "BAD_REQUEST",
  "message": "The request was invalid or malformed.",
  "status": 400,
  "details": "User ID must be greater than 0"
}
```

---

## Setup Guide

### Minimal APIs (WebApplication)

For modern ASP.NET Core applications using Minimal APIs:

```csharp
using ErrorHound.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Add ErrorHound early in the middleware pipeline
// Place it before routing, authentication, and other middleware
app.UseErrorHound();

// Optional: Configure ErrorHound with custom options
// app.UseErrorHound(options =>
// {
//     options.ResponseWrapper = (error) => new
//     {
//         errorCode = error.Code,
//         errorMessage = error.Message,
//         httpStatus = error.Status
//     };
// });

app.MapGet("/", () => "Hello World");

app.Run();
```

### Classic ASP.NET Core (IApplicationBuilder)

For traditional ASP.NET Core applications with controllers:

```csharp
using ErrorHound.Middleware;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // Add other services...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Add ErrorHound as the first middleware
        app.UseErrorHound();

        // Or with custom configuration
        // app.UseErrorHound(options =>
        // {
        //     options.ResponseWrapper = (error) => new
        //     {
        //         success = false,
        //         error = error.Message,
        //         code = error.Code
        //     };
        // });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

**Important:** Always place `UseErrorHound()` **early** in your middleware pipeline, before routing, authentication, and authorization.

---

## Built-in Errors

ErrorHound provides 11 ready-to-use error types:

| Error Class               | HTTP Status | Error Code            | Default Message                                             |
|---------------------------|-------------|-----------------------|-------------------------------------------------------------|
| `BadRequestError`         | 400         | `BAD_REQUEST`         | "The request was invalid or malformed."                     |
| `UnauthorizedError`       | 401         | `UNAUTHORIZED`        | "Authentication is required to access this resource."       |
| `ForbiddenError`          | 403         | `FORBIDDEN`           | "You do not have permission to access this resource."       |
| `NotFoundError`           | 404         | `NOT_FOUND`           | "The requested resource could not be found."                |
| `ConflictError`           | 409         | `CONFLICT`            | "The request could not be completed due to a conflict."     |
| `TooManyRequestsError`    | 429         | `TOO_MANY_REQUESTS`   | "Too many requests have been made. Please try again later." |
| `InternalServerError`     | 500         | `INTERNAL_SERVER`     | "An unexpected internal server error occurred."             |
| `DatabaseError`           | 500         | `DATABASE`            | "A server error occurred while accessing the database."     |
| `ServiceUnavailableError` | 503         | `SERVICE_UNAVAILABLE` | "The service is unavailable."                               |
| `TimeoutError`            | 504         | `TIMEOUT`             | "The request timed out while processing."                   |
| `ValidationError`         | 400         | `VALIDATION`          | "Validation failed"                                         |

---

## Usage Examples

### Basic Error Handling

Simply throw any built-in error from your endpoints or controllers:

```csharp
using ErrorHound.BuiltIn;

app.MapGet("/products/{id}", async (int id, ProductService productService) =>
{
    var product = await productService.GetByIdAsync(id);

    if (product == null)
        throw new NotFoundError($"Product with ID {id} does not exist");

    return Results.Ok(product);
});
```

**Response (404):**
```json
{
  "code": "NOT_FOUND",
  "message": "The requested resource could not be found.",
  "status": 404,
  "details": "Product with ID 123 does not exist"
}
```

### Validation Errors

`ValidationError` supports multiple errors per field for comprehensive form validation:

```csharp
using ErrorHound.BuiltIn;

app.MapPost("/register", (RegisterRequest request) =>
{
    var validation = new ValidationError();

    if (string.IsNullOrWhiteSpace(request.Email))
        validation.AddFieldError("Email", "Email is required");
    else if (!IsValidEmail(request.Email))
        validation.AddFieldError("Email", "Email must be a valid email address");

    if (string.IsNullOrWhiteSpace(request.Password))
        validation.AddFieldError("Password", "Password is required");
    else if (request.Password.Length < 8)
        validation.AddFieldError("Password", "Password must be at least 8 characters");

    if (string.IsNullOrWhiteSpace(request.Username))
        validation.AddFieldError("Username", "Username is required");

    if (validation.FieldErrors.Any())
        throw validation;

    // Process registration...
    return Results.Ok(new { message = "Registration successful" });
});
```

**Response (400):**
```json
{
  "code": "VALIDATION",
  "message": "Validation failed",
  "status": 400,
  "details": {
    "Email": [
      "Email is required"
    ],
    "Password": [
      "Password is required"
    ],
    "Username": [
      "Username is required"
    ]
  }
}
```

### Adding Details to Errors

All built-in errors accept an optional `details` parameter:

```csharp
// Simple details
throw new UnauthorizedError("Token has expired");

// Detailed object
throw new ConflictError(new
{
    conflictingField = "email",
    existingValue = "john@example.com",
    attemptedValue = "john@example.com"
});

// Multiple details
throw new BadRequestError(new
{
    invalidFields = new[] { "startDate", "endDate" },
    reason = "Start date must be before end date"
});
```

### Custom Response Wrapper

You have complete control over the error response structure:

```csharp
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        success = false,
        errorCode = error.Code,
        errorMessage = error.Message,
        timestamp = DateTime.UtcNow,
        data = error.Details
    };
});
```

**Custom Response Example:**
```json
{
  "success": false,
  "errorCode": "NOT_FOUND",
  "errorMessage": "The requested resource could not be found.",
  "timestamp": "2025-12-13T10:30:00Z",
  "data": "Product with ID 123 does not exist"
}
```

**Note:** When using a custom `ResponseWrapper`, the HTTP status code is always set to 500 (Internal Server Error) to prevent leaking error types through status codes. If you need different status codes, handle them within your wrapper.

---

## Real-World Scenarios

### E-Commerce API

```csharp
using ErrorHound.BuiltIn;

// Product not found
app.MapGet("/products/{id}", async (int id, IProductRepository repo) =>
{
    var product = await repo.FindByIdAsync(id);
    if (product == null)
        throw new NotFoundError($"Product {id} not found");

    return product;
});

// Insufficient stock
app.MapPost("/cart/add", async (AddToCartRequest request, ICartService cart) =>
{
    var available = await cart.CheckStockAsync(request.ProductId);
    if (available < request.Quantity)
        throw new ConflictError($"Only {available} items available in stock");

    await cart.AddItemAsync(request.ProductId, request.Quantity);
    return Results.Ok();
});

// Rate limiting
app.MapPost("/orders", async (CreateOrderRequest request, IRateLimiter limiter) =>
{
    if (!await limiter.AllowRequestAsync(request.UserId))
        throw new TooManyRequestsError("Order rate limit exceeded. Please try again in 1 minute.");

    // Process order...
    return Results.Created("/orders/123", new { orderId = 123 });
});
```

### User Authentication

```csharp
// Login endpoint
app.MapPost("/auth/login", async (LoginRequest request, IAuthService auth) =>
{
    var user = await auth.FindUserByEmailAsync(request.Email);
    if (user == null || !auth.VerifyPassword(user, request.Password))
        throw new UnauthorizedError("Invalid email or password");

    if (!user.IsEmailVerified)
        throw new ForbiddenError("Please verify your email before logging in");

    var token = auth.GenerateToken(user);
    return new { token, userId = user.Id };
});

// Protected endpoint
app.MapGet("/profile", async (HttpContext context, IUserService users) =>
{
    var userId = context.User.FindFirst("userId")?.Value;
    if (string.IsNullOrEmpty(userId))
        throw new UnauthorizedError("Authentication required");

    var profile = await users.GetProfileAsync(int.Parse(userId));
    return profile;
});
```

### Database Operations

```csharp
app.MapPost("/users", async (CreateUserRequest request, AppDbContext db) =>
{
    try
    {
        var user = new User { Name = request.Name, Email = request.Email };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/users/{user.Id}", user);
    }
    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
    {
        throw new ConflictError($"User with email {request.Email} already exists");
    }
    catch (DbUpdateException)
    {
        throw new DatabaseError("Failed to create user due to a database error");
    }
});
```

### External API Calls

```csharp
app.MapGet("/weather/{city}", async (string city, IWeatherApiClient weather) =>
{
    try
    {
        return await weather.GetWeatherAsync(city);
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
    {
        throw new ServiceUnavailableError("Weather service is temporarily unavailable");
    }
    catch (TaskCanceledException)
    {
        throw new TimeoutError("Weather service did not respond in time");
    }
});
```

---

## API Reference

### ErrorHoundOptions

Configuration options for ErrorHound middleware.

```csharp
public class ErrorHoundOptions
{
    // Custom function to wrap error responses
    public Func<ApiError, object>? ResponseWrapper { get; set; }
}
```

### ApiError Base Class

All ErrorHound exceptions inherit from `ApiError`:

```csharp
public abstract class ApiError : Exception
{
    public string Code { get; }        // Unique error code (e.g., "NOT_FOUND")
    public int Status { get; }         // HTTP status code (e.g., 404)
    public object? Details { get; }    // Optional additional details
}
```

### ValidationError

Special error type for field-level validation:

```csharp
public sealed class ValidationError : ApiError
{
    public IDictionary<string, List<string>> FieldErrors { get; }

    public void AddFieldError(string field, string error);
}
```

---

## Testing

ErrorHound is fully tested with xUnit. Run the test suite:

```bash
dotnet test
```

### Example Test

```csharp
[Fact]
public async Task Endpoint_Returns_NotFoundError()
{
    var response = await _client.GetAsync("/products/999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    var json = await response.Content.ReadAsStringAsync();
    var error = JsonSerializer.Deserialize<ErrorResponse>(json);

    Assert.Equal("NOT_FOUND", error.Code);
    Assert.Equal(404, error.Status);
}
```

---

## Best Practices

### 1. Place ErrorHound Early in the Pipeline

```csharp
app.UseErrorHound();  // First!
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

### 2. Use Specific Errors

Choose the most appropriate error type:

```csharp
// Good
throw new NotFoundError("User not found");

// Avoid generic errors
throw new BadRequestError("User not found"); // Wrong status code!
```

### 3. Provide Helpful Details

```csharp
// Good - actionable details
throw new BadRequestError("Start date must be before end date");

// Less helpful
throw new BadRequestError("Invalid input");
```

### 4. Don't Over-Expose Internal Details

```csharp
// Good - user-friendly
catch (SqlException)
{
    throw new DatabaseError("Failed to save user");
}

// Bad - exposes internal details
catch (SqlException ex)
{
    throw new DatabaseError(ex.Message); // May contain SQL details!
}
```

### 5. Use Validation Errors for Form Validation

```csharp
var validation = new ValidationError();

// Collect all validation errors before throwing
if (errors.Any())
    throw validation;
```

### 6. Log Before Throwing (if needed)

ErrorHound automatically logs all errors, but you can log additional context:

```csharp
_logger.LogWarning("Failed login attempt for user {Email}", email);
throw new UnauthorizedError("Invalid credentials");
```

---

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes
4. Ensure all tests pass (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to your branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Development Setup

```bash
git clone https://github.com/CydoEntis/ErrorHound.git
cd ErrorHound
dotnet restore
dotnet build
dotnet test
```

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Questions or Issues?

- Report bugs or request features via [GitHub Issues](https://github.com/CydoEntis/ErrorHound/issues)
- For questions, start a [Discussion](https://github.com/CydoEntis/ErrorHound/discussions)

---

Made with ❤️ by [Cydo](https://github.com/CydoEntis)
