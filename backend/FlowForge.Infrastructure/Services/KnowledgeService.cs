using FlowForge.Application.DTOs.Knowledge;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace FlowForge.Infrastructure.Services;

public sealed class KnowledgeService
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IDocumentParser _documentParser;
    private readonly IKnowledgeProcessingService _knowledgeProcessingService;
    private readonly IConfiguration _configuration;

    public KnowledgeService(IKnowledgeRepository knowledgeRepository, IDocumentParser documentParser, IKnowledgeProcessingService knowledgeProcessingService, IConfiguration configuration)
    {
        _knowledgeRepository = knowledgeRepository;
        _documentParser = documentParser;
        _knowledgeProcessingService = knowledgeProcessingService;
        _configuration = configuration;
    }

    public async Task<KnowledgeDocumentResponse> UploadAsync(Guid agentId, UploadDocumentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Content is null || request.Length == 0)
        {
            throw new InvalidOperationException("A file is required.");
        }

        if (request.Length > 20 * 1024 * 1024)
        {
            throw new InvalidOperationException("File size must not exceed 20 MB.");
        }

        var allowedTypes = new[] { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
        if (!allowedTypes.Contains(request.ContentType))
        {
            throw new InvalidOperationException("Unsupported file type.");
        }

        var storageRoot = _configuration["Storage:DocumentsPath"] ?? Path.Combine(AppContext.BaseDirectory, "storage", "documents");
        Directory.CreateDirectory(storageRoot);

        var extension = Path.GetExtension(request.FileName);
        var savedFileName = $"{Guid.NewGuid():N}{extension}";
        var storagePath = Path.Combine(storageRoot, savedFileName);

        await using (var stream = File.Create(storagePath))
        {
            if (request.Content is not null)
            {
                await request.Content.CopyToAsync(stream, cancellationToken);
            }
        }

        var document = new KnowledgeDocument
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            Title = string.IsNullOrWhiteSpace(request.FileName) ? "Uploaded document" : Path.GetFileNameWithoutExtension(request.FileName),
            FileName = request.FileName,
            ContentType = request.ContentType,
            StoragePath = storagePath,
            Size = request.Length,
            Status = "Uploaded",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _knowledgeRepository.AddAsync(document, cancellationToken);
        await _knowledgeRepository.SaveChangesAsync(cancellationToken);

        await _knowledgeProcessingService.ProcessDocumentAsync(document, cancellationToken);

        return new KnowledgeDocumentResponse
        {
            Id = document.Id,
            AgentId = document.AgentId,
            Title = document.Title,
            FileName = document.FileName,
            ContentType = document.ContentType,
            StoragePath = document.StoragePath,
            Size = document.Size,
            Status = document.Status,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task<IReadOnlyList<KnowledgeDocumentResponse>> ListAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var documents = await _knowledgeRepository.GetByAgentIdAsync(agentId, cancellationToken);
        return documents.Select(Map).ToList();
    }

    public async Task<KnowledgeDocumentResponse?> GetByIdAsync(Guid id, Guid agentId, CancellationToken cancellationToken = default)
    {
        var document = await _knowledgeRepository.GetByIdForAgentAsync(id, agentId, cancellationToken);
        return document is null ? null : Map(document);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid agentId, CancellationToken cancellationToken = default)
    {
        var document = await _knowledgeRepository.GetByIdForAgentAsync(id, agentId, cancellationToken);
        if (document is null)
        {
            return false;
        }

        await _knowledgeRepository.DeleteAsync(document, cancellationToken);
        await _knowledgeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static KnowledgeDocumentResponse Map(KnowledgeDocument document)
    {
        return new KnowledgeDocumentResponse
        {
            Id = document.Id,
            AgentId = document.AgentId,
            Title = document.Title,
            FileName = document.FileName,
            ContentType = document.ContentType,
            StoragePath = document.StoragePath,
            Size = document.Size,
            Status = document.Status,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
