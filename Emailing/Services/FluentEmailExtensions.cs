using FluentEmail.MailKitSmtp;
using SimpleArchitecture.Emailing.Configurations;
using SimpleArchitecture.Emailing.Enums;

namespace SimpleArchitecture.Emailing.Services;

public static class FluentEmailExtensions
{
        public static FluentEmailServicesBuilder AddSender(this FluentEmailServicesBuilder builder,
        EmailSenderService senderService, EmailConfig configuration)
    {
        return senderService switch
        {
            EmailSenderService.Smtp when string.IsNullOrEmpty(configuration.SmtpConfig.Username) ||
                                         string.IsNullOrEmpty(configuration.SmtpConfig.Password) =>
                builder.AddSmtpSender(configuration.SmtpConfig.Host, configuration.SmtpConfig.Port),
            EmailSenderService.Smtp => builder.AddSmtpSender(configuration.SmtpConfig.Host,
                configuration.SmtpConfig.Port, configuration.SmtpConfig.Username, configuration.SmtpConfig.Password),
            EmailSenderService.SendGrid => builder.AddSendGridSender(configuration.SendGridConfig.ApiKey,
                configuration.SendGridConfig.SandBoxMode),
            EmailSenderService.MailGun => builder.AddMailGunSender(configuration.MailGunConfig.DomainName, configuration.MailGunConfig.ApiKey),
            EmailSenderService.MailTrap when string.IsNullOrEmpty(configuration.MailTrapConfig.Host) || configuration.MailTrapConfig.Port == default => builder.AddMailtrapSender(configuration.MailTrapConfig.Username, configuration.MailTrapConfig.Password),
            EmailSenderService.MailTrap => builder.AddMailtrapSender(configuration.MailTrapConfig.Username, configuration.MailTrapConfig.Password, configuration.MailTrapConfig.Host, configuration.MailTrapConfig.Port),
            EmailSenderService.MailKit => builder.AddMailKitSender(new SmtpClientOptions
            {
                Password = string.IsNullOrEmpty(configuration.MailKitConfig.Password) ? null : configuration.MailKitConfig.Password,
                Port = configuration.MailKitConfig.Port,
                Server = configuration.MailKitConfig.Host,
                User = string.IsNullOrEmpty(configuration.MailKitConfig.Username) ? null : configuration.MailKitConfig.Username,
                UseSsl = configuration.MailKitConfig.UseSsl,
                RequiresAuthentication = configuration.MailKitConfig.RequiresAuthentication
            }),
            _ => throw new ArgumentException("Email sender service not supported.")
        };
    }
}