using System.Security.Claims;
using FlowForge.Domain.Entities;
using FlowForge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Api.Controllers;
[ApiController, Authorize, Route("api/invitations")]
public sealed class InvitationsController : ControllerBase
{
    private readonly ApplicationDbContext _db; public InvitationsController(ApplicationDbContext db) => _db = db;
    [HttpPost("{token}/accept")] public async Task<IActionResult> Accept(string token, CancellationToken ct) { var userId = User.FindFirstValue("sub"); if (!Guid.TryParse(userId, out var id)) return Unauthorized(); var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token))); var invite = await _db.Invitations.FirstOrDefaultAsync(i => i.TokenHash == hash && i.Status == "Pending", ct); if (invite is null || invite.ExpiresAt <= DateTimeOffset.UtcNow) return BadRequest(new { message = "Invitation is invalid or expired." }); if (await _db.ClientMemberships.AnyAsync(m => m.ClientId == invite.ClientId && m.UserId == id, ct)) return Conflict(); invite.Status = "Accepted"; invite.AcceptedAt = invite.UpdatedAt = DateTimeOffset.UtcNow; _db.ClientMemberships.Add(new ClientMembership { Id = Guid.NewGuid(), ClientId = invite.ClientId, UserId = id, Role = invite.Role, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }); await _db.SaveChangesAsync(ct); return Ok(); }
}
