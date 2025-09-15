using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Data.Seeds.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CleanArchitecture.Infrastructure.Services
{
    public class CityDataSeederService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CityDataSeederService> _logger;

        public CityDataSeederService(ApplicationDbContext context, ILogger<CityDataSeederService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedCitiesAsync()
        {
            try
            {
                _logger.LogInformation("🏙️ Starting to seed cities data from ArgentinaCities.json...");

                // Check if cities already exist
                if (await _context.Cities.AnyAsync())
                {
                    _logger.LogInformation("✅ Cities data already exists, skipping seeding");
                    return;
                }

                // Load and process cities data
                var cities = await LoadCitiesFromJsonFile();
                if (cities.Count == 0)
                {
                    _logger.LogWarning("⚠️ No cities found in JSON file");
                    return;
                }

                _logger.LogInformation("📊 Found {CityCount} cities in JSON file", cities.Count);

                // Get all states for matching
                var states = await _context.States.ToListAsync();
                _logger.LogInformation("🗺️ Found {StateCount} states in database", states.Count);

                // Process and insert cities
                await ProcessAndInsertCities(cities, states);

                _logger.LogInformation("🏙️ Successfully seeded cities data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error seeding cities data");
                throw;
            }
        }

        private async Task<List<ArgentinaCityDto>> LoadCitiesFromJsonFile()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.ArgentinaCities.json";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    _logger.LogWarning("⚠️ ArgentinaCities.json file not found as embedded resource: {ResourceName}", resourceName);
                    return new List<ArgentinaCityDto>();
                }

                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cities = JsonSerializer.Deserialize<List<ArgentinaCityDto>>(jsonContent, options);
                return cities ?? new List<ArgentinaCityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading cities from JSON file");
                throw;
            }
        }

        private async Task ProcessAndInsertCities(List<ArgentinaCityDto> argentinaCities, List<State> states)
        {
            var citiesToInsert = new List<City>();
            var matchedCount = 0;
            var unmatchedCount = 0;

            foreach (var argentinaCity in argentinaCities)
            {
                // Find matching state by province name
                var matchingState = FindMatchingState(argentinaCity.Provincia.Nombre, states);
                
                if (matchingState != null)
                {
                    var city = new City
                    {
                        Id = Guid.NewGuid(),
                        Name = argentinaCity.Nombre,
                        StateId = matchingState.Id, // This will be int from existing database
                        Code = argentinaCity.Id, // Use the original ID as code
                        Type = argentinaCity.Categoria,
                        Latitude = (decimal?)argentinaCity.Centroide.Lat,
                        Longitude = (decimal?)argentinaCity.Centroide.Lon,
                        Timezone = "America/Argentina/Buenos_Aires", // Default timezone for Argentina
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    citiesToInsert.Add(city);
                    matchedCount++;
                }
                else
                {
                    unmatchedCount++;
                    _logger.LogWarning("⚠️ Could not find matching state for province: {ProvinceName}", argentinaCity.Provincia.Nombre);
                }

                // Insert in batches to avoid memory issues
                if (citiesToInsert.Count >= 1000)
                {
                    await InsertCitiesBatch(citiesToInsert);
                    citiesToInsert.Clear();
                }
            }

            // Insert remaining cities
            if (citiesToInsert.Count > 0)
            {
                await InsertCitiesBatch(citiesToInsert);
            }

            _logger.LogInformation("✅ Cities processing completed: {MatchedCount} matched, {UnmatchedCount} unmatched", 
                matchedCount, unmatchedCount);
        }

        private State? FindMatchingState(string provinceName, List<State> states)
        {
            // Try exact match first
            var exactMatch = states.FirstOrDefault(s => 
                string.Equals(s.Name, provinceName, StringComparison.OrdinalIgnoreCase));
            
            if (exactMatch != null)
                return exactMatch;

            // Try partial match for common variations
            var partialMatch = states.FirstOrDefault(s => 
                s.Name.Contains(provinceName, StringComparison.OrdinalIgnoreCase) ||
                provinceName.Contains(s.Name, StringComparison.OrdinalIgnoreCase));

            return partialMatch;
        }

        private async Task InsertCitiesBatch(List<City> cities)
        {
            try
            {
                await _context.Cities.AddRangeAsync(cities);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Inserted batch of {Count} cities", cities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inserting cities batch");
                throw;
            }
        }
    }
}
