using System.Net;
using ErrorHound.Core;

namespace ErrorHound.Sample.Errors;

/// <summary>
/// Custom error for email verification scenarios
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
/// Custom error for subscription-related issues
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
/// Custom error for rate limiting with retry information
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
/// Custom error for insufficient stock
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
