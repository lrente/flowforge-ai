using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class Agent : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public bool IsActive { get; set; }
}
