using System.Security.Claims;
using FlowForge.Application.DTOs.Agent;
using FlowForge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlowForge.Application.Security;

namespace FlowForge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/agents")]
public sealed class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ITenantContext _tenantContext;

    public AgentsController(IAgentService agentService, ITenantContext tenantContext)
    {
        _agentService = agentService;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AgentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<AgentResponse>>> GetAgents(
        CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.AgentsView, cancellationToken)) return Forbid();
        var userId = GetUserId();

        var agents = await _agentService.GetAgentsAsync(
            userId,
            cancellationToken);

        return Ok(agents);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AgentResponse>> GetAgent(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.AgentsView, cancellationToken)) return Forbid();
        var userId = GetUserId();

        var agent = await _agentService.GetAgentAsync(
            id,
            userId,
            cancellationToken);

        return agent is null
            ? NotFound()
            : Ok(agent);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AgentResponse>> CreateAgent(
        CreateAgentRequest request,
        CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.AgentsCreate, cancellationToken)) return Forbid();
        var userId = GetUserId();

        var agent = await _agentService.CreateAgentAsync(
            userId,
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetAgent),
            new { id = agent.Id },
            agent);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AgentResponse>> UpdateAgent(
        Guid id,
        UpdateAgentRequest request,
        CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.AgentsUpdate, cancellationToken)) return Forbid();
        var userId = GetUserId();

        var agent = await _agentService.UpdateAgentAsync(
            id,
            userId,
            request,
            cancellationToken);

        return agent is null
            ? NotFound()
            : Ok(agent);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAgent(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!await HasAsync(Permissions.AgentsDelete, cancellationToken)) return Forbid();
        var userId = GetUserId();

        var deleted = await _agentService.DeleteAgentAsync(
            id,
            userId,
            cancellationToken);

        return deleted
            ? NoContent()
            : NotFound();
    }

    private Guid GetUserId()
    {
        var claim =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!Guid.TryParse(claim, out var userId))
        {
            throw new UnauthorizedAccessException("User identifier not found in JWT.");
        }

        return userId;
    }

    private async Task<bool> HasAsync(string permission, CancellationToken cancellationToken)
    {
        var access = await _tenantContext.GetAccessAsync(cancellationToken);
        return access is not null && (access.IsSystemAdministrator || Permissions.Has(access.Role, permission));
    }
}
