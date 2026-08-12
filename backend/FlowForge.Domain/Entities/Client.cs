using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<ClientMembership> Memberships { get; set; } = new List<ClientMembership>();
}
