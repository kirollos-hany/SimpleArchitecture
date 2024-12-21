using Microsoft.EntityFrameworkCore;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Data.Interfaces;
using SimpleArchitecture.Data.Queries.Users;

namespace SimpleArchitecture.Authentication.Services;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly HttpContext? _httpContext;

    private readonly IDbContext _dbContext;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor, IDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public int GetId()
    {
        if (_httpContext is null)
            return default;

        var isAuthenticated = _httpContext.User.Identity is not null && _httpContext.User.Identity.IsAuthenticated;

        return isAuthenticated ? _httpContext.User.GetId() : default;
    }

    public Task<bool> IsActiveAsync()
    {
        var id = GetId();

        return id == default ? Task.FromResult(false) : _dbContext.Users.IsUserActive().AnyAsync();
    }
}