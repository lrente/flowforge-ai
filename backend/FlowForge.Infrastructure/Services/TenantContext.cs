using System.Security.Claims;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Services;

public sealed class TenantContext : ITenantContext
{
    private const string SystemAdminEmail = "lrente@gmail.com";
    private readonly IHttpContextAccessor _http; private readonly ApplicationDbContext _db;
    public TenantContext(IHttpContextAccessor http, ApplicationDbContext db) { _http = http; _db = db; }
    public Task<TenantAccess?> GetAccessAsync(CancellationToken cancellationToken = default)
    {
        var clientHeader = _http.HttpContext?.Request.Headers["X-Client-Id"].FirstOrDefault();
        return Guid.TryParse(clientHeader, out var clientId) ? GetAccessAsync(clientId, cancellationToken) : GetDefaultAsync(cancellationToken);
    }
    public async Task<TenantAccess?> GetAccessAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var identity = GetIdentity(); if (identity is null) return null;
        if (identity.Value.IsSystem && await _db.Clients.AnyAsync(c => c.Id == clientId && c.IsActive, cancellationToken)) return new(identity.Value.UserId, clientId, ClientRole.Admin, true);
        var membership = await _db.ClientMemberships.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == identity.Value.UserId && m.ClientId == clientId, cancellationToken);
        return membership is null ? null : new(identity.Value.UserId, membership.ClientId, membership.Role, false);
    }
    private async Task<TenantAccess?> GetDefaultAsync(CancellationToken cancellationToken)
    {
        var identity = GetIdentity(); if (identity is null) return null;
        var membership = await _db.ClientMemberships.AsNoTracking().OrderBy(m => m.CreatedAt).FirstOrDefaultAsync(m => m.UserId == identity.Value.UserId, cancellationToken);
        return membership is null ? null : new(identity.Value.UserId, membership.ClientId, membership.Role, identity.Value.IsSystem);
    }
    private (Guid UserId, bool IsSystem)? GetIdentity()
    {
        var user = _http.HttpContext?.User; var sub = user?.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var id)) return null;
        return (id, string.Equals(user?.FindFirstValue(ClaimTypes.Email) ?? user?.FindFirstValue("email"), SystemAdminEmail, StringComparison.OrdinalIgnoreCase));
    }
}
