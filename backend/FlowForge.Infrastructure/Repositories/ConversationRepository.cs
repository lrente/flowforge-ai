using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Repositories;

public sealed class ConversationRepository : IConversationRepository
{
    private readonly ApplicationDbContext _context;

    public ConversationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Conversation>> GetByVisitorIdAsync(Guid visitorId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.VisitorId == visitorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation?> GetByIdForVisitorAsync(Guid id, Guid visitorId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == id && c.VisitorId == visitorId, cancellationToken);
    }

    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Conversation?> GetByAgentAndVisitorAsync(Guid agentId, Guid visitorId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.AgentId == agentId && c.VisitorId == visitorId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _context.Conversations.AddAsync(conversation, cancellationToken);
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Update(conversation);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Remove(conversation);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
