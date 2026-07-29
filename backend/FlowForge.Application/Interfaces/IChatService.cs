using FlowForge.Application.DTOs.Chat;

namespace FlowForge.Application.Interfaces;

public interface IChatService
{
    Task<IReadOnlyList<ConversationSummaryDto>> GetConversationsAsync(Guid visitorId, CancellationToken cancellationToken = default);
    Task<ConversationDetailDto?> GetConversationAsync(Guid conversationId, Guid visitorId, CancellationToken cancellationToken = default);
    Task<ChatResponse> SendMessageAsync(Guid agentId, string message, Guid visitorId, CancellationToken cancellationToken = default);
    Task<ChatResponse> SendMessageToConversationAsync(Guid conversationId, string message, Guid visitorId, CancellationToken cancellationToken = default);
    Task DeleteConversationAsync(Guid conversationId, Guid visitorId, CancellationToken cancellationToken = default);
}
