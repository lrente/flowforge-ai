using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class Conversation : BaseEntity
{
    public Guid AgentId { get; set; }
    public Guid VisitorId { get; set; }
}
