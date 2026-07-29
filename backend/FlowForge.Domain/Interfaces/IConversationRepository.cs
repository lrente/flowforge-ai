using FlowForge.Domain.Entities;

namespace FlowForge.Domain.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByAgentAndVisitorAsync(Guid agentId, Guid visitorId, CancellationToken cancellationToken = default);
    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
