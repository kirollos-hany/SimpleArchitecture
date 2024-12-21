using System.Security.Claims;

namespace SimpleArchitecture.Authentication.Interfaces;

public interface IJwtProvider
{
    (string token, DateTime expiration) GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
}