# ErrorHound Sample Application

This is a complete, working demonstration of **ErrorHound** - an error-handling middleware for ASP.NET Core. This sample showcases all features, including built-in errors, custom errors, validation, and custom formatters.

## What This Sample Demonstrates

### ✅ ErrorHound Setup
- Service registration with `AddErrorHound()`
- Middleware configuration with `UseErrorHound()`
- Two formatter options: `DefaultErrorFormatter` and `CustomApiFormatter`

### ✅ All 11 Built-in Error Types
- `BadRequestError` (400)
- `UnauthorizedError` (401)
- `ForbiddenError` (403)
- `NotFoundError` (404)
- `ConflictError` (409)
- `TooManyRequestsError` (429)
- `InternalServerError` (500)
- `DatabaseError` (500)
- `ServiceUnavailableError` (503)
- `TimeoutError` (504)
- `ValidationError` (400 with field-level errors)

### ✅ Custom Error Types
- `EmailNotVerifiedError` - Email verification scenarios
- `SubscriptionExpiredError` - Subscription management
- `RateLimitExceededError` - Advanced rate limiting with retry info
- `InsufficientStockError` - E-commerce inventory management

### ✅ Custom Formatter
- `CustomApiFormatter` - Demonstrates dependency injection in formatters
- Shows how to wrap errors in an envelope with metadata
- Uses `ILogger` and `IHttpContextAccessor` services

### ✅ Realistic API Scenarios
- User management (CRUD operations)
- Product catalog (with stock management)
- Authentication (with rate limiting)
- Validation errors (multiple errors per field)

---

## Running the Sample

### 1. Build and Run

```bash
cd ErrorHound.Sample
dotnet run
```

The API will start at `http://localhost:5000` (or the URL shown in the console).

### 2. Switch Between Formatters

Edit `Program.cs` lines 14-24 to switch formatters:

**Default Formatter:**
```csharp
builder.Services.AddErrorHound(options =>
{
    options.UseFormatter<DefaultErrorFormatter>();
});
```

**Custom Formatter (with envelope):**
```csharp
builder.Services.AddErrorHound(options =>
{
    options.UseFormatter<CustomApiFormatter>();
});
```

---

## Testing the Endpoints

### Welcome Endpoint
```bash
curl http://localhost:5000/
```

Response shows all available endpoint categories.

---

### Built-in Error Demonstrations

#### 1. Bad Request (400)
```bash
curl http://localhost:5000/api/demo/bad-request
```

#### 2. Unauthorized (401)
```bash
curl http://localhost:5000/api/demo/unauthorized
```

#### 3. Forbidden (403)
```bash
curl http://localhost:5000/api/demo/forbidden
```

#### 4. Not Found (404)
```bash
curl http://localhost:5000/api/demo/not-found
```

#### 5. Conflict (409)
```bash
curl http://localhost:5000/api/demo/conflict
```

#### 6. Rate Limit (429)
```bash
curl http://localhost:5000/api/demo/rate-limit
```

#### 7. Internal Server Error (500)
```bash
curl http://localhost:5000/api/demo/internal-error
```

#### 8. Database Error (500)
```bash
curl http://localhost:5000/api/demo/database-error
```

#### 9. Service Unavailable (503)
```bash
curl http://localhost:5000/api/demo/service-unavailable
```

#### 10. Timeout (504)
```bash
curl http://localhost:5000/api/demo/timeout
```

#### 11. Validation Error (400 with field-level errors)
```bash
curl -X POST http://localhost:5000/api/demo/validation \
  -H "Content-Type: application/json" \
  -d "{}"
```

**Response with CustomApiFormatter:**
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION",
    "message": "Validation failed",
    "validationErrors": {
      "Email": ["Email is required"],
      "Password": ["Password is required"],
      "Name": ["Name is required"]
    }
  },
  "meta": {
    "timestamp": "2025-12-21T10:30:00Z",
    "traceId": "0HMV9C6O9N4AR:00000001",
    "version": "v1.0"
  }
}
```

---

### Custom Error Demonstrations

#### Email Not Verified
```bash
curl http://localhost:5000/api/custom/email-not-verified
```

**Response:**
```json
{
  "success": false,
  "error": {
    "code": "EMAIL_NOT_VERIFIED",
    "message": "Email address has not been verified",
    "details": {
      "email": "user@example.com",
      "action": "Please check your inbox for the verification link"
    }
  },
  "meta": {
    "timestamp": "2025-12-21T10:30:00Z",
    "traceId": "...",
    "version": "v1.0"
  }
}
```

#### Subscription Expired
```bash
curl http://localhost:5000/api/custom/subscription-expired
```

#### Rate Limit Exceeded (Custom)
```bash
curl http://localhost:5000/api/custom/rate-limit
```

#### Insufficient Stock
```bash
curl http://localhost:5000/api/custom/insufficient-stock
```

---

### Realistic User Management

#### Create User
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "name": "John Doe",
    "password": "SecurePass123"
  }'
```

**Success Response (201 Created):**
```json
{
  "id": 1,
  "email": "john@example.com",
  "name": "John Doe",
  "isEmailVerified": false,
  "createdAt": "2025-12-21T10:30:00Z"
}
```

#### Create Duplicate User (Conflict Error)
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "name": "Jane Doe",
    "password": "AnotherPass456"
  }'
