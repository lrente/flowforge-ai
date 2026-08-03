namespace FlowForge.Application.Interfaces;

public interface IEmbeddingService
{
    Task<IReadOnlyList<float>> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IReadOnlyList<float>>> CreateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
}
