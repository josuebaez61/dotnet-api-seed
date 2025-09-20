using System;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Common.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Seeder for permissions
  /// </summary>
  public class PermissionSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;

    // Static GUIDs for consistent seeding
    private static readonly Guid ManageRolesPermissionId = new Guid("32edea54-6b49-4f4f-8257-aa1992f23c28");
    private static readonly Guid ManageUsersPermissionId = new Guid("e1d015ea-0d8a-42b5-a0c1-237a8e018999");
    private static readonly Guid ManageUserRolesPermissionId = new Guid("3c883108-b93d-4142-acc8-bbd67f694fb1");
    private static readonly Guid AdminPermissionId = new Guid("082a40e0-2ff4-4c05-a078-4dfaf778172f");
    private static readonly Guid SuperAdminPermissionId = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private static readonly DateTime SeedTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public string Name => "Permissions";

    public PermissionSeeder(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task SeedAsync()
    {
      Console.WriteLine($"🌱 Seeding {Name}...");

      // Check if permissions already exist
      if (await _context.Permissions.AnyAsync())
      {
        Console.WriteLine($"✅ {Name} already seeded, skipping...");
        return;
      }

      var permissions = new[]
      {
        new Domain.Entities.Permission
        {
          Id = ManageRolesPermissionId,
          Name = PermissionNames.ManageRoles,
          Description = "Manage roles (create, update, delete, read)",
          Resource = "roles",
          CreatedAt = SeedTimestamp
        },
        new Domain.Entities.Permission
        {
          Id = ManageUsersPermissionId,
          Name = PermissionNames.ManageUsers,
          Description = "Manage users (create, update, delete, read)",
          Resource = "users",
          CreatedAt = SeedTimestamp
        },
        new Domain.Entities.Permission
        {
          Id = ManageUserRolesPermissionId,
          Name = PermissionNames.ManageUserRoles,
          Description = "Manage user-role assignments",
          Resource = "users",
          CreatedAt = SeedTimestamp
        },
        new Domain.Entities.Permission
        {
          Id = AdminPermissionId,
          Name = PermissionNames.Admin,
          Description = "Administrative access",
          Resource = "system",
          CreatedAt = SeedTimestamp
        },
        new Domain.Entities.Permission
        {
          Id = SuperAdminPermissionId,
          Name = PermissionNames.SuperAdmin,
          Description = "Super administrative access (includes all permissions)",
          Resource = "system",
          CreatedAt = SeedTimestamp
        }
      };

      await _context.Permissions.AddRangeAsync(permissions);
      await _context.SaveChangesAsync();

      Console.WriteLine($"✅ {Name} seeded successfully");
    }

    // Public getters for other seeders to use
    public static Guid ManageRolesPermissionIdValue => ManageRolesPermissionId;
    public static Guid ManageUsersPermissionIdValue => ManageUsersPermissionId;
    public static Guid ManageUserRolesPermissionIdValue => ManageUserRolesPermissionId;
    public static Guid AdminPermissionIdValue => AdminPermissionId;
    public static Guid SuperAdminPermissionIdValue => SuperAdminPermissionId;
  }
}
