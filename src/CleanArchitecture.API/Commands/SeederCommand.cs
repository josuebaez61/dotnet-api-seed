using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.API.Common;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Services.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.API.Commands
{
  /// <summary>
  /// CLI command for running seeders
  /// </summary>
  public class SeederCommand
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SeederCommand> _logger;

    public SeederCommand(IServiceProvider serviceProvider, ILogger<SeederCommand> logger)
    {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }

    /// <summary>
    /// Creates the root command for seeding operations
    /// </summary>
    public RootCommand CreateCommand()
    {
      var rootCommand = new RootCommand("Database seeding commands");

      // Seed option - can run all seeders or a specific one
      var seedOption = new Option<string?>(
        aliases: new[] { "--seed", "-s" },
        description: "Run seeders. Use 'all' to run all seeders, or specify a seeder name (e.g., 'AdminUser', 'Roles', 'Countries')")
      {
        ArgumentHelpName = "seeder-name"
      };
      seedOption.AddValidator(result =>
      {
        var value = result.GetValueForOption(seedOption);
        if (string.IsNullOrEmpty(value))
        {
          result.ErrorMessage = "Seeder name is required. Use 'all' to run all seeders or specify a specific seeder name.";
        }
      });
      rootCommand.AddOption(seedOption);

      // List seeders command
      var listCommand = new Command("list", "List all available seeders");
      listCommand.SetHandler(ListSeeders);
      rootCommand.AddCommand(listCommand);

      // Truncate command
      var truncateCommand = new Command("truncate", "Truncate all database tables (DEV ONLY)");
      truncateCommand.SetHandler(TruncateAllTablesAsync);
      rootCommand.AddCommand(truncateCommand);

      // Set handler for seed option
      rootCommand.SetHandler(RunSeedersAsync, seedOption);

      return rootCommand;
    }

    /// <summary>
    /// Handles the --seed option
    /// </summary>
    private async Task RunSeedersAsync(string? seederName)
    {
      if (string.IsNullOrEmpty(seederName))
      {
        _logger.LogError("❌ Seeder name is required. Use 'all' to run all seeders or specify a specific seeder name.");
        return;
      }

      // Validate environment first
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      try
      {
        EnvironmentConstants.ValidateEnvironment(environment);
        _logger.LogInformation("✅ Environment validation passed: {Environment}", environment);
      }
      catch (InvalidOperationException ex)
      {
        _logger.LogError("❌ {ErrorMessage}", ex.Message);
        _logger.LogError("🔧 Current environment: '{Environment}'", environment);
        _logger.LogError("💡 Please set ASPNETCORE_ENVIRONMENT to one of: {AllowedEnvironments}",
            EnvironmentConstants.GetAllowedEnvironmentsString());
        throw;
      }

      // Check if running in Production environment
      if (string.Equals(environment, EnvironmentConstants.Production, StringComparison.OrdinalIgnoreCase))
      {
        _logger.LogWarning("⚠️  WARNING: You are about to run seeders in PRODUCTION environment!");
        _logger.LogWarning("⚠️  This action will modify the production database.");
        _logger.LogWarning("⚠️  Seeder: {SeederName}", seederName);
        _logger.LogWarning("⚠️  Environment: {Environment}", environment);

        Console.WriteLine();
        Console.WriteLine("🚨 PRODUCTION ENVIRONMENT DETECTED 🚨");
        Console.WriteLine($"You are about to run seeder: {seederName}");
        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine();
        Console.WriteLine("This action will modify the PRODUCTION database!");
        Console.WriteLine("Type 'CONFIRM' (in uppercase) to proceed, or anything else to cancel:");

        var confirmation = Console.ReadLine();
        if (!string.Equals(confirmation, "CONFIRM", StringComparison.OrdinalIgnoreCase))
        {
          _logger.LogInformation("❌ Operation cancelled by user");
          Console.WriteLine("❌ Operation cancelled. No changes made to the database.");
          return;
        }

        _logger.LogWarning("✅ User confirmed operation in Production environment");
        Console.WriteLine("✅ Confirmed. Proceeding with seeding in Production...");
        Console.WriteLine();
      }

      if (seederName.Equals("all", StringComparison.OrdinalIgnoreCase))
      {
        await RunAllSeedersAsync();
      }
      else
      {
        await RunSpecificSeederAsync(seederName);
      }
    }

    private async Task RunAllSeedersAsync()
    {
      try
      {
        var seederRunner = _serviceProvider.GetRequiredService<SeederRunner>();
        await seederRunner.RunAllSeedersAsync();
        _logger.LogInformation("✅ All seeders completed successfully");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error running seeders");
        throw;
      }
    }

    private async Task RunSpecificSeederAsync(string seederName)
    {
      try
      {
        var seederRunner = _serviceProvider.GetRequiredService<SeederRunner>();
        await seederRunner.RunSeederAsync(seederName);
        _logger.LogInformation($"✅ Seeder '{seederName}' completed successfully");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"❌ Error running seeder '{seederName}'");
        throw;
      }
    }

    private void ListSeeders()
    {
      try
      {
        var seederRunner = _serviceProvider.GetRequiredService<SeederRunner>();
        seederRunner.ListSeeders();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error listing seeders");
        throw;
      }
    }

    private async Task TruncateAllTablesAsync()
    {
      try
      {
        // Validate environment first
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
          EnvironmentConstants.ValidateEnvironment(environment);
          _logger.LogInformation("✅ Environment validation passed: {Environment}", environment);
        }
        catch (InvalidOperationException ex)
        {
          _logger.LogError("❌ {ErrorMessage}", ex.Message);
          _logger.LogError("🔧 Current environment: '{Environment}'", environment);
          _logger.LogError("💡 Please set ASPNETCORE_ENVIRONMENT to one of: {AllowedEnvironments}",
              EnvironmentConstants.GetAllowedEnvironmentsString());
          throw;
        }

        // Check if we're in development environment
        if (environment != EnvironmentConstants.Development)
        {
          _logger.LogError("❌ This command can only be run in Development environment!");
          _logger.LogError($"Current environment: {environment ?? "Not set"}");
          _logger.LogError("Please set ASPNETCORE_ENVIRONMENT=Development");
          throw new InvalidOperationException("Truncate command can only be run in Development environment");
        }

        _logger.LogWarning("⚠️ WARNING: This will delete ALL data from ALL tables!");
        _logger.LogWarning("⚠️ This command should ONLY be used in Development environment!");
        _logger.LogInformation("🔄 Starting table truncation...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get all table names
        var tableNames = await GetAllTableNamesAsync(context);

        if (tableNames.Count == 0)
        {
          _logger.LogInformation("ℹ️ No tables found to truncate");
          return;
        }

        _logger.LogInformation($"📋 Found {tableNames.Count} tables to truncate:");
        foreach (var tableName in tableNames)
        {
          _logger.LogInformation($"  - {tableName}");
        }

        // Disable foreign key checks temporarily
        await context.Database.ExecuteSqlRawAsync("SET session_replication_role = replica;");

        // Delete all data from each table (maintaining table structure)
        foreach (var tableName in tableNames)
        {
          try
          {
            _logger.LogInformation($"🗑️ Clearing table: {tableName}");
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\";");
            _logger.LogInformation($"✅ Successfully cleared: {tableName}");
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"❌ Failed to clear table {tableName}: {ex.Message}");
            // Continue with other tables even if one fails
          }
        }

        // Re-enable foreign key checks
        await context.Database.ExecuteSqlRawAsync("SET session_replication_role = DEFAULT;");

        _logger.LogInformation("✅ All tables cleared successfully!");
        _logger.LogInformation("💡 You can now run seeders to populate the database again");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error during table truncation");
        throw;
      }
    }

    private async Task<List<string>> GetAllTableNamesAsync(ApplicationDbContext context)
    {
      var tableNames = new List<string>();

      try
      {
        // Get all table names from the database
        var sql = @"
          SELECT table_name 
          FROM information_schema.tables 
          WHERE table_schema = 'public' 
          AND table_type = 'BASE TABLE'
          AND table_name NOT LIKE 'pg_%'
          AND table_name NOT LIKE '__EFMigrationsHistory%'
          ORDER BY table_name";

        var result = await context.Database.SqlQueryRaw<string>(sql).ToListAsync();
        tableNames.AddRange(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error getting table names from database");
        throw;
      }

      return tableNames;
    }
  }
}
