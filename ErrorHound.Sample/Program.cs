using ErrorHound.BuiltIn;
using ErrorHound.Extensions;
using ErrorHound.Formatters;
using ErrorHound.Sample.Errors;
using ErrorHound.Sample.Formatters;
using ErrorHound.Sample.Models;

var builder = WebApplication.CreateBuilder(args);

// Required for CustomApiFormatter if using it
builder.Services.AddHttpContextAccessor();

// ==========================================
// STEP 1: Register ErrorHound Services
// ==========================================
// Register ErrorHound and configure which formatter to use.
// The formatter controls the structure of your error responses.

// OPTION 1: Use DefaultErrorFormatter (recommended - currently active)
// This provides a consistent envelope format that matches SuccessHound:
// {
//   "success": false,
//   "error": { "code": "...", "message": "...", "details": "..." },
//   "meta": { "timestamp": "...", "version": "..." }
// }
builder.Services.AddErrorHound(options =>
{
    options.UseFormatter<DefaultErrorFormatter>();
});

// OPTION 2: Use a custom formatter (uncomment to use)
// Create your own formatter by implementing IErrorResponseFormatter.
// See CustomApiFormatter.cs for an example that adds traceId to metadata.
// builder.Services.AddErrorHound(options =>
// {
//     options.UseFormatter<CustomApiFormatter>();
// });

var app = builder.Build();

// ==========================================
// STEP 2: Add ErrorHound Middleware
// ==========================================
// IMPORTANT: Add this EARLY in your middleware pipeline, before any endpoints.
// ErrorHound will catch all unhandled exceptions and format them consistently.
app.UseErrorHound();

var users = new List<User>();
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 5 },
    new() { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 50 },
    new() { Id = 3, Name = "Keyboard", Price = 79.99m, Stock = 0 }
};

var loginAttempts = new Dictionary<string, List<DateTime>>();

// ==========================================
// WELCOME ENDPOINT
// ==========================================
app.MapGet("/", () => new
{
    message = "ErrorHound Sample API",
    version = "2.0.0",
    endpoints = new
    {
        builtInErrors = "/api/demo",
        customErrors = "/api/custom",
        users = "/api/users",
        products = "/api/products",
        auth = "/api/auth"
    }
});

// ==========================================
// BUILT-IN ERROR DEMONSTRATIONS
// ==========================================
// ErrorHound provides ready-to-use error types for common HTTP status codes.
// Simply throw these errors anywhere in your application, and ErrorHound
// will automatically catch and format them into consistent responses.
//
// Available built-in errors:
// - BadRequestError (400)
// - UnauthorizedError (401)
// - ForbiddenError (403)
// - NotFoundError (404)
// - ConflictError (409)
// - TooManyRequestsError (429)
// - InternalServerError (500)
// - DatabaseError (500)
// - ServiceUnavailableError (503)
// - TimeoutError (504)
//
// Each error accepts an optional 'details' parameter for additional context.

