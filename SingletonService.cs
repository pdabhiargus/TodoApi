// Singleton service: Only one instance for the entire application lifetime
public interface ISingletonService
{
    Guid GetOperationId();
}

public class SingletonService : ISingletonService
{
    private Guid _operationId;
    public SingletonService()
    {
        _operationId = Guid.NewGuid();
    }
    public Guid GetOperationId() => _operationId;
}
