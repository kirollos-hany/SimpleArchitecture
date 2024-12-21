using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SimpleArchitecture.Authentication.Configurations;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Common.Utilities;

namespace SimpleArchitecture.Authentication.Services;

public class JwtProvider : IJwtProvider
{
    private readonly JwtConfig _config;

    public JwtProvider(JwtConfig config)
    {
        _config = config;
    }

    public (string token, DateTime expiration) GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secrets));
        var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(_config.AccessTokenExpirationInMinutes);
        
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return (token, expiration);
    }

    public string GenerateRefreshToken()
    {
        var token = StringUtilities.GenerateRandomStringBase64(32);

        return token;
    }
}