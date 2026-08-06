using System.ComponentModel.DataAnnotations;
using FlowForge.Application.Helpers;

namespace FlowForge.Application.DTOs.Agent;

public sealed class CreateAgentRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string BusinessType { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string SystemPrompt { get; set; } = string.Empty;

    [StringLength(100)]
    public string Model { get; set; } = AiModels.GPT41Mini;

    [Range(0, 1)]
    public double Temperature { get; set; } = 0.7;

    public bool IsActive { get; set; } = true;
}
