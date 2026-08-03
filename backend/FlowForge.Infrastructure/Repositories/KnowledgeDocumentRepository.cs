using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Repositories;

public sealed class KnowledgeDocumentRepository : IKnowledgeDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<KnowledgeDocument>> GetByAgentIdAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeDocuments
            .Where(document => document.AgentId == agentId)
            .OrderByDescending(document => document.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<KnowledgeDocument?> GetByIdForAgentAsync(Guid id, Guid agentId, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeDocuments
            .Include(document => document.Chunks.OrderBy(chunk => chunk.ChunkIndex))
            .FirstOrDefaultAsync(document => document.Id == id && document.AgentId == agentId, cancellationToken);
    }

    public async Task AddAsync(KnowledgeDocument document, CancellationToken cancellationToken = default)
    {
        await _context.KnowledgeDocuments.AddAsync(document, cancellationToken);
    }

    public Task DeleteAsync(KnowledgeDocument document, CancellationToken cancellationToken = default)
    {
        _context.KnowledgeDocuments.Remove(document);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
