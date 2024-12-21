using FluentValidation;
using SimpleArchitecture.Common.Validators;
using SimpleArchitecture.Mappers;

namespace SimpleArchitecture.RequestValidation.Filters;

public class ValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().First();

        await using var scope = context.HttpContext.RequestServices.CreateAsyncScope();

        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TRequest>>();

        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        return validationResult.ToFailureResponse().ToResponse();
    }
}