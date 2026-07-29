using FlowForge.Domain.Entities;

namespace FlowForge.Domain.Interfaces;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
