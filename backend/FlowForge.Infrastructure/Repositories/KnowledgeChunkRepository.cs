using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Repositories;

public sealed class KnowledgeChunkRepository : IKnowledgeChunkRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeChunkRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<KnowledgeChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeChunks
            .Where(chunk => chunk.DocumentId == documentId)
            .OrderBy(chunk => chunk.ChunkIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(KnowledgeChunk chunk, CancellationToken cancellationToken = default)
    {
        await _context.KnowledgeChunks.AddAsync(chunk, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<KnowledgeChunk> chunks, CancellationToken cancellationToken = default)
    {
        await _context.KnowledgeChunks.AddRangeAsync(chunks, cancellationToken);
    }

    public Task DeleteRangeAsync(IEnumerable<KnowledgeChunk> chunks, CancellationToken cancellationToken = default)
    {
        _context.KnowledgeChunks.RemoveRange(chunks);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
