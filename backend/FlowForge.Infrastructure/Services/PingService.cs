using FlowForge.Application.Interfaces;

namespace FlowForge.Infrastructure.Services;

public sealed class PingService : IPingService
{
    public string Ping() => "pong";
}
