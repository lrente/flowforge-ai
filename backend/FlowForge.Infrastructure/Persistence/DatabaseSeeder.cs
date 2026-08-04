using BCrypt.Net;
using FlowForge.Domain.Entities;

namespace FlowForge.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (db.Users.Any())
            return;

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@flowforge.ai",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync();
    }
}