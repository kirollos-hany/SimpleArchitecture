using SimpleArchitecture.Common.Response;

namespace SimpleArchitecture.Emailing.Interfaces;

public interface IEmailService
{
    Task<OutboundMessageSendResponse> SendAsync(string toEmail, string subject, string body, string? fromEmail = null, string? fromName = null);

    Task<OutboundMessageSendResponse> SendUsingTemplateAsync<TModel>(string toEmail, string subject, string templatePath, TModel templateModel, string? fromEmail = null, string? fromName = null);
}