using System.Reflection;
using System.Text.Json;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Data.Seeds.Models;
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
                _logger.LogInformation("🌍 Starting to seed countries and states data from JSON files...");

                // Check if countries already exist
                if (await _context.Countries.AnyAsync())
                {
                    _logger.LogInformation("✅ Countries data already exists, skipping seeding");
                    return;
                }

                // Load data from JSON files
                await LoadCountriesFromJsonFile();
                await LoadStatesFromJsonFile();

                _logger.LogInformation("🌍 Successfully seeded countries and states data from JSON files");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error seeding countries and states data from JSON files");
                throw;
            }
        }

        private async Task LoadCountriesFromJsonFile()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.Countries.json";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    _logger.LogError("❌ Countries.json resource not found: {ResourceName}", resourceName);
                    throw new FileNotFoundException($"Countries.json resource not found: {resourceName}");
                }

                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                var countryDtos = JsonSerializer.Deserialize<List<CountryJsonDto>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (countryDtos == null || !countryDtos.Any())
                {
                    _logger.LogWarning("⚠️ No countries found in JSON file");
                    return;
                }

                var countries = countryDtos.Select(MapToCountry).ToList();
                await InsertCountriesBatch(countries);

                _logger.LogInformation("✅ Countries data loaded successfully from JSON: {Count} countries", countries.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading countries data from JSON file");
                throw;
            }
        }

        private Country MapToCountry(CountryJsonDto dto)
        {
            return new Country
            {
                Id = dto.Id,
                Name = dto.Name,
                Iso3 = dto.Iso3,
                Iso2 = dto.Iso2,
                NumericCode = dto.NumericCode,
                Phonecode = dto.Phonecode,
                Capital = dto.Capital,
                Currency = dto.Currency,
                CurrencyName = dto.CurrencyName,
                CurrencySymbol = dto.CurrencySymbol,
                Tld = dto.Tld,
                Native = dto.Native,
                Nationality = dto.Nationality,
                Timezones = dto.Timezones != null ? JsonSerializer.Serialize(dto.Timezones) : null,
                Translations = dto.Translations != null ? JsonSerializer.Serialize(dto.Translations) : null,
                Latitude = ParseDecimal(dto.Latitude),
                Longitude = ParseDecimal(dto.Longitude),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Flag = true
                // Note: We're omitting region_id, subregion_id, wikiDataId, and emoji as requested
            };
        }

        private async Task LoadStatesFromJsonFile()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.States.json";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    _logger.LogError("❌ States.json resource not found: {ResourceName}", resourceName);
                    throw new FileNotFoundException($"States.json resource not found: {resourceName}");
                }

                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                var stateDtos = JsonSerializer.Deserialize<List<StateJsonDto>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (stateDtos == null || !stateDtos.Any())
                {
                    _logger.LogWarning("⚠️ No states found in JSON file");
                    return;
                }

                // Get countries to validate country_id references
                var countries = await _context.Countries.ToListAsync();
                var countryIdSet = countries.Select(c => c.Id).ToHashSet();

                var states = stateDtos
                    .Where(s => countryIdSet.Contains(s.CountryId))
                    .Select(s => MapToState(s, s.CountryId))
                    .ToList();

                await InsertStatesBatch(states);

                _logger.LogInformation("✅ States data loaded successfully from JSON: {Count} states", states.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading states data from JSON file");
                throw;
            }
        }

        private State MapToState(StateJsonDto dto, int countryId)
        {
            return new State
            {
                Id = dto.Id,
                Name = dto.Name,
                CountryId = dto.CountryId,
                CountryCode = dto.CountryCode ?? string.Empty,
                FipsCode = dto.FipsCode,
                Iso2 = dto.Iso2,
                Iso31662 = dto.Iso31662,
                Type = dto.Type,
                Level = ParseInt(dto.Level),
                ParentId = !string.IsNullOrEmpty(dto.ParentId) && dto.ParentId != "null" ? int.Parse(dto.ParentId) : null,
                Latitude = ParseDecimal(dto.Latitude),
                Longitude = ParseDecimal(dto.Longitude),
                Timezone = dto.Timezone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Flag = true
            };
        }

        private string? ParseString(string? value)
        {
            if (string.IsNullOrEmpty(value) || value == "NULL") return null;
            return value.Trim('\'', '"');
        }

        private int? ParseInt(string? value)
        {
            if (string.IsNullOrEmpty(value) || value == "NULL") return null;
            if (int.TryParse(value, out var result)) return result;
            return null;
        }

        private decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrEmpty(value) || value == "NULL") return null;
            if (decimal.TryParse(value, out var result)) return result;
            return null;
        }


        private async Task InsertCountriesBatch(List<Country> countries)
        {
            const int batchSize = 100;
            for (int i = 0; i < countries.Count; i += batchSize)
            {
                var batch = countries.Skip(i).Take(batchSize).ToList();
                await _context.Countries.AddRangeAsync(batch);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Inserted countries batch: {Count}/{Total}", batch.Count, countries.Count);
            }
        }

        private async Task InsertStatesBatch(List<State> states)
        {
            const int batchSize = 100;
            for (int i = 0; i < states.Count; i += batchSize)
            {
                var batch = states.Skip(i).Take(batchSize).ToList();
                await _context.States.AddRangeAsync(batch);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Inserted states batch: {Count}/{Total}", batch.Count, states.Count);
            }
        }
    }
}
