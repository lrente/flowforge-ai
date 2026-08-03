using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;

namespace FlowForge.Infrastructure.Services;

public sealed class KnowledgeSearchService : IKnowledgeSearchService
{
    private readonly IKnowledgeRepository _knowledgeRepository;

    public KnowledgeSearchService(IKnowledgeRepository knowledgeRepository)
    {
        _knowledgeRepository = knowledgeRepository;
    }

    public Task<IReadOnlyList<KnowledgeChunk>> SearchAsync(Guid agentId, IReadOnlyList<float> embedding, int limit, CancellationToken cancellationToken = default)
    {
        return _knowledgeRepository.SearchByEmbeddingAsync(agentId, embedding, limit, cancellationToken);
    }
}
