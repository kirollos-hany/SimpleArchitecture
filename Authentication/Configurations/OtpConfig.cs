namespace SimpleArchitecture.Authentication.Configurations;

public class OtpConfig
{
    public int Length { get; set; }
    
    public int ExpirationInMinutes { get; set; }
    
    public int StorageLifeSpanInMinutes { get; set; }
}