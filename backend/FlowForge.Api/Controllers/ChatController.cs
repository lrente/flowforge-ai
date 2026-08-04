using FlowForge.Application.DTOs.Chat;
using FlowForge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public sealed class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> chatserviceGetConversations(CancellationToken cancellationToken)
    {
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
}
