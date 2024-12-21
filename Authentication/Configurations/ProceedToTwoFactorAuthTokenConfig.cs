namespace SimpleArchitecture.Authentication.Configurations;

public class ProceedToTwoFactorAuthTokenConfig
{
    public int ExpirationInMinutes { get; set; }
    
    public int StorageLifeSpanInMinutes { get; set; }
}