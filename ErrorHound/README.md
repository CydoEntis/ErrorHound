<div align="center">
  <img src="./.github/assets/logo.png" alt="ErrorHound Logo" width="200" style="border-radius: 20px;"/>

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
- [Using All Error Types](#using-all-error-types)
- [Response Formats](#response-formats)
  - [Default Response Format](#default-response-format)
  - [Custom Response Wrapper](#custom-response-wrapper-1)
- [Validation Errors](#validation-errors-1)
- [Creating Custom Errors](#creating-custom-errors)
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

## Using All Error Types

Here's how to use each built-in error type with practical examples:

### BadRequestError (400)

Use when the client sends invalid or malformed data:

```csharp
app.MapPost("/products", (CreateProductRequest request) =>
{
    if (request.Price < 0)
        throw new BadRequestError("Price cannot be negative");

    if (string.IsNullOrWhiteSpace(request.Name))
        throw new BadRequestError("Product name is required");

    // Create product...
});
```

**Response:**
```json
{
  "code": "BAD_REQUEST",
  "message": "The request was invalid or malformed.",
  "status": 400,
  "details": "Price cannot be negative"
}
```

### UnauthorizedError (401)

Use when authentication is required or has failed:

```csharp
app.MapGet("/admin/dashboard", (HttpContext context) =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault();

    if (string.IsNullOrEmpty(token))
        throw new UnauthorizedError("Authentication token is required");

    if (!IsValidToken(token))
        throw new UnauthorizedError("Invalid or expired token");

    // Return dashboard data...
});
```

**Response:**
```json
{
  "code": "UNAUTHORIZED",
  "message": "Authentication is required to access this resource.",
  "status": 401,
  "details": "Invalid or expired token"
}
```

### ForbiddenError (403)

Use when a user is authenticated but lacks permission:

```csharp
app.MapDelete("/users/{id}", (int id, HttpContext context) =>
{
    var currentUserId = GetCurrentUserId(context);
    var currentUserRole = GetUserRole(context);

    if (currentUserRole != "Admin" && currentUserId != id)
        throw new ForbiddenError("You can only delete your own account");

    // Delete user...
});
```

**Response:**
```json
{
  "code": "FORBIDDEN",
  "message": "You do not have permission to access this resource.",
  "status": 403,
  "details": "You can only delete your own account"
}
```

### NotFoundError (404)

Use when a requested resource doesn't exist:

```csharp
app.MapGet("/orders/{id}", async (int id, IOrderRepository repo) =>
{
    var order = await repo.GetByIdAsync(id);

    if (order == null)
        throw new NotFoundError($"Order with ID {id} not found");

    return order;
});
```

**Response:**
```json
{
  "code": "NOT_FOUND",
  "message": "The requested resource could not be found.",
  "status": 404,
  "details": "Order with ID 123 not found"
}
```

### ConflictError (409)

Use when a request conflicts with current state:

```csharp
app.MapPost("/users/register", async (RegisterRequest request, IUserRepository repo) =>
{
    var existingUser = await repo.FindByEmailAsync(request.Email);

    if (existingUser != null)
        throw new ConflictError($"User with email {request.Email} already exists");

    // Create user...
});
```

**Response:**
```json
{
  "code": "CONFLICT",
  "message": "The request could not be completed due to a conflict.",
  "status": 409,
  "details": "User with email john@example.com already exists"
}
```

### TooManyRequestsError (429)

Use for rate limiting:

```csharp
app.MapPost("/api/send-email", async (EmailRequest request, IRateLimiter limiter) =>
{
    var allowed = await limiter.CheckRateLimitAsync(request.UserId, "email", max: 10, window: TimeSpan.FromHours(1));

    if (!allowed)
        throw new TooManyRequestsError("Email rate limit exceeded. Maximum 10 emails per hour.");

    // Send email...
});
```

**Response:**
```json
{
  "code": "TOO_MANY_REQUESTS",
  "message": "Too many requests have been made. Please try again later.",
  "status": 429,
  "details": "Email rate limit exceeded. Maximum 10 emails per hour."
}
```

### InternalServerError (500)

Use for unexpected server errors:

```csharp
app.MapGet("/reports/generate", () =>
{
    try
    {
        // Complex report generation...
        return GenerateReport();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Report generation failed");
        throw new InternalServerError("Failed to generate report. Please try again later.");
    }
});
```

**Response:**
```json
{
  "code": "INTERNAL_SERVER",
  "message": "An unexpected internal server error occurred.",
  "status": 500,
  "details": "Failed to generate report. Please try again later."
}
```

### DatabaseError (500)

Use for database-specific errors:

```csharp
app.MapPost("/orders", async (CreateOrderRequest request, AppDbContext db) =>
{
    try
    {
        var order = new Order { /* ... */ };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError(ex, "Database error creating order");
        throw new DatabaseError("Failed to create order due to a database error");
    }
});
```

**Response:**
```json
{
  "code": "DATABASE",
  "message": "A server error occurred while accessing the database.",
  "status": 500,
  "details": "Failed to create order due to a database error"
}
```

### ServiceUnavailableError (503)

Use when an external service is unavailable:

```csharp
app.MapGet("/payment/status/{id}", async (string id, IPaymentGateway gateway) =>
{
    try
    {
        return await gateway.GetPaymentStatusAsync(id);
    }
    catch (HttpRequestException)
    {
        throw new ServiceUnavailableError("Payment gateway is temporarily unavailable. Please try again later.");
    }
});
```

**Response:**
```json
{
  "code": "SERVICE_UNAVAILABLE",
  "message": "The service is unavailable.",
  "status": 503,
  "details": "Payment gateway is temporarily unavailable. Please try again later."
}
```

### TimeoutError (504)

Use when operations exceed time limits:

```csharp
app.MapGet("/analytics/report", async (IAnalyticsService analytics) =>
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    try
    {
        return await analytics.GenerateReportAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        throw new TimeoutError("Report generation timed out. Please try with a smaller date range.");
    }
});
```

**Response:**
```json
{
  "code": "TIMEOUT",
  "message": "The request timed out while processing.",
  "status": 504,
  "details": "Report generation timed out. Please try with a smaller date range."
}
```

---

## Response Formats

### Default Response Format

When you use ErrorHound **without** a custom response wrapper, all errors return in this standardized format:

```json
{
  "code": "ERROR_CODE",
  "message": "Human-readable error message",
  "status": 400,
  "details": "Optional additional details or null"
}
```

**Example with NotFoundError:**

```csharp
throw new NotFoundError("User with ID 123 not found");
```

```json
{
  "code": "NOT_FOUND",
  "message": "The requested resource could not be found.",
  "status": 404,
  "details": "User with ID 123 not found"
}
```

**Example with ValidationError:**

```csharp
var validation = new ValidationError();
validation.AddFieldError("Email", "Email is required");
validation.AddFieldError("Password", "Password must be at least 8 characters");
throw validation;
```

```json
{
  "code": "VALIDATION",
  "message": "Validation failed",
  "status": 400,
  "details": {
    "Email": ["Email is required"],
    "Password": ["Password must be at least 8 characters"]
  }
}
```

**Example without details:**

```csharp
throw new UnauthorizedError();
```

```json
{
  "code": "UNAUTHORIZED",
  "message": "Authentication is required to access this resource.",
  "status": 401,
  "details": null
}
```

### Custom Response Wrapper

You can completely customize the response structure using the `ResponseWrapper` option. This is useful for:
- Matching existing API response formats
- Adding additional metadata (timestamps, request IDs, etc.)
- Wrapping errors in a consistent envelope

#### Example 1: Simple Custom Format

```csharp
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        success = false,
        errorCode = error.Code,
        errorMessage = error.Message,
        timestamp = DateTime.UtcNow
    };
});
```

**When you throw:**
```csharp
throw new NotFoundError("User not found");
```

**Response becomes:**
```json
{
  "success": false,
  "errorCode": "NOT_FOUND",
  "errorMessage": "The requested resource could not be found.",
  "timestamp": "2025-12-13T15:30:00Z"
}
```

#### Example 2: Envelope Pattern with Metadata

```csharp
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        success = false,
        error = new
        {
            code = error.Code,
            message = error.Message,
            details = error.Details
        },
        meta = new
        {
            timestamp = DateTime.UtcNow,
            requestId = Guid.NewGuid().ToString(),
            version = "v1"
        }
    };
});
```

**Response:**
```json
{
  "success": false,
  "error": {
    "code": "BAD_REQUEST",
    "message": "The request was invalid or malformed.",
    "details": "Invalid product ID"
  },
  "meta": {
    "timestamp": "2025-12-13T15:30:00Z",
    "requestId": "550e8400-e29b-41d4-a716-446655440000",
    "version": "v1"
  }
}
```

#### Example 3: JSend-Style Responses

```csharp
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        status = "error",
        message = error.Message,
        code = error.Code,
        data = error.Details
    };
});
```

**Response:**
```json
{
  "status": "error",
  "message": "The requested resource could not be found.",
  "code": "NOT_FOUND",
  "data": "Product with ID 456 not found"
}
```

#### Example 4: Including HTTP Status in Body

```csharp
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        httpStatus = error.Status,
        errorCode = error.Code,
        errorMessage = error.Message,
        errorDetails = error.Details,
        timestamp = DateTime.UtcNow.ToString("o")
    };
});
```

**Response:**
```json
{
  "httpStatus": 409,
  "errorCode": "CONFLICT",
  "errorMessage": "The request could not be completed due to a conflict.",
  "errorDetails": "Email already exists",
  "timestamp": "2025-12-13T15:30:00.0000000Z"
}
```

**Important Note:** When using a custom `ResponseWrapper`, the HTTP status code from the original error is preserved. For example, a `NotFoundError` will still return HTTP 404, a `BadRequestError` will return HTTP 400, etc. Only the response body format changes based on your custom wrapper.

---

## Validation Errors

`ValidationError` is a special error type for handling form validation with support for multiple errors per field.

### Basic Usage

```csharp
app.MapPost("/users", (CreateUserRequest request) =>
{
    var validation = new ValidationError();

    if (string.IsNullOrWhiteSpace(request.Email))
        validation.AddFieldError("Email", "Email is required");

    if (string.IsNullOrWhiteSpace(request.Password))
        validation.AddFieldError("Password", "Password is required");

    if (validation.FieldErrors.Any())
        throw validation;

    // Create user...
});
```

**Default Response:**
```json
{
  "code": "VALIDATION",
  "message": "Validation failed",
  "status": 400,
  "details": {
    "Email": ["Email is required"],
    "Password": ["Password is required"]
  }
}
```

### Multiple Errors Per Field

You can add multiple validation errors to the same field:

```csharp
var validation = new ValidationError();

validation.AddFieldError("Email", "Email is required");
validation.AddFieldError("Email", "Email must be a valid email address");
validation.AddFieldError("Email", "Email domain is not allowed");

validation.AddFieldError("Password", "Password is required");
validation.AddFieldError("Password", "Password must be at least 8 characters");
validation.AddFieldError("Password", "Password must contain a number");

throw validation;
```

**Response:**
```json
{
  "code": "VALIDATION",
  "message": "Validation failed",
  "status": 400,
  "details": {
    "Email": [
      "Email is required",
      "Email must be a valid email address",
      "Email domain is not allowed"
    ],
    "Password": [
      "Password is required",
      "Password must be at least 8 characters",
      "Password must contain a number"
    ]
  }
}
```

### Custom Validation Message

You can override the default "Validation failed" message:

```csharp
var validation = new ValidationError("Registration validation failed");
validation.AddFieldError("Username", "Username is already taken");
throw validation;
```

**Response:**
```json
{
  "code": "VALIDATION",
  "message": "Registration validation failed",
  "status": 400,
  "details": {
    "Username": ["Username is already taken"]
  }
}
```

### Pre-populated Field Errors

You can create a `ValidationError` with pre-populated errors:

```csharp
var fieldErrors = new Dictionary<string, List<string>>
{
    ["Email"] = new List<string> { "Email is required" },
    ["Password"] = new List<string> { "Password is required", "Password is too short" }
};

throw new ValidationError("Form validation failed", fieldErrors);
```

---

## Creating Custom Errors

You can create your own error types by extending the `ApiError` base class. This is useful for domain-specific errors.

### Basic Custom Error

```csharp
using System.Net;
using ErrorHound.Core;

namespace MyApp.Errors;

public sealed class PaymentFailedError : ApiError
{
    public PaymentFailedError(string? details = null)
        : base("PAYMENT_FAILED",
               "Payment processing failed",
               (int)HttpStatusCode.PaymentRequired,
               details)
    {
    }
}
```

**Usage:**
```csharp
app.MapPost("/checkout", async (CheckoutRequest request, IPaymentService payment) =>
{
    var result = await payment.ProcessPaymentAsync(request);

    if (!result.Success)
        throw new PaymentFailedError(result.ErrorMessage);

    return Results.Ok(result);
});
```

**Response:**
```json
{
  "code": "PAYMENT_FAILED",
  "message": "Payment processing failed",
  "status": 402,
  "details": "Insufficient funds"
}
```

### Custom Error with Constants

For better organization, define error codes and messages as constants:

```csharp
using System.Net;
using ErrorHound.Core;

namespace MyApp.Errors;

public static class CustomErrorCodes
{
    public const string SubscriptionExpired = "SUBSCRIPTION_EXPIRED";
    public const string QuotaExceeded = "QUOTA_EXCEEDED";
}

public static class CustomErrorMessages
{
    public const string SubscriptionExpired = "Your subscription has expired";
    public const string QuotaExceeded = "You have exceeded your usage quota";
}

public sealed class SubscriptionExpiredError : ApiError
{
    public SubscriptionExpiredError(string? details = null)
        : base(CustomErrorCodes.SubscriptionExpired,
               CustomErrorMessages.SubscriptionExpired,
               (int)HttpStatusCode.PaymentRequired,
               details)
    {
    }
}

public sealed class QuotaExceededError : ApiError
{
    public QuotaExceededError(string? details = null)
        : base(CustomErrorCodes.QuotaExceeded,
               CustomErrorMessages.QuotaExceeded,
               (int)HttpStatusCode.TooManyRequests,
               details)
    {
    }
}
```

**Usage:**
```csharp
app.MapPost("/api/process", async (ProcessRequest request, ISubscriptionService subs) =>
{
    var subscription = await subs.GetSubscriptionAsync(request.UserId);

    if (subscription.IsExpired)
        throw new SubscriptionExpiredError($"Expired on {subscription.ExpiryDate:yyyy-MM-dd}");

    if (subscription.UsageCount >= subscription.Quota)
        throw new QuotaExceededError($"Limit: {subscription.Quota} requests per month");

    // Process request...
});
```

### Custom Error with Rich Details

```csharp
using System.Net;
using ErrorHound.Core;

namespace MyApp.Errors;

public sealed class BusinessRuleViolationError : ApiError
{
    public BusinessRuleViolationError(string rule, string reason, object? additionalData = null)
        : base("BUSINESS_RULE_VIOLATION",
               $"Business rule '{rule}' was violated",
               (int)HttpStatusCode.UnprocessableEntity,
               new
               {
                   rule,
                   reason,
                   additionalData
               })
    {
    }
}
```

**Usage:**
```csharp
app.MapPost("/orders", (CreateOrderRequest request) =>
{
    if (request.Items.Count > 100)
        throw new BusinessRuleViolationError(
            rule: "MaxOrderItems",
            reason: "Orders cannot contain more than 100 items",
            additionalData: new { maxItems = 100, requestedItems = request.Items.Count }
        );

    // Create order...
});
```

**Response:**
```json
{
  "code": "BUSINESS_RULE_VIOLATION",
  "message": "Business rule 'MaxOrderItems' was violated",
  "status": 422,
  "details": {
    "rule": "MaxOrderItems",
    "reason": "Orders cannot contain more than 100 items",
    "additionalData": {
      "maxItems": 100,
      "requestedItems": 150
    }
  }
}
```

### Custom Error with Parameterized Message

```csharp
using System.Net;
using ErrorHound.Core;

namespace MyApp.Errors;

public sealed class ResourceLockedError : ApiError
{
    public ResourceLockedError(string resourceType, string resourceId, string lockedBy)
        : base("RESOURCE_LOCKED",
               $"{resourceType} is currently locked by another user",
               (int)HttpStatusCode.Locked,
               new
               {
                   resourceType,
                   resourceId,
                   lockedBy,
                   lockedAt = DateTime.UtcNow
               })
    {
    }
}
```

**Usage:**
```csharp
app.MapPut("/documents/{id}", async (int id, UpdateDocumentRequest request, IDocumentService docs) =>
{
    var doc = await docs.GetAsync(id);

    if (doc.IsLocked && doc.LockedBy != request.UserId)
        throw new ResourceLockedError("Document", id.ToString(), doc.LockedBy);

    // Update document...
});
```

**Response:**
```json
{
  "code": "RESOURCE_LOCKED",
  "message": "Document is currently locked by another user",
  "status": 423,
  "details": {
    "resourceType": "Document",
    "resourceId": "42",
    "lockedBy": "user@example.com",
    "lockedAt": "2025-12-13T15:30:00Z"
  }
}
```

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
