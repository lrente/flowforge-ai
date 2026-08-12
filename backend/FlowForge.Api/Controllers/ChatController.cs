using FlowForge.Application.DTOs.Chat;
using FlowForge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FlowForge.Application.Interfaces;
using FlowForge.Application.Security;

namespace FlowForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ITenantContext _tenant;

    public ChatController(IChatService chatService, ITenantContext tenant)
    {
        _chatService = chatService;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<IActionResult> chatserviceGetConversations(CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.ConversationsView, cancellationToken)) return Forbid();
        var visitorId = GetVisitorId();
        if (visitorId is null)
        {
            return Unauthorized();
        }

        var conversations = await _chatService.GetConversationsAsync(visitorId.Value, cancellationToken);
        return Ok(conversations);
    }

    [HttpGet("{conversationId:guid}")]
    public async Task<IActionResult> GetConversation(Guid conversationId, CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.ConversationsView, cancellationToken)) return Forbid();
        var visitorId = GetVisitorId();
        if (visitorId is null)
        {
            return Unauthorized();
        }

        var conversation = await _chatService.GetConversationAsync(conversationId, visitorId.Value, cancellationToken);
        return conversation is null ? NotFound() : Ok(conversation);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.ConversationsUse, cancellationToken)) return Forbid();
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var visitorId = GetVisitorId();
        if (visitorId is null)
        {
            return Unauthorized();
        }

        try
        {
            var response = await _chatService.SendMessageAsync(request.AgentId, request.Message, visitorId.Value, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{conversationId:guid}/messages")]
    public async Task<IActionResult> PostToConversation(Guid conversationId, [FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.ConversationsUse, cancellationToken)) return Forbid();
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var visitorId = GetVisitorId();
        if (visitorId is null)
        {
            return Unauthorized();
        }

        try
        {
            var response = await _chatService.SendMessageToConversationAsync(conversationId, request.Message, visitorId.Value, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{conversationId:guid}")]
    public async Task<IActionResult> DeleteConversation(Guid conversationId, CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.ConversationsUse, cancellationToken)) return Forbid();
        var visitorId = GetVisitorId();
        if (visitorId is null)
        {
            return Unauthorized();
        }

        await _chatService.DeleteConversationAsync(conversationId, visitorId.Value, cancellationToken);
        return NoContent();
    }

    private Guid? GetVisitorId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : null;
    }
    private async Task<bool> HasAsync(string permission, CancellationToken ct) { var access = await _tenant.GetAccessAsync(ct); return access is not null && (access.IsSystemAdministrator || Permissions.Has(access.Role, permission)); }
}
