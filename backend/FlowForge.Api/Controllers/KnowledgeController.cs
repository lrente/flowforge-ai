using FlowForge.Application.DTOs.Knowledge;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class KnowledgeController : ControllerBase
{
    private readonly KnowledgeService _knowledgeService;
    private readonly IAgentRepository _agentRepository;

    public KnowledgeController(KnowledgeService knowledgeService, IAgentRepository agentRepository)
    {
        _knowledgeService = knowledgeService;
        _agentRepository = agentRepository;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var uploadRequest = new UploadDocumentRequest
        {
            AgentId = request.AgentId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            Content = request.Content,
            Length = request.Length
        };

        var agent = await _agentRepository.GetByIdAsync(request.AgentId, cancellationToken);
        if (agent is null || agent.UserId != userId.Value)
        {
            return Forbid();
        }

        try
        {
            var response = await _knowledgeService.UploadAsync(request.AgentId, uploadRequest, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var documents = await _knowledgeService.ListAsync(userId.Value, cancellationToken);
        return Ok(documents);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDocument(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var document = await _knowledgeService.GetByIdAsync(id, userId.Value, cancellationToken);
        return document is null ? NotFound() : Ok(document);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _knowledgeService.DeleteAsync(id, userId.Value, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : null;
    }
}
