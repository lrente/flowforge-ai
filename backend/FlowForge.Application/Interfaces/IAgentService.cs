using FlowForge.Application.DTOs.Agent;
using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IAgentService
{
    Task<IReadOnlyList<AgentResponse>> GetAgentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AgentResponse?> GetAgentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<AgentResponse> CreateAgentAsync(Guid userId, CreateAgentRequest request, CancellationToken cancellationToken = default);
    Task<AgentResponse?> UpdateAgentAsync(Guid id, Guid userId, UpdateAgentRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAgentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
