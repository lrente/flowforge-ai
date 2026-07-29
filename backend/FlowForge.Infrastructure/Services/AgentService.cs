using FlowForge.Application.DTOs.Agent;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;

namespace FlowForge.Infrastructure.Services;

public sealed class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;

    public AgentService(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public async Task<IReadOnlyList<AgentResponse>> GetAgentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var agents = await _agentRepository.GetByUserIdAsync(userId, cancellationToken);
        return agents.Select(MapToResponse).ToList();
    }

    public async Task<AgentResponse?> GetAgentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdForUserAsync(id, userId, cancellationToken);
        return agent is null ? null : MapToResponse(agent);
    }

    public async Task<AgentResponse> CreateAgentAsync(Guid userId, CreateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            UserId = userId,
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

        return MapToResponse(agent);
    }

    public async Task<AgentResponse?> UpdateAgentAsync(Guid id, Guid userId, UpdateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdForUserAsync(id, userId, cancellationToken);
        if (agent is null)
        {
            return null;
        }

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

        return MapToResponse(agent);
    }

    public async Task<bool> DeleteAgentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdForUserAsync(id, userId, cancellationToken);
        if (agent is null)
        {
            return false;
        }

        await _agentRepository.DeleteAsync(agent, cancellationToken);
        await _agentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

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
