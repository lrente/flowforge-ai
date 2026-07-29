using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IKnowledgeRepository
{
    Task<IReadOnlyList<KnowledgeDocument>> GetByAgentIdAsync(Guid agentId, CancellationToken cancellationToken = default);
    Task<KnowledgeDocument?> GetByIdForAgentAsync(Guid id, Guid agentId, CancellationToken cancellationToken = default);
    Task AddAsync(KnowledgeDocument document, CancellationToken cancellationToken = default);
    Task DeleteAsync(KnowledgeDocument document, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
