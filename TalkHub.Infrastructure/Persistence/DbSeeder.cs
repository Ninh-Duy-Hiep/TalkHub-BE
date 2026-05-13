using Microsoft.EntityFrameworkCore;
using TalkHub.Domain.Entities;
using TalkHub.Domain.Enums;
using BC = BCrypt.Net.BCrypt;

namespace TalkHub.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = BC.HashPassword("Admin@123"),
                    FullName = "System Administrator",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    AvatarUrl = "https://example.com/avatars/admin.png",
                    Email = "admin@gmail.com",
                    IsDeleted = false,
                    PhoneNumber = "0999999999",
                    LastLoginAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user",
                    PasswordHash = BC.HashPassword("User@123"),
                    FullName = "Regular User",
                    Role = UserRole.Staff,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    AvatarUrl = "https://example.com/avatars/staff.png",
                    Email = "staff@gmail.com",
                    IsDeleted = false,
                    PhoneNumber = "0123456789",
                    LastLoginAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
