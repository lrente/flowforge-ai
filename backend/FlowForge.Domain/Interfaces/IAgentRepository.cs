using FlowForge.Domain.Entities;

namespace FlowForge.Domain.Interfaces;

public interface IAgentRepository
{
    Task<IReadOnlyList<Agent>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agent>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Agent?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<Agent?> GetByIdForClientAsync(Guid id, Guid clientId, CancellationToken cancellationToken = default);
    Task AddAsync(Agent agent, CancellationToken cancellationToken = default);
    Task UpdateAsync(Agent agent, CancellationToken cancellationToken = default);
    Task DeleteAsync(Agent agent, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
