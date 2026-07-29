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

    public async Task<IReadOnlyList<ConversationSummaryDto>> GetConversationsAsync(Guid visitorId, CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationRepository.GetByVisitorIdAsync(visitorId, cancellationToken);
        return conversations.Select(c => new ConversationSummaryDto
        {
            Id = c.Id,
            AgentId = c.AgentId,
            Title = string.IsNullOrWhiteSpace(c.Title) ? "New conversation" : c.Title,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            MessageCount = c.Messages?.Count ?? 0
        }).ToList();
    }

    public async Task<ConversationDetailDto?> GetConversationAsync(Guid conversationId, Guid visitorId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdForVisitorAsync(conversationId, visitorId, cancellationToken);
        if (conversation is null)
        {
            return null;
        }

        return new ConversationDetailDto
        {
            Id = conversation.Id,
            AgentId = conversation.AgentId,
            Title = string.IsNullOrWhiteSpace(conversation.Title) ? "New conversation" : conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            Messages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Role = m.Role,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                }).ToList()
        };
    }

    public async Task<ChatResponse> SendMessageAsync(Guid agentId, string message, Guid visitorId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId, cancellationToken);
        if (agent is null || !agent.IsActive)
        {
            throw new InvalidOperationException("Agent not found or inactive.");
        }

        var conversation = await GetOrCreateConversationAsync(agentId, visitorId, cancellationToken);
        conversation.Title = string.IsNullOrWhiteSpace(conversation.Title) ? Truncate(message, 60) : conversation.Title;
        conversation.UpdatedAt = DateTimeOffset.UtcNow;

        await _messageRepository.AddAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "user",
            Content = message,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        var history = await _messageRepository.GetByConversationIdAsync(conversation.Id, cancellationToken);
        var prompt = BuildPrompt(agent.SystemPrompt, history);
        var responseText = await _openAiService.GenerateResponseAsync(prompt, message, cancellationToken);

        await _messageRepository.AddAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "assistant",
            Content = responseText,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);

        return new ChatResponse { Response = responseText };
    }

    public async Task<ChatResponse> SendMessageToConversationAsync(Guid conversationId, string message, Guid visitorId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdForVisitorAsync(conversationId, visitorId, cancellationToken);
        if (conversation is null)
        {
            throw new InvalidOperationException("Conversation not found.");
        }

        var agent = await _agentRepository.GetByIdAsync(conversation.AgentId, cancellationToken);
        if (agent is null || !agent.IsActive)
        {
            throw new InvalidOperationException("Agent not found or inactive.");
        }

        conversation.Title = string.IsNullOrWhiteSpace(conversation.Title) ? Truncate(message, 60) : conversation.Title;
        conversation.UpdatedAt = DateTimeOffset.UtcNow;

        await _messageRepository.AddAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "user",
            Content = message,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        var history = await _messageRepository.GetByConversationIdAsync(conversation.Id, cancellationToken);
        var prompt = BuildPrompt(agent.SystemPrompt, history);
        var responseText = await _openAiService.GenerateResponseAsync(prompt, message, cancellationToken);

        await _messageRepository.AddAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "assistant",
            Content = responseText,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);

        return new ChatResponse { Response = responseText };
    }

    public async Task DeleteConversationAsync(Guid conversationId, Guid visitorId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdForVisitorAsync(conversationId, visitorId, cancellationToken);
        if (conversation is null)
        {
            return;
        }

        await _conversationRepository.DeleteAsync(conversation, cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);
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
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    private static string BuildPrompt(string systemPrompt, IReadOnlyList<Message> history)
    {
        var messages = history.Where(m => !string.IsNullOrWhiteSpace(m.Content)).Select(m => $"{m.Role}: {m.Content}").ToList();
        var prompt = $"System instructions:\n{systemPrompt}\n\nConversation history:\n{string.Join("\n", messages)}";
        return prompt;
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
