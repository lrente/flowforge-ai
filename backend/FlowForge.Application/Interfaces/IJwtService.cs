using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
