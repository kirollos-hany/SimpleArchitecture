using FluentEmail.Core.Models;
using SimpleArchitecture.Common.Response;

namespace SimpleArchitecture.Emailing.Services;

internal static class EmailResponseMapper
{
    public static OutboundMessageSendResponse ToResponse(this SendResponse result)
    {
        var errorMessage = string.Join('\n', result.ErrorMessages);

        return new OutboundMessageSendResponse(errorMessage, result.Successful);
    }
}