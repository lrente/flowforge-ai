using BCrypt.Net;
using FlowForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.MigrateAsync();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "lrente@gmail.com");
        if (user is null)
        {
            user = new User { Id = Guid.NewGuid(), Name = "Administrator", Email = "lrente@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Miguel84marta"), CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
            db.Users.Add(user);
        }
        if (!await db.ClientMemberships.AnyAsync(m => m.UserId == user.Id))
        {
            var now = DateTimeOffset.UtcNow;
            var client = new Client { Id = Guid.NewGuid(), Name = "FlowForge Demo", Email = user.Email, IsActive = true, CreatedAt = now, UpdatedAt = now };
            db.Clients.Add(client);
            db.ClientMemberships.Add(new ClientMembership { Id = Guid.NewGuid(), ClientId = client.Id, UserId = user.Id, Role = ClientRole.Admin, CreatedAt = now, UpdatedAt = now });
        }

        await db.SaveChangesAsync();
    }
}
