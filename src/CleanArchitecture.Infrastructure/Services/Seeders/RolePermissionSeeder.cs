using System;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Seeder for role permissions
  /// </summary>
  public class RolePermissionSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;

    // Static GUIDs for consistent seeding
    private static readonly Guid AdminManageRolesPermissionId = new Guid("54135e51-88ee-490b-92b6-31fc3802db45");
    private static readonly Guid AdminManageUsersPermissionId = new Guid("8fccec4a-199d-4e4d-8f33-659d7ea41d8c");
    private static readonly Guid AdminManageUserRolesPermissionId = new Guid("94ef9ec0-390e-4e4f-8da4-80a49ea997b6");
    private static readonly Guid AdminAdminPermissionId = new Guid("ea00cad1-99a9-415c-b8b8-4bf5efd0f81b");
    private static readonly Guid AdminSuperAdminPermissionId = new Guid("f1a2b3c4-d5e6-7890-abcd-ef1234567890");
    private static readonly DateTime SeedTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public string Name => "RolePermissions";

    public RolePermissionSeeder(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task SeedAsync()
    {
      Console.WriteLine($"🌱 Seeding {Name}...");

      // Check if role permissions already exist
      if (await _context.RolePermissions.AnyAsync())
      {
        Console.WriteLine($"✅ {Name} already seeded, skipping...");
        return;
      }

      var rolePermissions = new[]
      {
        // Admin role gets all permissions
        new RolePermission
        {
          Id = AdminManageRolesPermissionId,
          RoleId = RoleSeeder.AdminRoleIdValue,
          PermissionId = PermissionSeeder.ManageRolesPermissionIdValue,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminManageUsersPermissionId,
          RoleId = RoleSeeder.AdminRoleIdValue,
          PermissionId = PermissionSeeder.ManageUsersPermissionIdValue,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminManageUserRolesPermissionId,
          RoleId = RoleSeeder.AdminRoleIdValue,
          PermissionId = PermissionSeeder.ManageUserRolesPermissionIdValue,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminAdminPermissionId,
          RoleId = RoleSeeder.AdminRoleIdValue,
          PermissionId = PermissionSeeder.AdminPermissionIdValue,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminSuperAdminPermissionId,
          RoleId = RoleSeeder.AdminRoleIdValue,
          PermissionId = PermissionSeeder.SuperAdminPermissionIdValue,
          CreatedAt = SeedTimestamp
        }
      };

      await _context.RolePermissions.AddRangeAsync(rolePermissions);
      await _context.SaveChangesAsync();

      Console.WriteLine($"✅ {Name} seeded successfully");
    }
  }
}
