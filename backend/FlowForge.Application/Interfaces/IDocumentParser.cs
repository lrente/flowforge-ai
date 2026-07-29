using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IDocumentParser
{
    Task<string> ExtractTextAsync(KnowledgeDocument document, CancellationToken cancellationToken = default);
}
