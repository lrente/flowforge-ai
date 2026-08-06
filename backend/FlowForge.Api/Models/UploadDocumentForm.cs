using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FlowForge.Api.Models;

public sealed class UploadDocumentForm
{
    [Required]
    public Guid AgentId { get; set; }

    [Required]
    public IFormFile File { get; set; } = default!;
}