namespace SimpleArchitecture.Common.Response;

public class ValidationFailureResponse
{
    public ValidationFailureResponse(IReadOnlyDictionary<string, string> validationMap)
    {
        ValidationMap = validationMap;
    }

    public IReadOnlyDictionary<string, string> ValidationMap { get; }
}