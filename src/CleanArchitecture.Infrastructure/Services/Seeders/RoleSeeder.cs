using System;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Seeder for roles
  /// </summary>
  public class RoleSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<Role> _roleManager;

    // Static GUIDs for consistent seeding
    private static readonly Guid AdminRoleId = new Guid("11734856-f5ac-4ee1-b28d-e35ef9fc0d4f");
    private static readonly Guid UserRoleId = new Guid("c894dc98-510d-4a96-8e90-aecf829d2a7c");
    private static readonly DateTime SeedTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public string Name => "Roles";

    public RoleSeeder(ApplicationDbContext context, RoleManager<Role> roleManager)
    {
      _context = context;
      _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
      Console.WriteLine($"🌱 Seeding {Name}...");

      // Check if roles already exist
      if (await _context.Roles.AnyAsync())
      {
        Console.WriteLine($"✅ {Name} already seeded, skipping...");
        return;
      }

      var adminRole = new Role
      {
        Id = AdminRoleId,
        Name = "Admin",
        NormalizedName = "ADMIN",
        Description = "Administrator role with full access",
        CreatedAt = SeedTimestamp
      };

      var userRole = new Role
      {
        Id = UserRoleId,
        Name = "User",
        NormalizedName = "USER",
        Description = "Standard user role",
        CreatedAt = SeedTimestamp
      };

      await _roleManager.CreateAsync(adminRole);
      await _roleManager.CreateAsync(userRole);

      Console.WriteLine($"✅ {Name} seeded successfully");
    }

    // Public getters for other seeders to use
    public static Guid AdminRoleIdValue => AdminRoleId;
    public static Guid UserRoleIdValue => UserRoleId;
  }
}
