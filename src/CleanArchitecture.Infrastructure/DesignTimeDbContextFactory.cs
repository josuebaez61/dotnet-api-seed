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

      // Use a simple connection string for design time
      optionsBuilder.UseNpgsql("Host=localhost;Database=CleanArchitectureDB;Username=postgres;Password=postgres");

      return new ApplicationDbContext(optionsBuilder.Options);
    }
  }
}
