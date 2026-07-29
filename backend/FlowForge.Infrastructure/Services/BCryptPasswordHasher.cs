using FlowForge.Application.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace FlowForge.Infrastructure.Services;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BC.Verify(password, passwordHash);
    }
}
