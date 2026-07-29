using FlowForge.Application.DTOs.Chat;

namespace FlowForge.Application.Interfaces;

public interface IChatService
{
    Task<ChatResponse> SendMessageAsync(Guid agentId, string message, Guid visitorId, CancellationToken cancellationToken = default);
}
