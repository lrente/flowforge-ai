using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
