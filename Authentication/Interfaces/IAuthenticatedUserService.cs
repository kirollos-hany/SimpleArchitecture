namespace SimpleArchitecture.Authentication.Interfaces;

public interface IAuthenticatedUserService
{
    public int GetId();

    public Task<bool> IsActiveAsync();
}