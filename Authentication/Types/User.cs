using MediatR;
using Microsoft.AspNetCore.Identity;
using SimpleArchitecture.Auditing.Events;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Common.Interfaces;
using SimpleArchitecture.Common.ValueObjects;

namespace SimpleArchitecture.Authentication.Types;

public class User : IdentityUser<int>, IEntityWithNotifications
{
    public bool IsActive { get; private set; } = true;
    
    public string DisplayName { get; set; } = string.Empty;
    
    public string ProfilePicture { get; set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<INotification> _events = new ();
    
    public IReadOnlyList<INotification> Notifications => _events;

    public void PhoneVerified()
    {
        PhoneNumberConfirmed = true;
    }
    
    public void EmailVerified()
    {
        EmailConfirmed = true;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public bool IsVerified() => EmailConfirmed || PhoneNumberConfirmed;

    private List<UserRole> _userRoles = new();

    public IReadOnlyList<UserRole> UserRoles { get => _userRoles; private set => _userRoles = value.ToList(); }

    private List<UserDevice> _devices = new ();
    
    public IReadOnlyList<UserDevice> Devices
    {
        get => _devices;
        private set => _devices = value.ToList();
    }

    public UserDevice? Login(string? refreshToken, string ipAddress, string userAgent, DeviceInfo? deviceInfo, string? deviceToken)
    {
        if (refreshToken is null)
        {
            _events.Add(new LoginEvent(this, LoginStatus.Failure));
            
            return null;
        }
        
        _events.Add(new LoginEvent(this, LoginStatus.Success));
    
        var device = _devices.FirstOrDefault(device => device.IpAddress == ipAddress);
    
        if (device is null)
        {
            device = new UserDevice
            {
                Device = deviceInfo,
                IpAddress = ipAddress,
                RefreshToken = new RefreshToken(refreshToken),
                UserAgent = userAgent,
                DeviceToken = deviceToken
            };
            
            _devices.Add(device);
            
            return device;
        }
    
        device.Revoke(refreshToken);
    
        if (!string.IsNullOrEmpty(deviceToken))
        {
            device.DeviceToken = deviceToken;
        }
    
        return device;
    }

    public UserDevice? Revoke(string ipAddress, string refreshToken, string newRefreshToken)
    {
        var device = _devices.FirstOrDefault(device => device.IpAddress == ipAddress && device.RefreshToken?.Token == refreshToken);

        device?.Revoke(newRefreshToken);

        return device;
    }
}
