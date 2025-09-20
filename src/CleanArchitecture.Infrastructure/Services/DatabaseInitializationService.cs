using System;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Common.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services
{
  public class DatabaseInitializationService
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly CountryDataSeederService _countryDataSeeder;
    private readonly CityDataSeederService _cityDataSeeder;

    // Static GUIDs for consistent seeding
    private static readonly Guid AdminRoleId = new Guid("11734856-f5ac-4ee1-b28d-e35ef9fc0d4f");
    private static readonly Guid UserRoleId = new Guid("c894dc98-510d-4a96-8e90-aecf829d2a7c");
    private static readonly Guid AdminUserId = new Guid("f9e8d7c6-b5a4-3210-9876-543210fedcba");

    // New Permission GUIDs
    private static readonly Guid ManageRolesPermissionId = new Guid("32edea54-6b49-4f4f-8257-aa1992f23c28");
    private static readonly Guid ManageUsersPermissionId = new Guid("e1d015ea-0d8a-42b5-a0c1-237a8e018999");
    private static readonly Guid ManageUserRolesPermissionId = new Guid("3c883108-b93d-4142-acc8-bbd67f694fb1");
    private static readonly Guid ManageRolePermissionsPermissionId = new Guid("02033fae-fccd-4a7f-8cea-06a43178ec73");
    private static readonly Guid AdminPermissionId = new Guid("082a40e0-2ff4-4c05-a078-4dfaf778172f");
    private static readonly Guid SuperAdminPermissionId = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    // Legacy Permission GUIDs (for backward compatibility)
    private static readonly Guid UserReadPermissionId = new Guid("b2c3d4e5-f6a7-8901-bcde-f23456789012");
    private static readonly Guid UserWritePermissionId = new Guid("c3d4e5f6-a7b8-9012-cdef-345678901234");
    private static readonly Guid UserDeletePermissionId = new Guid("d4e5f6a7-b8c9-0123-def4-456789012345");
    private static readonly Guid RoleReadPermissionId = new Guid("e5f6a7b8-c9d0-1234-ef56-567890123456");
    private static readonly Guid RoleWritePermissionId = new Guid("f6a7b8c9-d0e1-2345-f678-678901234567");
    private static readonly Guid PermissionReadPermissionId = new Guid("a7b8c9d0-e1f2-3456-789a-789012345678");
    private static readonly Guid PermissionWritePermissionId = new Guid("b8c9d0e1-f2a3-4567-89ab-890123456789");

    // New RolePermission GUIDs
    private static readonly Guid AdminManageRolesPermissionId = new Guid("54135e51-88ee-490b-92b6-31fc3802db45");
    private static readonly Guid AdminManageUsersPermissionId = new Guid("8fccec4a-199d-4e4d-8f33-659d7ea41d8c");
    private static readonly Guid AdminManageUserRolesPermissionId = new Guid("94ef9ec0-390e-4e4f-8da4-80a49ea997b6");
    private static readonly Guid AdminManageRolePermissionsPermissionId = new Guid("b4c4f525-866d-468f-b474-e7a0ffc8e53f");
    private static readonly Guid AdminAdminPermissionId = new Guid("ea00cad1-99a9-415c-b8b8-4bf5efd0f81b");
    private static readonly Guid AdminSuperAdminPermissionId = new Guid("f1a2b3c4-d5e6-7890-abcd-ef1234567890");

    // Legacy RolePermission GUIDs (for backward compatibility)
    private static readonly Guid AdminUserReadPermissionId = new Guid("a2b3c4d5-e6f7-8901-bcde-f23456789012");
    private static readonly Guid AdminUserWritePermissionId = new Guid("b3c4d5e6-f7a8-9012-cdef-345678901234");
    private static readonly Guid AdminUserDeletePermissionId = new Guid("c4d5e6f7-a8b9-0123-def4-456789012345");
    private static readonly Guid AdminRoleReadPermissionId = new Guid("d5e6f7a8-b9c0-1234-ef56-567890123456");
    private static readonly Guid AdminRoleWritePermissionId = new Guid("e6f7a8b9-c0d1-2345-f678-678901234567");
    private static readonly Guid AdminPermissionReadPermissionId = new Guid("f7a8b9c0-d1e2-3456-789a-789012345678");
    private static readonly Guid AdminPermissionWritePermissionId = new Guid("a8b9c0d1-e2f3-4567-89ab-890123456789");
    private static readonly Guid UserUserReadPermissionId = new Guid("b9c0d1e2-f3a4-5678-9abc-901234567890");

    private static readonly DateTime SeedTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public DatabaseInitializationService(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        CountryDataSeederService countryDataSeeder,
        CityDataSeederService cityDataSeeder)
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
      _countryDataSeeder = countryDataSeeder;
      _cityDataSeeder = cityDataSeeder;
    }

    public async Task InitializeAsync()
    {
      Console.WriteLine("🔍 DEBUG: Starting database initialization...");

      // Ensure database is created
      await _context.Database.EnsureCreatedAsync();
      Console.WriteLine("🔍 DEBUG: Database ensured created");

      // Seed roles
      Console.WriteLine("🔍 DEBUG: Starting to seed roles...");
      await SeedRolesAsync();

      // Seed permissions
      await SeedPermissionsAsync();

      // Seed role permissions
      await SeedRolePermissionsAsync();

      // Seed countries and states
      await _countryDataSeeder.SeedCountriesAndStatesAsync();

      // Seed cities
      await _cityDataSeeder.SeedCitiesAsync();

      // Create admin user if it doesn't exist
      await CreateAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
      // Check if roles already exist
      if (await _context.Roles.AnyAsync())
      {
        return; // Roles already seeded
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
    }

    private async Task SeedPermissionsAsync()
    {
      // Check if permissions already exist
      if (await _context.Permissions.AnyAsync())
      {
        return; // Permissions already seeded
      }

      var newPermissions = new[]
      {
        new Permission
        {
          Id = ManageRolesPermissionId,
          Name = PermissionConstants.ManageRoles,
          Description = "Manage roles (create, update, delete, read)",
          Resource = "Roles",
          Action = "Manage",
          Module = "RoleManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = ManageUsersPermissionId,
          Name = PermissionConstants.ManageUsers,
          Description = "Manage users (create, update, delete, read)",
          Resource = "Users",
          Action = "Manage",
          Module = "UserManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = ManageUserRolesPermissionId,
          Name = PermissionConstants.ManageUserRoles,
          Description = "Manage user-role assignments",
          Resource = "UserRoles",
          Action = "Manage",
          Module = "UserManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = ManageRolePermissionsPermissionId,
          Name = PermissionConstants.ManageRolePermissions,
          Description = "Manage role-permission assignments",
          Resource = "RolePermissions",
          Action = "Manage",
          Module = "PermissionManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = AdminPermissionId,
          Name = PermissionConstants.Admin,
          Description = "Administrative access",
          Resource = "System",
          Action = "Admin",
          Module = "System",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = SuperAdminPermissionId,
          Name = PermissionConstants.SuperAdmin,
          Description = "Super administrative access (includes all permissions)",
          Resource = "System",
          Action = "SuperAdmin",
          Module = "System",
          CreatedAt = SeedTimestamp
        }
      };

      var legacyPermissions = new[]
      {
        new Permission
        {
          Id = UserReadPermissionId,
          Name = "Users.Read", // Legacy permission
          Description = "Read access to users (Legacy)",
          Resource = "Users",
          Action = "Read",
          Module = "UserManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = UserWritePermissionId,
          Name = "Users.Write", // Legacy permission
          Description = "Write access to users (Legacy)",
          Resource = "Users",
          Action = "Write",
          Module = "UserManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = UserDeletePermissionId,
          Name = "Users.Delete", // Legacy permission
          Description = "Delete access to users (Legacy)",
          Resource = "Users",
          Action = "Delete",
          Module = "UserManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = RoleReadPermissionId,
          Name = "Roles.Read", // Legacy permission
          Description = "Read access to roles (Legacy)",
          Resource = "Roles",
          Action = "Read",
          Module = "RoleManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = RoleWritePermissionId,
          Name = "Roles.Write", // Legacy permission
          Description = "Write access to roles (Legacy)",
          Resource = "Roles",
          Action = "Write",
          Module = "RoleManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = PermissionReadPermissionId,
          Name = "Permissions.Read", // Legacy permission
          Description = "Read access to permissions (Legacy)",
          Resource = "Permissions",
          Action = "Read",
          Module = "PermissionManagement",
          CreatedAt = SeedTimestamp
        },
        new Permission
        {
          Id = PermissionWritePermissionId,
          Name = "Permissions.Write", // Legacy permission
          Description = "Write access to permissions (Legacy)",
          Resource = "Permissions",
          Action = "Write",
          Module = "PermissionManagement",
          CreatedAt = SeedTimestamp
        }
      };

      // Add both new and legacy permissions
      await _context.Permissions.AddRangeAsync(newPermissions);
      await _context.Permissions.AddRangeAsync(legacyPermissions);
      await _context.SaveChangesAsync();
    }

    private async Task SeedRolePermissionsAsync()
    {
      // Check if role permissions already exist
      if (await _context.RolePermissions.AnyAsync())
      {
        return; // Role permissions already seeded
      }

      var newRolePermissions = new[]
      {
        // Admin role gets all new permissions
        new RolePermission
        {
          Id = AdminManageRolesPermissionId,
          RoleId = AdminRoleId,
          PermissionId = ManageRolesPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminManageUsersPermissionId,
          RoleId = AdminRoleId,
          PermissionId = ManageUsersPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminManageUserRolesPermissionId,
          RoleId = AdminRoleId,
          PermissionId = ManageUserRolesPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminManageRolePermissionsPermissionId,
          RoleId = AdminRoleId,
          PermissionId = ManageRolePermissionsPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminAdminPermissionId,
          RoleId = AdminRoleId,
          PermissionId = AdminPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminSuperAdminPermissionId,
          RoleId = AdminRoleId,
          PermissionId = SuperAdminPermissionId,
          CreatedAt = SeedTimestamp
        }
      };

      var legacyRolePermissions = new[]
      {
        // Legacy permissions for backward compatibility
        new RolePermission
        {
          Id = AdminUserReadPermissionId,
          RoleId = AdminRoleId,
          PermissionId = UserReadPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminUserWritePermissionId,
          RoleId = AdminRoleId,
          PermissionId = UserWritePermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminUserDeletePermissionId,
          RoleId = AdminRoleId,
          PermissionId = UserDeletePermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminRoleReadPermissionId,
          RoleId = AdminRoleId,
          PermissionId = RoleReadPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminRoleWritePermissionId,
          RoleId = AdminRoleId,
          PermissionId = RoleWritePermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminPermissionReadPermissionId,
          RoleId = AdminRoleId,
          PermissionId = PermissionReadPermissionId,
          CreatedAt = SeedTimestamp
        },
        new RolePermission
        {
          Id = AdminPermissionWritePermissionId,
          RoleId = AdminRoleId,
          PermissionId = PermissionWritePermissionId,
          CreatedAt = SeedTimestamp
        },
        // User role gets only read permissions (legacy)
        new RolePermission
        {
          Id = UserUserReadPermissionId,
          RoleId = UserRoleId,
          PermissionId = UserReadPermissionId,
          CreatedAt = SeedTimestamp
        }
      };

      // Add both new and legacy role permissions
      await _context.RolePermissions.AddRangeAsync(newRolePermissions);
      await _context.RolePermissions.AddRangeAsync(legacyRolePermissions);
      await _context.SaveChangesAsync();
    }

    private async Task CreateAdminUserAsync()
    {
      // Check if admin user already exists
      var adminUser = await _userManager.FindByNameAsync("admin");
      if (adminUser != null)
      {
        return; // Admin user already exists
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
        var adminRole = await _roleManager.FindByNameAsync("Admin");
        if (adminRole != null)
        {
          // Assign admin role to user
          await _userManager.AddToRoleAsync(user, "Admin");
        }
      }
    }
  }
}