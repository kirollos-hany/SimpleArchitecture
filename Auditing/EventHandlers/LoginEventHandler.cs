using MediatR;
using SimpleArchitecture.Auditing.Events;
using SimpleArchitecture.Auditing.Types;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Data.Interfaces;
using SimpleArchitecture.Web.Interfaces;

namespace SimpleArchitecture.Auditing.EventHandlers;

public class LoginEventHandler : INotificationHandler<LoginEvent>
{
    private readonly IUserDeviceDetector _userDeviceDetector;

    private readonly IUserIpAddressProvider _userIpAddressProvider;

    private readonly IDbContext _dbContext;

    public LoginEventHandler(IUserDeviceDetector userDeviceDetector, IUserIpAddressProvider userIpAddressProvider,
        IDbContext dbContext)
    {
        _userDeviceDetector = userDeviceDetector;
        _userIpAddressProvider = userIpAddressProvider;
        _dbContext = dbContext;
    }

    public async Task Handle(LoginEvent notification, CancellationToken cancellationToken)
    {
        var user = notification.User;

        var displayName = user.DisplayName;

        var deviceInfo = _userDeviceDetector.DetectDevice();

        var userAgent = _userDeviceDetector.UserAgent();

        var ipAddress = _userIpAddressProvider.GetIpAddress();

        var loginAudit = new LoginAudit(user, displayName, ipAddress, userAgent, notification.Status, deviceInfo);

        await _dbContext.LoginAudits.AddAsync(loginAudit, cancellationToken);
    }
}