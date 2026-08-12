using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface ITenantContext
{
    Task<TenantAccess?> GetAccessAsync(CancellationToken cancellationToken = default);
    Task<TenantAccess?> GetAccessAsync(Guid clientId, CancellationToken cancellationToken = default);
}

public sealed record TenantAccess(Guid UserId, Guid ClientId, ClientRole Role, bool IsSystemAdministrator);
