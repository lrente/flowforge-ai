using FlowForge.Application.Interfaces;
using FlowForge.Application.Security;
using FlowForge.Domain.Entities;
using FlowForge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Api.Controllers;

[ApiController, Authorize, Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _db; private readonly ITenantContext _tenant; private readonly IAuditService _audit;
    public ClientsController(ApplicationDbContext db, ITenantContext tenant, IAuditService audit) { _db = db; _tenant = tenant; _audit = audit; }

    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) { var access = await _tenant.GetAccessAsync(ct); if (access is null) return Forbid(); var clients = access.IsSystemAdministrator ? await _db.Clients.Where(c => c.IsActive).ToListAsync(ct) : await _db.Clients.Where(c => c.Id == access.ClientId).ToListAsync(ct); return Ok(clients.Select(c => new { c.Id, c.Name, c.Email, c.IsActive })); }
    [HttpPost] public async Task<IActionResult> Create(ClientRequest request, CancellationToken ct) { var access = await _tenant.GetAccessAsync(ct); if (access is not { IsSystemAdministrator: true }) return Forbid(); var now = DateTimeOffset.UtcNow; var client = new Client { Id = Guid.NewGuid(), Name = request.Name.Trim(), Email = request.Email.Trim().ToLowerInvariant(), IsActive = true, CreatedAt = now, UpdatedAt = now }; _db.Clients.Add(client); await _audit.WriteAsync("CREATE", "Client", client.Id, newValues: new { client.Name, client.Email }, cancellationToken: ct); await _db.SaveChangesAsync(ct); return CreatedAtAction(nameof(Get), new { id = client.Id }, new { client.Id, client.Name, client.Email }); }
    [HttpGet("{clientId:guid}/users")] public async Task<IActionResult> Users(Guid clientId, CancellationToken ct) { if (!await Can(clientId, Permissions.UsersView, ct)) return NotFound(); var users = await _db.ClientMemberships.Where(m => m.ClientId == clientId).Include(m => m.User).Select(m => new { m.UserId, m.User!.Name, m.User.Email, role = m.Role.ToString(), m.CreatedAt }).ToListAsync(ct); return Ok(users); }
    [HttpPost("{clientId:guid}/users/invite")] public async Task<IActionResult> Invite(Guid clientId, InviteRequest request, CancellationToken ct) { var access = await _tenant.GetAccessAsync(clientId, ct); if (access is null || !CanInvite(access, request.Role)) return Forbid(); if (await _db.ClientMemberships.Include(m => m.User).AnyAsync(m => m.ClientId == clientId && m.User!.Email == request.Email, ct)) return Conflict(new { message = "User is already a member." }); var token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)); var invitation = new Invitation { Id = Guid.NewGuid(), ClientId = clientId, Email = request.Email.Trim().ToLowerInvariant(), Role = request.Role, TokenHash = Hash(token), ExpiresAt = DateTimeOffset.UtcNow.AddHours(48), CreatedByUserId = access.UserId, Status = "Pending", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }; _db.Invitations.Add(invitation); await _audit.WriteAsync("INVITE", "Invitation", invitation.Id, newValues: new { invitation.Email, role = invitation.Role.ToString() }, cancellationToken: ct); await _db.SaveChangesAsync(ct); return Accepted(new { message = "Invitation created. Delivery requires the configured email service.", invitation.Id, invitation.ExpiresAt }); }
    [HttpGet("{clientId:guid}/audit-logs")] public async Task<IActionResult> AuditLogs(Guid clientId, CancellationToken ct) { if (!await Can(clientId, Permissions.AuditLogsView, ct)) return NotFound(); return Ok(await _db.AuditLogs.Where(a => a.ClientId == clientId).OrderByDescending(a => a.CreatedAt).Select(a => new { a.Id, a.UserId, a.Action, a.EntityType, a.EntityId, a.OldValues, a.NewValues, a.IpAddress, a.CreatedAt }).ToListAsync(ct)); }
    private async Task<bool> Can(Guid clientId, string permission, CancellationToken ct) { var access = await _tenant.GetAccessAsync(clientId, ct); return access is not null && (access.IsSystemAdministrator || Permissions.Has(access.Role, permission)); }
    private static bool CanInvite(TenantAccess a, ClientRole role) => a.IsSystemAdministrator || (a.Role == ClientRole.Admin) || (a.Role == ClientRole.Editor && role is ClientRole.Member or ClientRole.Guest);
    private static string Hash(string value) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value)));
}
public sealed record ClientRequest(string Name, string Email);
public sealed record InviteRequest(string Email, ClientRole Role);
