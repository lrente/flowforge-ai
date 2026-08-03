using FlowForge.Domain.Common;

namespace FlowForge.Domain.Entities;

public sealed class KnowledgeChunk : BaseEntity
{
    public Guid DocumentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public string? Embedding { get; set; }
    public Guid? EmbeddingId { get; set; }

    public KnowledgeDocument? Document { get; set; }
}
