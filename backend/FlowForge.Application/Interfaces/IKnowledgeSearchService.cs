using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IKnowledgeSearchService
{
    Task<IReadOnlyList<KnowledgeChunk>> SearchAsync(Guid agentId, IReadOnlyList<float> embedding, int limit, CancellationToken cancellationToken = default);
}
