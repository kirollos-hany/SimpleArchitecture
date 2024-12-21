namespace SimpleArchitecture.Common.Response;

public record OrderCreatedResponse
{
    public required string OrderReferenceId { get; init; }
    
    public required int OrderNumber { get; init; }
    
    public required string Message { get; init; }
    
    public required DateTime CreatedAt { get; init; }
}