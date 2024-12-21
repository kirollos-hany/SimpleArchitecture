using SimpleArchitecture.Common.ValueObjects;

namespace SimpleArchitecture.Authentication.Types;

public class UserDevice
{
    public int Id { get; private set; }
    
    public int UserId { get; private set; }
    
    public User? User { get; private set; }
    
    public required string IpAddress { get; init; }
    
    public string? DeviceToken { get; set; }
    
    public required string UserAgent { get; init; }
    
    public required DeviceInfo? Device { get; init; }

    private RefreshToken? _refreshToken;
    
    public RefreshToken? RefreshToken
    {
        get => _refreshToken;
        internal set
        {
            _refreshToken = value;
            LastActiveAt = DateTime.UtcNow;
        }
    }

    public DateTime LastActiveAt { get; private set; }
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    internal void Revoke(string refreshToken)
    {
        LastActiveAt = DateTime.UtcNow;
        
        _refreshToken?.Revoke(refreshToken);
    }
}