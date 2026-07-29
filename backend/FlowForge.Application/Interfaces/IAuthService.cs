using FlowForge.Application.DTOs.Auth;
using FlowForge.Domain.Entities;

namespace FlowForge.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<User?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
