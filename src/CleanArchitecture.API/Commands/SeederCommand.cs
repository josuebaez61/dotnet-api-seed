using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
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

      // All seeders command
      var allCommand = new Command("all", "Run all seeders in the correct order");
      allCommand.SetHandler(RunAllSeedersAsync);
      rootCommand.AddCommand(allCommand);

      // Specific seeder command
      var seederCommand = new Command("run", "Run a specific seeder");
      var seederNameArgument = new Argument<string>("name", "Name of the seeder to run");
      seederCommand.AddArgument(seederNameArgument);
      seederCommand.SetHandler(RunSpecificSeederAsync, seederNameArgument);
      rootCommand.AddCommand(seederCommand);

      // List seeders command
      var listCommand = new Command("list", "List all available seeders");
      listCommand.SetHandler(ListSeeders);
      rootCommand.AddCommand(listCommand);

      // Truncate command
      var truncateCommand = new Command("truncate", "Truncate all database tables (DEV ONLY)");
      truncateCommand.SetHandler(TruncateAllTablesAsync);
      rootCommand.AddCommand(truncateCommand);

      return rootCommand;
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
        // Check if we're in development environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Development")
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
