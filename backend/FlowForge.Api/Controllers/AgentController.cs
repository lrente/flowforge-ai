using FlowForge.Application.DTOs.Agent;
using FlowForge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AgentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgents(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var agents = await _agentService.GetAgentsAsync(userId.Value, cancellationToken);
        return Ok(agents);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgent(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var agent = await _agentService.GetAgentAsync(id, userId.Value, cancellationToken);
        return agent is null ? NotFound() : Ok(agent);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAgent([FromBody] CreateAgentRequest request, CancellationToken cancellationToken)
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

        var agent = await _agentService.CreateAgentAsync(userId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetAgent), new { id = agent.Id }, agent);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAgent(Guid id, [FromBody] UpdateAgentRequest request, CancellationToken cancellationToken)
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

        var agent = await _agentService.UpdateAgentAsync(id, userId.Value, request, cancellationToken);
        return agent is null ? NotFound() : Ok(agent);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAgent(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _agentService.DeleteAgentAsync(id, userId.Value, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : null;
    }
}
