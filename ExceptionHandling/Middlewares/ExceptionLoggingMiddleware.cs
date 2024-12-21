using Microsoft.AspNetCore.Diagnostics;
using SimpleArchitecture.Authentication.Services;

namespace SimpleArchitecture.ExceptionHandling.Middlewares;

public class ExceptionLoggingMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionLoggingMiddleware> _logger;

    public ExceptionLoggingMiddleware(ILogger<ExceptionLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var isAuthenticated = httpContext.User.IsAuthenticated();

        var userId = isAuthenticated ? httpContext.User.GetId().ToString() : "Anonymous";

        var method = httpContext.Request.Method;

        var endpoint = httpContext.Request.Path.ToString();

        _logger.LogError(exception, "User: {userId} invoked a {method} request to {endpoint} with a result of an exception", userId, method, endpoint);
        
        return ValueTask.FromResult(false);
    }
}