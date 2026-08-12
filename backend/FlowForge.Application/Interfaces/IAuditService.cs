namespace FlowForge.Application.Interfaces;

public interface IAuditService
{
    Task WriteAsync(string action, string entityType, Guid entityId, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default);
}
