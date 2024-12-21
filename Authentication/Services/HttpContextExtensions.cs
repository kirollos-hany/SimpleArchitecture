using System.Security.Claims;
using SimpleArchitecture.Authentication.Enums;

namespace SimpleArchitecture.Authentication.Services;

public static class HttpContextExtensions
{
    public static int GetId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == nameof(ClaimType.UserId));

        return userIdClaim is null ? default : int.Parse(userIdClaim.Value);
    }
    
    public static bool IsAuthenticated(this ClaimsPrincipal user) => user.Identity is not null && user.Identity.IsAuthenticated;
}