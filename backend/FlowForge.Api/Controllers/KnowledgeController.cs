using FlowForge.Application.DTOs.Knowledge;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FlowForge.Api.Models;
using FlowForge.Application.Interfaces;
using FlowForge.Application.Security;

namespace FlowForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class KnowledgeController : ControllerBase
{
    private readonly KnowledgeService _knowledgeService;
    private readonly IAgentRepository _agentRepository;
    private readonly ITenantContext _tenant;

    public KnowledgeController(KnowledgeService knowledgeService, IAgentRepository agentRepository, ITenantContext tenant)
    {
        _knowledgeService = knowledgeService;
        _agentRepository = agentRepository;
        _tenant = tenant;
    }


[HttpPost("upload")]
public async Task<IActionResult> Upload(
    [FromForm] UploadDocumentForm request,
    CancellationToken cancellationToken)
{
    if (!await HasAsync(Permissions.KnowledgeCreate, cancellationToken)) return Forbid();
    if (!ModelState.IsValid)
        return ValidationProblem(ModelState);

    var userId = GetUserId();
    if (userId is null)
        return Unauthorized();

    var access = await _tenant.GetAccessAsync(cancellationToken);
    var agent = access is null ? null : await _agentRepository.GetByIdForClientAsync(request.AgentId, access.ClientId, cancellationToken);

    if (agent is null || agent.UserId != userId.Value)
        return Forbid();

    await using var stream = request.File.OpenReadStream();

    var uploadRequest = new UploadDocumentRequest
    {
        AgentId = request.AgentId,
        FileName = request.File.FileName,
        ContentType = request.File.ContentType,
        Length = request.File.Length,
        Content = stream
    };

    try
    {
        var response = await _knowledgeService.UploadAsync(
            request.AgentId,
            uploadRequest,
            cancellationToken);

        return Ok(response);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

    [HttpGet]
    public async Task<IActionResult> GetDocuments(CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.KnowledgeView, cancellationToken)) return Forbid();
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
        if (!await HasAsync(Permissions.KnowledgeView, cancellationToken)) return Forbid();
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
        if (!await HasAsync(Permissions.KnowledgeDelete, cancellationToken)) return Forbid();
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
    private async Task<bool> HasAsync(string permission, CancellationToken ct) { var access = await _tenant.GetAccessAsync(ct); return access is not null && (access.IsSystemAdministrator || Permissions.Has(access.Role, permission)); }
}
