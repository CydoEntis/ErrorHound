using System.Net;
using ErrorHound.Core;

namespace ErrorHound.Sample.Errors;

/// <summary>
/// Custom error for email verification scenarios.
/// Demonstrates creating a custom error with structured details.
/// </summary>
public sealed class EmailNotVerifiedError : ApiError
{
    public EmailNotVerifiedError(string email)
        : base(
            "EMAIL_NOT_VERIFIED",
            "Email address has not been verified",
            (int)HttpStatusCode.Forbidden,
            new
            {
                email,
                action = "Please check your inbox for the verification link"
            })
    {
    }
}

/// <summary>
/// Custom error for subscription-related issues.
/// Demonstrates using a custom HTTP status code (402 Payment Required).
/// </summary>
public sealed class SubscriptionExpiredError : ApiError
{
    public SubscriptionExpiredError(DateTime expiryDate)
        : base(
            "SUBSCRIPTION_EXPIRED",
            "Your subscription has expired",
            (int)HttpStatusCode.PaymentRequired,
            new
            {
                expiredOn = expiryDate.ToString("yyyy-MM-dd"),
                message = "Please renew your subscription to continue"
            })
    {
    }
}

/// <summary>
/// Custom error for rate limiting with retry information.
/// Demonstrates including structured retry metadata in error details.
/// </summary>
public sealed class RateLimitExceededError : ApiError
{
    public RateLimitExceededError(int maxRequests, TimeSpan window, DateTime retryAfter)
        : base(
            "RATE_LIMIT_EXCEEDED",
            $"Rate limit exceeded. Maximum {maxRequests} requests per {window.TotalMinutes} minutes.",
            (int)HttpStatusCode.TooManyRequests,
            new
            {
                maxRequests,
                windowMinutes = window.TotalMinutes,
                retryAfter = retryAfter.ToString("o")
            })
    {
    }
}

/// <summary>
/// Custom error for insufficient stock scenarios.
/// Demonstrates creating domain-specific errors with contextual information.
/// </summary>
public sealed class InsufficientStockError : ApiError
{
    public InsufficientStockError(string productName, int requested, int available)
        : base(
            "INSUFFICIENT_STOCK",
            "Insufficient stock available",
            (int)HttpStatusCode.Conflict,
            new
            {
                productName,
                requested,
                available,
                message = $"Only {available} units available, but {requested} requested"
            })
    {
    }
}
