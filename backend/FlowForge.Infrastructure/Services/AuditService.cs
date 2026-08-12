using System.Text.Json;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace FlowForge.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db; private readonly ITenantContext _tenant; private readonly IHttpContextAccessor _http;
    public AuditService(ApplicationDbContext db, ITenantContext tenant, IHttpContextAccessor http) { _db = db; _tenant = tenant; _http = http; }
    public async Task WriteAsync(string action, string entityType, Guid entityId, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default)
    {
        var access = await _tenant.GetAccessAsync(cancellationToken); if (access is null) return;
        _db.AuditLogs.Add(new AuditLog { Id = Guid.NewGuid(), ClientId = access.ClientId, UserId = access.UserId, Action = action, EntityType = entityType, EntityId = entityId, OldValues = SafeJson(oldValues), NewValues = SafeJson(newValues), IpAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString(), UserAgent = _http.HttpContext?.Request.Headers.UserAgent.ToString(), CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow });
    }
    private static string? SafeJson(object? value) => value is null ? null : JsonSerializer.Serialize(value);
}
