using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Data.Queries.Users;

public static class UsersQueries
{
    public static IQueryable<User> IsUserActive(this IQueryable<User> usersQueryable) =>
        usersQueryable.Where(user => user.IsActive);
}