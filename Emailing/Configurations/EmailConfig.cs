namespace SimpleArchitecture.Emailing.Configurations;

public class EmailConfig
{
    public string DefaultFromEmail { get; set; } = string.Empty;

    public string DefaultFromName { get; set; } = string.Empty;

    public SmtpConfig SmtpConfig { get; set; } = new();

    public SendGridConfig SendGridConfig { get; set; } = new();

    public MailGunConfig MailGunConfig { get; set; } = new();

    public MailTrapConfig MailTrapConfig { get; set; } = new();

    public MailKitConfig MailKitConfig { get; set; } = new();
}

public class SmtpConfig
{
    public int Port { get; set; }

    public string Host { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public class SendGridConfig
{
    public string ApiKey { get; set; } = string.Empty;

    public bool SandBoxMode { get; set; } 
}
    
public class MailGunConfig
{
    public string ApiKey { get; set; } = string.Empty;

    public string DomainName { get; set; } = string.Empty;
}

public class MailTrapConfig
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;
    
    public int Port { get; set; }
}

public class MailKitConfig
{
    public int Port { get; set; }

    public string Host { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    
    public bool UseSsl { get; set; }
    
    public bool RequiresAuthentication { get; set; }
}