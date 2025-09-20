using System;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Seeder for admin user
  /// </summary>
  public class AdminUserSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    // Static GUIDs for consistent seeding
    private static readonly Guid AdminUserId = new Guid("f9e8d7c6-b5a4-3210-9876-543210fedcba");

    public string Name => "AdminUser";

    public AdminUserSeeder(ApplicationDbContext context, UserManager<User> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public async Task SeedAsync()
    {
      Console.WriteLine($"🌱 Seeding {Name}...");

      // Check if admin user already exists
      var adminUser = await _userManager.FindByNameAsync("admin");
      if (adminUser != null)
      {
        Console.WriteLine($"✅ {Name} already seeded, skipping...");
        return;
      }

      // Create admin user
      var user = new User
      {
        Id = AdminUserId,
        UserName = "admin",
        NormalizedUserName = "ADMIN",
        Email = "admin@example.com",
        NormalizedEmail = "ADMIN@EXAMPLE.COM",
        EmailConfirmed = true,
        FirstName = "Admin",
        LastName = "User",
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      // Create user with password
      var result = await _userManager.CreateAsync(user, "Admin123!");

      if (result.Succeeded)
      {
        // Find admin role
        var adminRole = await _userManager.GetRolesAsync(user);
        if (adminRole.Count == 0)
        {
          // Assign admin role to user
          await _userManager.AddToRoleAsync(user, "Admin");
        }

        Console.WriteLine($"✅ {Name} seeded successfully");
      }
      else
      {
        Console.WriteLine($"❌ Failed to create {Name}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }
    }
  }
}
