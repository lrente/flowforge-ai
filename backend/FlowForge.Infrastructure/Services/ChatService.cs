using FlowForge.Application.DTOs.Chat;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;

namespace FlowForge.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IOpenAiService _openAiService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public ChatService(
        IAgentRepository agentRepository,
        IOpenAiService openAiService,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _agentRepository = agentRepository;
        _openAiService = openAiService;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<ChatResponse> SendMessageAsync(Guid agentId, string message, Guid visitorId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null || !agent.IsActive)
        {
            throw new InvalidOperationException("Agent not found or inactive.");
        }

        var conversation = await GetOrCreateConversationAsync(agentId, visitorId, cancellationToken);

        await _messageRepository.AddAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "user",
            Content = message,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        var responseText = await _openAiService.GenerateResponseAsync(agent.SystemPrompt, message, cancellationToken);

        await _messageRepository.AddAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "assistant",
            Content = responseText,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _messageRepository.SaveChangesAsync(cancellationToken);

        return new ChatResponse { Response = responseText };
    }

    private async Task<Conversation> GetOrCreateConversationAsync(Guid agentId, Guid visitorId, CancellationToken cancellationToken)
    {
        var existing = await _conversationRepository.GetByAgentAndVisitorAsync(agentId, visitorId, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            VisitorId = visitorId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);
        return conversation;
    }
}
