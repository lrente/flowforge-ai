using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;

namespace FlowForge.Infrastructure.Services;

public sealed class KnowledgeProcessingService : IKnowledgeProcessingService
{
    private readonly IDocumentParser _documentParser;
    private readonly ITextChunker _textChunker;
    private readonly IEmbeddingService _embeddingService;
    private readonly IKnowledgeRepository _knowledgeRepository;

    public KnowledgeProcessingService(
        IDocumentParser documentParser,
        ITextChunker textChunker,
        IEmbeddingService embeddingService,
        IKnowledgeRepository knowledgeRepository)
    {
        _documentParser = documentParser;
        _textChunker = textChunker;
        _embeddingService = embeddingService;
        _knowledgeRepository = knowledgeRepository;
    }

    public async Task ProcessDocumentAsync(KnowledgeDocument document, CancellationToken cancellationToken = default)
    {
        var text = await _documentParser.ExtractTextAsync(document, cancellationToken);
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var chunks = _textChunker.Chunk(text);
        var embeddings = await _embeddingService.CreateEmbeddingsAsync(chunks, cancellationToken);

        for (var index = 0; index < chunks.Count; index++)
        {
            var embedding = embeddings.ElementAtOrDefault(index);
            var chunk = new KnowledgeChunk
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                Content = chunks[index],
                ChunkIndex = index,
                Embedding = embedding is null ? null : string.Join(",", embedding.Select(value => value.ToString(System.Globalization.CultureInfo.InvariantCulture))),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _knowledgeRepository.AddChunkAsync(chunk, cancellationToken);
        }

        await _knowledgeRepository.SaveChangesAsync(cancellationToken);
    }
}
