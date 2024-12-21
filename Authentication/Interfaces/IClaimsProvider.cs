using System.Security.Claims;
using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Authentication.Interfaces;

public interface IClaimsProvider
{
    Task<IEnumerable<Claim>> GetClaimsAsync(User user);
}