using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class Conversation : BaseEntity
{
    public Guid AgentId { get; set; }
    public Guid VisitorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
