using Serilog.Ui.Web.Authorization;
using SimpleArchitecture.Authentication.Enums;

namespace SimpleArchitecture.Logging.Filters;

public class LoggingUiAuthorizationFilter : IUiAuthorizationFilter
{
    public bool Authorize(HttpContext httpContext) => httpContext.User.IsInRole(nameof(Roles.SystemAdministrator));
}