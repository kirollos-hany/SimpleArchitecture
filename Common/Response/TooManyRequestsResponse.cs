namespace SimpleArchitecture.Common.Response;

public class TooManyRequestsResponse
{
    public TooManyRequestsResponse(DateTime tryAgainAt)
    {
        TryAgainAt = tryAgainAt;
    }
    
    public DateTime TryAgainAt { get; }
}