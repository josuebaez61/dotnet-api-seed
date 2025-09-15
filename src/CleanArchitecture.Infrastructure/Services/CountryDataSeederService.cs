using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services
{
    public class CountryDataSeederService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CountryDataSeederService> _logger;

        public CountryDataSeederService(ApplicationDbContext context, ILogger<CountryDataSeederService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedCountriesAndStatesAsync()
        {
            try
            {
                _logger.LogInformation("🌍 Starting to seed countries and states data...");

                // Check if countries already exist
                if (await _context.Countries.AnyAsync())
                {
                    _logger.LogInformation("✅ Countries data already exists, skipping seeding");
                    return;
                }

                // Load data from SQL files
                await LoadCountriesFromSqlFile();
                await LoadStatesFromSqlFile();

                _logger.LogInformation("🌍 Successfully seeded countries and states data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error seeding countries and states data");
                throw;
            }
        }

        private async Task LoadCountriesFromSqlFile()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.countries.sql";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    _logger.LogWarning("⚠️ Countries SQL file not found as embedded resource: {ResourceName}", resourceName);
                    return;
                }

                using var reader = new StreamReader(stream);
                var sqlContent = await reader.ReadToEndAsync();

                // Execute SQL directly - PostgreSQL format
                await _context.Database.ExecuteSqlRawAsync(sqlContent);
                _logger.LogInformation("✅ Countries data loaded from PostgreSQL SQL file");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading countries from SQL file");
                throw;
            }
        }

        private async Task LoadStatesFromSqlFile()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.states.sql";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    _logger.LogWarning("⚠️ States SQL file not found as embedded resource: {ResourceName}", resourceName);
                    return;
                }

                using var reader = new StreamReader(stream);
                var sqlContent = await reader.ReadToEndAsync();

                // Execute SQL directly - PostgreSQL format
                await _context.Database.ExecuteSqlRawAsync(sqlContent);
                _logger.LogInformation("✅ States data loaded from PostgreSQL SQL file");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading states from SQL file");
                throw;
            }
        }
    }
}
