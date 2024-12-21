using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Authentication.Services;

public class ClaimsProvider : IClaimsProvider
{
    private readonly UserManager<User> _userManager;

    public ClaimsProvider(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        
        var roles = await _userManager.GetRolesAsync(user);
        
        var userIdClaim = new Claim(nameof(ClaimType.UserId), Convert.ToString(user.Id));
        
        var jtiClaim = new Claim(nameof(JwtRegisteredClaimNames.Jti),Guid.NewGuid().ToString());
        
        var emailClaim = new Claim(nameof(JwtRegisteredClaimNames.Email), user.Email ?? string.Empty);

        var statusClaim = new Claim(nameof(ClaimType.AccountStatus), user.IsActive.ToString());
        
        var tokenClaims = claims.Concat(roles.Select(r => new Claim(nameof(ClaimType.Roles), r))).ToList();

        tokenClaims.AddRange(new [] { userIdClaim, emailClaim, jtiClaim, statusClaim });

        return tokenClaims;
    }
}