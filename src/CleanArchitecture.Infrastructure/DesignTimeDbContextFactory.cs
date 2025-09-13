using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Data
{
  public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
  {
    public ApplicationDbContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

      // Build configuration from appsettings files
      // This replicates the same configuration logic as Program.cs
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

      var configuration = new ConfigurationBuilder()
          .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "CleanArchitecture.API"))
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .Build();

      // Get connection string using the same key as Program.cs
      var connectionString = configuration.GetConnectionString("DefaultConnection");
      optionsBuilder.UseNpgsql(connectionString);

      return new ApplicationDbContext(optionsBuilder.Options);
    }
  }
}
