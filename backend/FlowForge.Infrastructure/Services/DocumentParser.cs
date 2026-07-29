using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;

namespace FlowForge.Infrastructure.Services;

public sealed class DocumentParser : IDocumentParser
{
    public Task<string> ExtractTextAsync(KnowledgeDocument document, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(string.Empty);
    }
}
