// Scoped service: New instance per HTTP request
public interface IScopedService
{
    Guid GetOperationId();
}

public class ScopedService : IScopedService
{
    private Guid _operationId;
    public ScopedService()
    {
        _operationId = Guid.NewGuid();
    }
    public Guid GetOperationId() => _operationId;
}