```

**Error Response (409 Conflict):**
```json
{
  "success": false,
  "error": {
    "code": "CONFLICT",
    "message": "The request could not be completed due to a conflict.",
    "details": "A user with email 'john@example.com' already exists"
  },
  "meta": { ... }
}
```

#### Get User by ID
```bash
curl http://localhost:5000/api/users/1
```

#### Get Non-Existent User (Not Found Error)
```bash
curl http://localhost:5000/api/users/999
```

**Error Response (404 Not Found):**
```json
{
  "success": false,
  "error": {
    "code": "NOT_FOUND",
    "message": "The requested resource could not be found.",
    "details": "User with ID 999 not found"
  },
  "meta": { ... }
}
```

#### List All Users
```bash
curl http://localhost:5000/api/users
```

#### Delete User
```bash
curl -X DELETE http://localhost:5000/api/users/1
```

---

### Product Management

#### List Products
```bash
curl http://localhost:5000/api/products
```

**Response:**
```json
[
  { "id": 1, "name": "Laptop", "price": 999.99, "stock": 5 },
  { "id": 2, "name": "Mouse", "price": 29.99, "stock": 50 },
  { "id": 3, "name": "Keyboard", "price": 79.99, "stock": 0 }
]
```

#### Purchase Product (Success)
```bash
curl -X POST "http://localhost:5000/api/products/1/purchase?quantity=2"
```

**Success Response:**
```json
{
  "message": "Successfully purchased 2 units of Laptop",
  "remainingStock": 3
}
```

#### Purchase Out-of-Stock Product (Custom Error)
```bash
curl -X POST "http://localhost:5000/api/products/3/purchase?quantity=1"
```

**Error Response (409 Conflict):**
```json
{
  "success": false,
  "error": {
    "code": "INSUFFICIENT_STOCK",
    "message": "Insufficient stock available",
    "details": {
      "productName": "Keyboard",
      "requested": 1,
      "available": 0,
      "message": "Only 0 units available, but 1 requested"
    }
  },
  "meta": { ... }
}
```

#### Purchase Too Many Units
```bash
curl -X POST "http://localhost:5000/api/products/1/purchase?quantity=100"
```

---

### Authentication

#### Login (User Not Found)
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "nonexistent@example.com",
    "password": "Password123"
  }'
```

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "error": {
    "code": "UNAUTHORIZED",
    "message": "Authentication is required to access this resource.",
    "details": "Invalid email or password"
  },
  "meta": { ... }
}
```

---

## Understanding the Code

### Setup in Program.cs

```csharp
// 1. Register ErrorHound services
builder.Services.AddErrorHound(options =>
{
    options.UseFormatter<CustomApiFormatter>();
});

// 2. Add middleware early in pipeline
app.UseErrorHound();
```

### Using Built-in Errors

```csharp
app.MapGet("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null)
        throw new NotFoundError($"User with ID {id} not found");

    return Results.Ok(user);
});
```

### Using Validation Errors

```csharp
var validation = new ValidationError();

if (string.IsNullOrWhiteSpace(request.Email))
    validation.AddFieldError("Email", "Email is required");

if (request.Password.Length < 8)
    validation.AddFieldError("Password", "Password must be at least 8 characters");

if (validation.FieldErrors.Any())
    throw validation;
```

### Creating Custom Errors

```csharp
public sealed class EmailNotVerifiedError : ApiError
{
    public EmailNotVerifiedError(string email)
        : base(
            "EMAIL_NOT_VERIFIED",
            "Email address has not been verified",
            (int)HttpStatusCode.Forbidden,
            new { email, action = "Please verify your email" })
    {
    }
}
```

### Creating Custom Formatters

```csharp
public sealed class CustomApiFormatter : IErrorResponseFormatter
{
    private readonly ILogger<CustomApiFormatter> _logger;

    public CustomApiFormatter(ILogger<CustomApiFormatter> logger)
    {
        _logger = logger;
    }

    public object Format(ApiError error)
    {
        return new
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
                version = "v1.0"
            }
        };
    }
}
```

---

## Key Takeaways

1. **ErrorHound Setup is Simple**
   - Two lines: `AddErrorHound()` and `UseErrorHound()`

2. **Consistent Error Responses**
   - All errors follow the same format
   - HTTP status codes are preserved

3. **Validation is Easy**
   - Multiple errors per field
   - Clear field-level error messages

4. **Custom Errors are Powerful**
   - Extend `ApiError` for domain-specific errors
   - Include rich details and metadata

5. **Custom Formatters Support DI**
   - Inject any service into formatters
   - Full control over response structure

---

## Next Steps

1. Experiment with different endpoints
2. Try creating your own custom errors
3. Switch between `DefaultErrorFormatter` and `CustomApiFormatter`
4. Create your own formatter with different response structures
5. Add more endpoints with different error scenarios

---

## Questions?

See the main ErrorHound documentation:
- [README.md](../README.md)
- [REFACTORING_GUIDE.md](../REFACTORING_GUIDE.md)

Or explore the code in this sample:
- `Program.cs` - All endpoints and setup
- `Errors/CustomErrors.cs` - Custom error implementations
- `Formatters/CustomApiFormatter.cs` - Custom formatter with DI
- `Models/User.cs` - Request/response models
