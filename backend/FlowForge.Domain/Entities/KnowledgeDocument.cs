using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class KnowledgeDocument : BaseEntity
{
    public Guid AgentId { get; set; }
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Status { get; set; } = "Uploaded";

    public Agent? Agent { get; set; }
    public ICollection<KnowledgeChunk> Chunks { get; set; } = new List<KnowledgeChunk>();
}
