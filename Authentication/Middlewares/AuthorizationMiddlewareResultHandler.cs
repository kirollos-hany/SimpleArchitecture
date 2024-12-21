using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using SimpleArchitecture.Common.Response;
using SimpleArchitecture.Serialization.Configurations;

namespace SimpleArchitecture.Authentication.Middlewares;

public class AuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly JsonSerializerOptions _serializerOptions;
    
    public AuthorizationMiddlewareResultHandler(JsonSerializerConfigProvider jsonSerializerConfigProvider)
    {
        _serializerOptions = jsonSerializerConfigProvider.SerializerOptions;
    }
    
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }
        
        if (authorizeResult.Forbidden)
        {
            var authorizationResponse = new UnauthorizedResponse(UnauthorizedReason.NotHavePermissions);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            await context.Response.WriteAsJsonAsync(authorizationResponse, _serializerOptions);

            return;
        }
        
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var authResponse = new UnauthorizedResponse(UnauthorizedReason.InvalidAccessToken);

        await context.Response.WriteAsJsonAsync(authResponse, _serializerOptions);
    }
}