app.MapGet("/api/demo/bad-request", () =>
{
    // Throw a built-in error with optional details
    throw new BadRequestError("This is a simulated bad request error");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/unauthorized", () =>
{
    throw new UnauthorizedError("You must be logged in to access this resource");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/forbidden", () =>
{
    throw new ForbiddenError("You don't have permission to access this resource");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/not-found", () =>
{
    throw new NotFoundError("The requested resource was not found");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/conflict", () =>
{
    throw new ConflictError("A resource with this identifier already exists");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/rate-limit", () =>
{
    throw new TooManyRequestsError("Too many requests. Please try again in 60 seconds.");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/internal-error", () =>
{
    throw new InternalServerError("An unexpected server error occurred");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/database-error", () =>
{
    throw new DatabaseError("Failed to connect to the database");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/service-unavailable", () =>
{
    throw new ServiceUnavailableError("The payment gateway is temporarily unavailable");
})
.WithTags("Demo - Built-in Errors");

app.MapGet("/api/demo/timeout", () =>
{
    throw new TimeoutError("The request timed out after 30 seconds");
})
.WithTags("Demo - Built-in Errors");

// ValidationError is a special built-in error for handling field-level validation.
// It allows you to add multiple errors per field and automatically formats them.
app.MapPost("/api/demo/validation", (CreateUserRequest request) =>
{
    // Create a new ValidationError instance
    var validation = new ValidationError();

    // Add field-level errors using AddFieldError(fieldName, errorMessage)
    // You can add multiple errors to the same field
    if (string.IsNullOrWhiteSpace(request.Email))
        validation.AddFieldError("Email", "Email is required");
    else if (!request.Email.Contains('@'))
        validation.AddFieldError("Email", "Email must be valid");

    if (string.IsNullOrWhiteSpace(request.Password))
        validation.AddFieldError("Password", "Password is required");
    else
    {
        if (request.Password.Length < 8)
            validation.AddFieldError("Password", "Password must be at least 8 characters");
        if (!request.Password.Any(char.IsDigit))
            validation.AddFieldError("Password", "Password must contain at least one number");
    }

    if (string.IsNullOrWhiteSpace(request.Name))
        validation.AddFieldError("Name", "Name is required");

    // Throw the ValidationError if any errors were added
    if (validation.FieldErrors.Any())
        throw validation;

    return Results.Ok(new { message = "Validation passed!" });
})
.WithTags("Demo - Built-in Errors");

// ==========================================
// CUSTOM ERROR DEMONSTRATIONS
// ==========================================
// You can create custom errors by extending the ApiError base class.
// This allows you to define domain-specific errors with custom codes,
// messages, HTTP status codes, and structured details.
//
// To create a custom error:
// 1. Create a class that inherits from ApiError
// 2. Pass your custom code, message, status, and details to the base constructor
//
// See the Errors/CustomErrors.cs file for examples.

app.MapGet("/api/custom/email-not-verified", () =>
{
    // Throw a custom error defined in your application
    throw new EmailNotVerifiedError("user@example.com");
})
.WithTags("Demo - Custom Errors");

app.MapGet("/api/custom/subscription-expired", () =>
{
    throw new SubscriptionExpiredError(DateTime.UtcNow.AddDays(-30));
})
.WithTags("Demo - Custom Errors");

app.MapGet("/api/custom/rate-limit", () =>
{
    var retryAfter = DateTime.UtcNow.AddMinutes(5);
    throw new RateLimitExceededError(
        maxRequests: 100,
        window: TimeSpan.FromHours(1),
        retryAfter: retryAfter);
})
.WithTags("Demo - Custom Errors");

app.MapGet("/api/custom/insufficient-stock", () =>
{
    throw new InsufficientStockError("Laptop", requested: 10, available: 3);
})
.WithTags("Demo - Custom Errors");

// ==========================================
// REALISTIC USER ENDPOINTS
// ==========================================
// These endpoints demonstrate real-world usage patterns:
// - Using NotFoundError when a resource doesn't exist
// - Using ValidationError for request validation
// - Using ConflictError when a duplicate resource is created

app.MapGet("/api/users", () => Results.Ok(users))
    .WithTags("Users");

app.MapGet("/api/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null)
        throw new NotFoundError($"User with ID {id} not found");

    return Results.Ok(user);
})
.WithTags("Users");

app.MapPost("/api/users", (CreateUserRequest request) =>
{
    // Validation
    var validation = new ValidationError();

    if (string.IsNullOrWhiteSpace(request.Email))
        validation.AddFieldError("Email", "Email is required");
    else if (!request.Email.Contains('@'))
        validation.AddFieldError("Email", "Email must be a valid email address");

    if (string.IsNullOrWhiteSpace(request.Password))
        validation.AddFieldError("Password", "Password is required");
    else if (request.Password.Length < 8)
        validation.AddFieldError("Password", "Password must be at least 8 characters");

    if (string.IsNullOrWhiteSpace(request.Name))
        validation.AddFieldError("Name", "Name is required");

    if (validation.FieldErrors.Any())
        throw validation;

    if (users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        throw new ConflictError($"A user with email '{request.Email}' already exists");

    var user = new User
    {
        Id = users.Count + 1,
        Email = request.Email,
        Name = request.Name,
        IsEmailVerified = false,
        CreatedAt = DateTime.UtcNow
    };

    users.Add(user);

    return Results.Created($"/api/users/{user.Id}", user);
})
.WithTags("Users");

app.MapDelete("/api/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null)
        throw new NotFoundError($"User with ID {id} not found");

    users.Remove(user);
    return Results.NoContent();
})
.WithTags("Users");

// ==========================================
// REALISTIC PRODUCT ENDPOINTS
// ==========================================
// These endpoints demonstrate using custom errors (InsufficientStockError)
// alongside built-in errors (NotFoundError, ValidationError, BadRequestError).

app.MapGet("/api/products", () => Results.Ok(products))
    .WithTags("Products");

app.MapGet("/api/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        throw new NotFoundError($"Product with ID {id} not found");

    return Results.Ok(product);
})
.WithTags("Products");

app.MapPost("/api/products", (CreateProductRequest request) =>
{
    var validation = new ValidationError();

    if (string.IsNullOrWhiteSpace(request.Name))
        validation.AddFieldError("Name", "Product name is required");

    if (request.Price <= 0)
        validation.AddFieldError("Price", "Price must be greater than 0");

    if (request.Stock < 0)
        validation.AddFieldError("Stock", "Stock cannot be negative");

    if (validation.FieldErrors.Any())
        throw validation;

    var product = new Product
    {
        Id = products.Max(p => p.Id) + 1,
        Name = request.Name,
        Price = request.Price,
        Stock = request.Stock
    };

    products.Add(product);

    return Results.Created($"/api/products/{product.Id}", product);
})
.WithTags("Products");

app.MapPost("/api/products/{id}/purchase", (int id, int quantity) =>
{
    if (quantity <= 0)
        throw new BadRequestError("Quantity must be greater than 0");

    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        throw new NotFoundError($"Product with ID {id} not found");

    if (product.Stock < quantity)
        throw new InsufficientStockError(product.Name, quantity, product.Stock);

    product.Stock -= quantity;

    return Results.Ok(new
    {
        message = $"Successfully purchased {quantity} units of {product.Name}",
        remainingStock = product.Stock
    });
})
.WithTags("Products");

// ==========================================
// AUTHENTICATION ENDPOINTS
// ==========================================
// This endpoint demonstrates:
// - Using BadRequestError for missing required fields
// - Using custom RateLimitExceededError with retry information
// - Using UnauthorizedError for invalid credentials
// - Using custom EmailNotVerifiedError for unverified accounts

app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        throw new BadRequestError("Email and password are required");

    if (!loginAttempts.ContainsKey(request.Email))
        loginAttempts[request.Email] = new List<DateTime>();

    var attempts = loginAttempts[request.Email];
    var recentAttempts = attempts.Where(a => a > DateTime.UtcNow.AddMinutes(-15)).ToList();
    loginAttempts[request.Email] = recentAttempts;

    if (recentAttempts.Count >= 5)
    {
        var retryAfter = recentAttempts.First().AddMinutes(15);
        throw new RateLimitExceededError(5, TimeSpan.FromMinutes(15), retryAfter);
    }

    loginAttempts[request.Email].Add(DateTime.UtcNow);

    var user = users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
    if (user == null)
        throw new UnauthorizedError("Invalid email or password");

    if (!user.IsEmailVerified)
        throw new EmailNotVerifiedError(user.Email);

    if (request.Password != "Password123")  
        throw new UnauthorizedError("Invalid email or password");

    return Results.Ok(new
    {
        message = "Login successful",
        token = "fake-jwt-token-12345",
        user = new
        {
            user.Id,
            user.Email,
            user.Name
        }
    });
})
.WithTags("Authentication");

// ==========================================
// UNHANDLED EXCEPTION DEMONSTRATION
// ==========================================
// ErrorHound catches ALL exceptions, not just ApiError types.
// Any unhandled exception (like InvalidOperationException, NullReferenceException, etc.)
// will be caught and formatted as an InternalServerError (500) response.
// This ensures your API never returns unformatted error responses to clients.

app.MapGet("/api/demo/unhandled-exception", () =>
{
    // This generic exception will be caught and formatted as InternalServerError
    throw new InvalidOperationException("This is an unhandled exception that ErrorHound will catch");
})
.WithTags("Demo - Built-in Errors");

app.Run();
