using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Seeder for creating fake users using Bogus library
  /// </summary>
  public class FakeUserSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly Faker<User> _userFaker;

    public string Name => "FakeUsers";

    public FakeUserSeeder(ApplicationDbContext context, UserManager<User> userManager)
    {
      _context = context;
      _userManager = userManager;

      // Configure Bogus to generate fake users
      _userFaker = new Faker<User>("en")
        .RuleFor(u => u.Id, f => Guid.NewGuid())
        .RuleFor(u => u.UserName, f => f.Internet.UserName())
        .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName?.ToUpper())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpper())
        .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool(0.8f)) // 80% confirmed
        .RuleFor(u => u.FirstName, f => f.Name.FirstName())
        .RuleFor(u => u.LastName, f => f.Name.LastName())
        .RuleFor(u => u.DateOfBirth, f => f.Date.Between(DateTime.UtcNow.AddYears(-80), DateTime.UtcNow.AddYears(-18)))
        .RuleFor(u => u.ProfilePicture, f => f.Random.Bool(0.3f) ? f.Internet.Avatar() : null) // 30% have profile pictures
        .RuleFor(u => u.IsActive, f => f.Random.Bool(0.9f)) // 90% active
        .RuleFor(u => u.MustChangePassword, f => false)
        .RuleFor(u => u.CreatedAt, f => f.Date.Between(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow))
        .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt.AddDays(f.Random.Int(0, 365)));
    }

    public async Task SeedAsync()
    {
      Console.WriteLine($"🌱 Seeding {Name}...");

      // Check if we already have fake users (more than just admin)
      var existingUserCount = await _userManager.Users.CountAsync();
      if (existingUserCount > 1) // More than just admin user
      {
        Console.WriteLine($"✅ {Name} already seeded ({existingUserCount} users exist), skipping...");
        return;
      }

      // Get available roles for assignment
      var roles = await _context.Roles.ToListAsync();
      if (!roles.Any())
      {
        Console.WriteLine($"❌ No roles found. Please run role seeders first.");
        return;
      }

      // Generate 50 fake users
      var fakeUsers = _userFaker.Generate(50);
      var successCount = 0;
      var errorCount = 0;

      foreach (var user in fakeUsers)
      {
        try
        {
          // Create user with a standard password
          var result = await _userManager.CreateAsync(user, "FakeUser123!");

          if (result.Succeeded)
          {
            // Assign random role(s) to user
            await AssignRandomRolesAsync(user, roles);
            successCount++;
          }
          else
          {
            Console.WriteLine($"⚠️ Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            errorCount++;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"❌ Error creating user {user.UserName}: {ex.Message}");
          errorCount++;
        }
      }

      Console.WriteLine($"✅ {Name} seeding completed: {successCount} users created, {errorCount} errors");
    }

    private async Task AssignRandomRolesAsync(User user, List<Role> roles)
    {
      try
      {
        // 70% chance of having at least one role
        if (!new Faker().Random.Bool(0.7f))
          return;

        // Assign 1-3 random roles
        var rolesToAssign = new Faker().PickRandom(roles, new Faker().Random.Int(1, Math.Min(3, roles.Count)));

        foreach (var role in rolesToAssign)
        {
          try
          {
            await _userManager.AddToRoleAsync(user, role.Name!);
          }
          catch (Exception ex)
          {
            Console.WriteLine($"⚠️ Failed to assign role {role.Name} to user {user.UserName}: {ex.Message}");
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"❌ Error assigning roles to user {user.UserName}: {ex.Message}");
      }
    }
  }
}
