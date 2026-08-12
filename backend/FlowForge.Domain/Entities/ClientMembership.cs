using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class ClientMembership : BaseEntity
{
    public Guid ClientId { get; set; }
    public Guid UserId { get; set; }
    public ClientRole Role { get; set; }
    public Client? Client { get; set; }
    public User? User { get; set; }
}
