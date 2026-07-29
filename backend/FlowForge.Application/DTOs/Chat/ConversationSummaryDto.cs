namespace FlowForge.Application.DTOs.Chat;

public sealed class ConversationSummaryDto
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int MessageCount { get; set; }
}
