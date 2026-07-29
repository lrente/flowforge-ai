namespace FlowForge.Application.DTOs.Chat;

public sealed class ConversationDetailDto
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public IReadOnlyList<MessageDto> Messages { get; set; } = Array.Empty<MessageDto>();
}

public sealed class MessageDto
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
