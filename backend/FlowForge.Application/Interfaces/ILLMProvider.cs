using FlowForge.Application.AI;

namespace FlowForge.Application.Interfaces;

public interface ILLMProvider
{
    string Name { get; }

    Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken cancellationToken = default);
}
