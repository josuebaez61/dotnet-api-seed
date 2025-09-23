using System;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Infrastructure.Services.Seeders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// CLI tool for running seeders manually
  /// </summary>
  public class SeederRunner
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SeederRunner> _logger;

    public SeederRunner(IServiceProvider serviceProvider, ILogger<SeederRunner> logger)
    {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }

    /// <summary>
    /// Runs all seeders in the correct order
    /// </summary>
    public async Task RunAllSeedersAsync()
    {
      _logger.LogInformation("🌱 Starting to run all seeders...");

      var seeders = _serviceProvider.GetServices<ISeeder>().ToList();

      // Define the order of seeders
      var orderedSeeders = new[]
      {
        "Roles",           // Must be first
        "Permissions",     // Must be second
        "RolePermissions", // Must be third (depends on Roles and Permissions)
        "AdminUser",       // Must be fourth (depends on Roles)
        "FakeUsers",       // Must be fifth (depends on Roles)
        "Countries",       // Geographic data - countries first
        "Cities"           // Geographic data - cities depend on countries
      };

      foreach (var seederName in orderedSeeders)
      {
        var seeder = seeders.FirstOrDefault(s => s.Name == seederName);
        if (seeder != null)
        {
          try
          {
            _logger.LogInformation($"🌱 Running seeder: {seeder.Name}");
            await seeder.SeedAsync();
            _logger.LogInformation($"✅ Seeder {seeder.Name} completed successfully");
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"❌ Error running seeder {seeder.Name}");
            throw;
          }
        }
        else
        {
          _logger.LogWarning($"⚠️ Seeder '{seederName}' not found");
        }
      }

      _logger.LogInformation("✅ All seeders completed successfully");
    }

    /// <summary>
    /// Runs a specific seeder by name
    /// </summary>
    /// <param name="seederName">Name of the seeder to run</param>
    public async Task RunSeederAsync(string seederName)
    {
      var seeders = _serviceProvider.GetServices<ISeeder>().ToList();
      var seeder = seeders.FirstOrDefault(s => s.Name.Equals(seederName, StringComparison.OrdinalIgnoreCase));

      if (seeder != null)
      {
        try
        {
          _logger.LogInformation($"🌱 Running seeder: {seeder.Name}");
          await seeder.SeedAsync();
          _logger.LogInformation($"✅ Seeder {seeder.Name} completed successfully");
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"❌ Error running seeder {seeder.Name}");
          throw;
        }
      }
      else
      {
        var availableSeeders = string.Join(", ", seeders.Select(s => s.Name));
        _logger.LogError($"❌ Seeder '{seederName}' not found. Available seeders: {availableSeeders}");
        throw new ArgumentException($"Seeder '{seederName}' not found. Available seeders: {availableSeeders}");
      }
    }

    /// <summary>
    /// Lists all available seeders
    /// </summary>
    public void ListSeeders()
    {
      var seeders = _serviceProvider.GetServices<ISeeder>().ToList();
      _logger.LogInformation("📋 Available seeders:");
      foreach (var seeder in seeders)
      {
        _logger.LogInformation($"  - {seeder.Name}");
      }
    }
  }
}
