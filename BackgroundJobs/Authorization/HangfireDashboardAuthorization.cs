using Hangfire.Dashboard;
using SimpleArchitecture.Authentication.Enums;

namespace SimpleArchitecture.BackgroundJobs.Authorization;

public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        var isAllowed = httpContext.User.Identity is not null &&
                        httpContext.User.IsInRole(nameof(Roles.SystemAdministrator));

        return isAllowed;
    }
}