# ErrorHound

**ErrorHound** is a lightweight, configurable ASP.NET Core middleware for consistent API error handling. It standardizes error responses, supports built-in common errors, and allows custom response wrappers.

---

## Features

* Built-in API errors (`BadRequestError`, `NotFoundError`, `InternalServerError`, etc.)
* Field-level validation errors with `ValidationError`
* Minimal API and classic ASP.NET Core support
* Optional custom response wrapper for full flexibility
* Automatic logging of all errors

---

## Installation

```bash
# Using NuGet
dotnet add package ErrorHound
```

---

## Usage

### Minimal APIs (WebApplication)

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Use ErrorHound middleware
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        code = error.Code,
        message = error.Message,
        status = error.Status
    };
});

app.MapGet("/", () =>
{
    throw new NotFoundError("The resource could not be found");
});

app.Run();
```

### Classic ASP.NET Core (IApplicationBuilder)

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseErrorHound(); // optional: pass configuration

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

---

## Built-in Errors

| Error                     | HTTP Status | Code                  | Message                                                     |
| ------------------------- | ----------- | --------------------- | ----------------------------------------------------------- |
| `BadRequestError`         | 400         | `BAD_REQUEST`         | "The request was invalid or malformed."                     |
| `ConflictError`           | 409         | `CONFLICT`            | "The request could not be completed due to a conflict."     |
| `DatabaseError`           | 500         | `DATABASE`            | "A server error occurred while accessing the database."     |
| `ForbiddenError`          | 403         | `FORBIDDEN`           | "You do not have permission to access this resource."       |
| `InternalServerError`     | 500         | `INTERNAL_SERVER`     | "An unexpected internal server error occurred."             |
| `NotFoundError`           | 404         | `NOT_FOUND`           | "The requested resource could not be found."                |
| `ServiceUnavailableError` | 503         | `SERVICE_UNAVAILABLE` | "The service is unavailable."                               |
| `TimeoutError`            | 504         | `TIMEOUT`             | "The request timed out while processing."                   |
| `TooManyRequestsError`    | 429         | `TOO_MANY_REQUESTS`   | "Too many requests have been made. Please try again later." |
| `UnauthorizedError`       | 401         | `UNAUTHORIZED`        | "Authentication is required to access this resource."       |
| `ValidationError`         | 400         | `VALIDATION`          | "Validation failed" (supports multiple field errors)        |

---

## Validation Errors

`ValidationError` supports multiple errors per field:

```csharp
var validation = new ValidationError();
validation.AddFieldError("Email", "Email is required");
validation.AddFieldError("Email", "Email must be valid");
validation.AddFieldError("Password", "Password is required");

throw validation;
```

Response example:

```json
{
    "code": "VALIDATION",
    "message": "Validation failed",
    "status": 400,
    "details": {
        "Email": ["Email is required", "Email must be valid"],
        "Password": ["Password is required"]
    }
}
```

---

## Custom Response Wrapper

You can fully control the shape of error responses:

```csharp
app.UseErrorHound(options =>
{
    options.ResponseWrapper = (error) => new
    {
        customCode = error.Code,
        customMessage = $"Oops! {error.Message}",
        timestamp = DateTime.UtcNow
    };
});
```

---

## Testing

* All built-in errors and `ValidationError` are fully tested.
* Middleware behavior is covered for:

  * Standard built-in errors
  * Validation errors
  * Unhandled exceptions
  * Custom response wrapper

Run tests using xUnit:

```bash
dotnet test
```

---

## Logging

ErrorHound logs automatically using `ILogger`:

* `ValidationError` → `LogWarning`
* `ApiError` → `LogError`
* Other unhandled exceptions → `LogCritical`

---

## Contributing

* Fork the repo, make changes, and submit a PR.
* Tests are written in xUnit; run via `dotnet test`.
* Ensure any new built-in errors or middleware behavior are fully tested.

---

## License

MIT License.
