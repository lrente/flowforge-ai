using FlowForge.Domain.Entities;

namespace FlowForge.Domain.Interfaces;

public interface IKnowledgeChunkRepository
{
    Task<IReadOnlyList<KnowledgeChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task AddAsync(KnowledgeChunk chunk, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<KnowledgeChunk> chunks, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<KnowledgeChunk> chunks, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
