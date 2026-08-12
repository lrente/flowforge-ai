using FlowForge.Application.DTOs.Auth;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Persistence;

namespace FlowForge.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ApplicationDbContext _db;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IPasswordHasher passwordHasher, ApplicationDbContext db)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _db = db;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var client = new Client { Id = Guid.NewGuid(), Name = $"{user.Name}'s workspace", Email = user.Email, IsActive = true, CreatedAt = now, UpdatedAt = now };
        _db.Clients.Add(client);
        _db.ClientMemberships.Add(new ClientMembership { Id = Guid.NewGuid(), ClientId = client.Id, UserId = user.Id, Role = ClientRole.Admin, CreatedAt = now, UpdatedAt = now });
        await _db.SaveChangesAsync(cancellationToken);

        return await GenerateResponseAsync(user, cancellationToken);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return await GenerateResponseAsync(user, cancellationToken);
    }

    public Task<User?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByIdAsync(userId, cancellationToken);
    }

    private Task<LoginResponse> GenerateResponseAsync(User user, CancellationToken cancellationToken)
    {
        var token = _jwtService.GenerateToken(user);
        var response = new LoginResponse
        {
            AccessToken = token,
            ExpiresIn = 28800,
            Email = user.Email,
            Name = user.Name
        };

        return Task.FromResult(response);
    }
}
