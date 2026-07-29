using FlowForge.Domain.Entities;

namespace FlowForge.Domain.Interfaces;

public interface IConversationRepository
{
    Task<IReadOnlyList<Conversation>> GetByVisitorIdAsync(Guid visitorId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByIdForVisitorAsync(Guid id, Guid visitorId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByAgentAndVisitorAsync(Guid agentId, Guid visitorId, CancellationToken cancellationToken = default);
    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task DeleteAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
