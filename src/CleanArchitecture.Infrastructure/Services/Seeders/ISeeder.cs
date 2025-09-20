using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Base interface for all seeders
  /// </summary>
  public interface ISeeder
  {
    /// <summary>
    /// Executes the seeding operation
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task SeedAsync();

    /// <summary>
    /// Gets the name of the seeder for logging purposes
    /// </summary>
    string Name { get; }
  }
}
