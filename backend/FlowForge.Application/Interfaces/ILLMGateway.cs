using FlowForge.Application.AI;

namespace FlowForge.Application.Interfaces;

public interface ILLMGateway
{
    Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken cancellationToken = default);
}
