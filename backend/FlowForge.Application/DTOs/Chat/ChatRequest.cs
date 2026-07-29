using System.ComponentModel.DataAnnotations;

namespace FlowForge.Application.DTOs.Chat;

public sealed class ChatRequest
{
    [Required]
    public Guid AgentId { get; set; }

    [Required]
    [StringLength(4000)]
    public string Message { get; set; } = string.Empty;
}
