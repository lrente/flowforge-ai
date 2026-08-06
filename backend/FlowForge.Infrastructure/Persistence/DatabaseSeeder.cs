using BCrypt.Net;
using FlowForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync())
            return;

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Email = "admin@flowforge.ai",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync();
    }
}