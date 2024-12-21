using Microsoft.AspNetCore.Diagnostics;
using SimpleArchitecture.Common;
using SimpleArchitecture.Common.Response;

namespace SimpleArchitecture.ExceptionHandling.Middlewares;

public class ExceptionHandlingMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        const string message = Constants.ResponseMessages.InternalServerErrorMessage;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var messageResponse = new MessageResponse(message);

        await httpContext.Response.WriteAsJsonAsync(messageResponse, cancellationToken);
        
        return true;
    }
}