using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Repositories;

public sealed class KnowledgeRepository : IKnowledgeRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<KnowledgeDocument>> GetByAgentIdAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeDocuments
            .Where(d => d.AgentId == agentId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<KnowledgeDocument?> GetByIdForAgentAsync(Guid id, Guid agentId, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeDocuments.FirstOrDefaultAsync(d => d.Id == id && d.AgentId == agentId, cancellationToken);
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
