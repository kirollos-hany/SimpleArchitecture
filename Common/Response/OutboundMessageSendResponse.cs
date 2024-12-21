namespace SimpleArchitecture.Common.Response;

public class OutboundMessageSendResponse 
{
    public OutboundMessageSendResponse(string message, bool successful)
    {
        Message = message;
        Successful = successful;
    }
    
    public string Message { get; }
    
    public bool Successful { get; }
}