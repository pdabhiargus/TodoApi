// Transient service: New instance every time it is requested
public interface ITransientService
{
    Guid GetOperationId();
}

public class TransientService : ITransientService
{
    private Guid _operationId;
    public TransientService()
    {
        _operationId = Guid.NewGuid();
    }
    public Guid GetOperationId() => _operationId;
}
