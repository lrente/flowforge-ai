using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Infrastructure.Services;
using Moq;
using Xunit;

namespace FlowForge.Tests;

public sealed class KnowledgeSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        var repository = new Mock<IKnowledgeRepository>();
        var agentId = Guid.NewGuid();
        var expected = new List<KnowledgeChunk>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DocumentId = Guid.NewGuid(),
                Content = "Relevant chunk",
                ChunkIndex = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            }
        };

        var embedding = new[] { 0.1f, 0.2f, 0.3f };
        repository.Setup(r => r.SearchByEmbeddingAsync(agentId, It.Is<IReadOnlyList<float>>(values => values.SequenceEqual(embedding)), 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new KnowledgeSearchService(repository.Object);

        var results = await service.SearchAsync(agentId, embedding, 3);

        Assert.Same(expected, results);
        repository.Verify(r => r.SearchByEmbeddingAsync(agentId, It.Is<IReadOnlyList<float>>(values => values.SequenceEqual(embedding)), 3, It.IsAny<CancellationToken>()), Times.Once);
    }
}
