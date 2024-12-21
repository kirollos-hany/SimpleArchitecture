using System.Text.Json;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Authentication.Services;
using SimpleArchitecture.Common.Response;
using SimpleArchitecture.Serialization.Configurations;

namespace SimpleArchitecture.Authentication.Middlewares;

public class AccountStatusMiddleware : IMiddleware
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    
    private readonly JsonSerializerOptions _serializerOptions;

    public AccountStatusMiddleware(IAuthenticatedUserService authenticatedUserService, JsonSerializerConfigProvider jsonSerializerConfigProvider)
    {
        _authenticatedUserService = authenticatedUserService;
        _serializerOptions = jsonSerializerConfigProvider.SerializerOptions;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isAttemptToLogin = context.Request.Path.Value?.Contains("/account/login") ?? false;
        
        if (!context.User.IsAuthenticated() || isAttemptToLogin)
        {
            await next(context);
            return;
        }

        var isActive = await _authenticatedUserService.IsActiveAsync();

        if (isActive)
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        var response = new UnauthorizedResponse(UnauthorizedReason.AccountDeactivated);

        await context.Response.WriteAsJsonAsync(response, _serializerOptions);
    }
}