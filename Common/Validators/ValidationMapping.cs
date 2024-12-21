using FluentValidation.Results;
using SimpleArchitecture.Common.Response;

namespace SimpleArchitecture.Common.Validators;

public static class ValidationMapping
{
    public static FailureResponse ToFailureResponse(this ValidationResult validationResult)
    {
        var errorDict = validationResult.Errors.ToDictionary(error => error.PropertyName, error => error.ErrorMessage);
        var responseData = new ValidationFailureResponse(errorDict);
        return FailureResponse<ValidationFailureResponse>.ValidationFailure(responseData);
    }
}