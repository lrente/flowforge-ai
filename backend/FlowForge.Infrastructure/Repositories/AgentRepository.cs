using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Repositories;

public sealed class AgentRepository : IAgentRepository
{
    private readonly ApplicationDbContext _context;

    public AgentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Agent>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Agents
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Agent>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default) => await _context.Agents.Where(a => a.ClientId == clientId).OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agents.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Agent?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Agents.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);
    }
    public Task<Agent?> GetByIdForClientAsync(Guid id, Guid clientId, CancellationToken cancellationToken = default) => _context.Agents.FirstOrDefaultAsync(a => a.Id == id && a.ClientId == clientId, cancellationToken);

    public async Task AddAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        await _context.Agents.AddAsync(agent, cancellationToken);
    }

    public Task UpdateAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        _context.Agents.Update(agent);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        _context.Agents.Remove(agent);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
