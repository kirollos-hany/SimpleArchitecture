using FluentEmail.Core;
using SimpleArchitecture.Common.Response;
using SimpleArchitecture.Emailing.Interfaces;

namespace SimpleArchitecture.Emailing.Services;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task<OutboundMessageSendResponse> SendAsync(string toEmail, string subject, string body, string? fromEmail = null, string? fromName = null)
    {
        var sendEmailBuild = _fluentEmail
            .To(toEmail)
            .Subject(subject)
            .Body(body);

        switch (string.IsNullOrEmpty(fromEmail))
        {
            case false when !string.IsNullOrEmpty(fromName):
                sendEmailBuild = sendEmailBuild
                    .SetFrom(fromEmail, fromName);
                break;
            case false:
                sendEmailBuild = sendEmailBuild
                    .SetFrom(fromEmail);
                break;
        }

        var result = await sendEmailBuild.SendAsync();

        return result.ToResponse();
    }

    public async Task<OutboundMessageSendResponse> SendUsingTemplateAsync<TModel>(string toEmail, string subject, string templatePath, TModel templateModel, string? fromEmail = null, string? fromName = null)
    {
        var sendEmailBuild = _fluentEmail
            .To(toEmail)
            .Subject(subject)
            .UsingTemplateFromFile(templatePath, templateModel);

        switch (string.IsNullOrEmpty(fromEmail))
        {
            case false when !string.IsNullOrEmpty(fromName):
                sendEmailBuild = sendEmailBuild
                    .SetFrom(fromEmail, fromName);
                break;
            case false:
                sendEmailBuild = sendEmailBuild
                    .SetFrom(fromEmail);
                break;
        }

        var result = await sendEmailBuild.SendAsync();

        return result.ToResponse();
    }
}