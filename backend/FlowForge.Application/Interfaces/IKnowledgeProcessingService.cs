using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IKnowledgeProcessingService
{
    Task ProcessDocumentAsync(KnowledgeDocument document, CancellationToken cancellationToken = default);
}
