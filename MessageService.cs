// IMessageService defines a contract for a message service that can be injected via dependency injection.
public interface IMessageService
{
    string GetMessage();
}

// MessageService is a concrete implementation of IMessageService.
// It will be provided to controllers that request IMessageService via dependency injection.
public class MessageService : IMessageService
{
    public string GetMessage()
    {
        return "Hello from MessageService!";
    }
}
