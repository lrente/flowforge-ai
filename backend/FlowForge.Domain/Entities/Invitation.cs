using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class Invitation : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public ClientRole Role { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string Status { get; set; } = "Pending";
}
