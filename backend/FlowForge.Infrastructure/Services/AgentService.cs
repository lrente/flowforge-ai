using System.ComponentModel.DataAnnotations;
using FlowForge.Application.DTOs.Agent;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using  FlowForge.Infrastructure.Helpers;

public sealed class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _audit;

    public AgentService(IAgentRepository agentRepository, ITenantContext tenantContext, IAuditService audit)
    {
        _agentRepository = agentRepository;
        _tenantContext = tenantContext;
        _audit = audit;
    }

    public async Task<IReadOnlyList<AgentResponse>> GetAgentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var access = await RequireTenantAsync(cancellationToken);
        var agents = await _agentRepository.GetByClientIdAsync(access.ClientId, cancellationToken);
        return agents.Select(MapToResponse).ToList();
    }

    public async Task<AgentResponse?> GetAgentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var access = await RequireTenantAsync(cancellationToken);
        var agent = await _agentRepository.GetByIdForClientAsync(id, access.ClientId, cancellationToken);
        return agent is null ? null : MapToResponse(agent);
    }

    public async Task<AgentResponse> CreateAgentAsync(Guid userId, CreateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var access = await RequireTenantAsync(cancellationToken);
        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClientId = access.ClientId,
            Name = request.Name.Trim(),
            BusinessType = request.BusinessType.Trim(),
            Description = request.Description.Trim(),
            CompanyName = request.CompanyName.Trim(),
            SystemPrompt = request.SystemPrompt.Trim(),
            Model = request.Model.Trim(),
            Temperature = request.Temperature,
            IsActive = request.IsActive,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _agentRepository.AddAsync(agent, cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);
        await _audit.WriteAsync("CREATE", "Agent", agent.Id, newValues: new { agent.Name, agent.Model }, cancellationToken: cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(agent);
    }

    public async Task<AgentResponse?> UpdateAgentAsync(Guid id, Guid userId, UpdateAgentRequest request, CancellationToken cancellationToken = default)
    {
        if (!SupportedModels.All.Contains(request.Model))
            throw new ValidationException("Unsupported model.");

        var access = await RequireTenantAsync(cancellationToken);
        var agent = await _agentRepository.GetByIdForClientAsync(id, access.ClientId, cancellationToken);
        if (agent is null)
        {
            return null;
        }

        var oldValues = new { agent.Name, agent.Model, agent.IsActive };
        agent.Name = request.Name.Trim();
        agent.BusinessType = request.BusinessType.Trim();
        agent.Description = request.Description.Trim();
        agent.CompanyName = request.CompanyName.Trim();
        agent.SystemPrompt = request.SystemPrompt.Trim();
        agent.Model = request.Model.Trim();
        agent.Temperature = request.Temperature;
        agent.IsActive = request.IsActive;
        agent.UpdatedAt = DateTimeOffset.UtcNow;

        await _agentRepository.UpdateAsync(agent, cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);
        await _audit.WriteAsync("UPDATE", "Agent", agent.Id, oldValues, new { agent.Name, agent.Model, agent.IsActive }, cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(agent);
    }

    public async Task<bool> DeleteAgentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var access = await RequireTenantAsync(cancellationToken);
        var agent = await _agentRepository.GetByIdForClientAsync(id, access.ClientId, cancellationToken);
        if (agent is null)
        {
            return false;
        }

        await _agentRepository.DeleteAsync(agent, cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);
        await _audit.WriteAsync("DELETE", "Agent", agent.Id, oldValues: new { agent.Name }, cancellationToken: cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<TenantAccess> RequireTenantAsync(CancellationToken cancellationToken) => await _tenantContext.GetAccessAsync(cancellationToken) ?? throw new UnauthorizedAccessException("No active client membership.");

    private static AgentResponse MapToResponse(Agent agent)
    {
        return new AgentResponse
        {
            Id = agent.Id,
            UserId = agent.UserId,
            Name = agent.Name,
            BusinessType = agent.BusinessType,
            Description = agent.Description,
            CompanyName = agent.CompanyName,
            SystemPrompt = agent.SystemPrompt,
            Model = agent.Model,
            Temperature = agent.Temperature,
            IsActive = agent.IsActive,
            CreatedAt = agent.CreatedAt,
            UpdatedAt = agent.UpdatedAt
        };
    }
}
