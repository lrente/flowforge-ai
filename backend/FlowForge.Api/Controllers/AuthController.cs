using FlowForge.Application.DTOs.Auth;
using FlowForge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlowForge.Infrastructure.Persistence;
using FlowForge.Application.Interfaces;
using FlowForge.Application.Security;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _db;
    private readonly ITenantContext _tenant;

    public AuthController(IAuthService authService, ApplicationDbContext db, ITenantContext tenant)
    {
        _authService = authService;
        _db = db;
        _tenant = tenant;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        var access = await _tenant.GetAccessAsync(cancellationToken);
        if (user is null || access is null) return NotFound();
        var client = await _db.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == access.ClientId, cancellationToken);
        return client is null ? NotFound() : Ok(new { user.Id, user.Name, user.Email, client = new { client.Id, client.Name }, role = access.Role.ToString(), permissions = Permissions.For(access.Role) });
    }
}
