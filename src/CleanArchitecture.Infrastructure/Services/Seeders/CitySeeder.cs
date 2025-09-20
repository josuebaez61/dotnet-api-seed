using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Data.Seeds.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services.Seeders
{
  /// <summary>
  /// Seeder for cities data
  /// </summary>
  public class CitySeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CitySeeder> _logger;

    public string Name => "Cities";

    public CitySeeder(ApplicationDbContext context, ILogger<CitySeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task SeedAsync()
    {
      try
      {
        _logger.LogInformation("🏙️ Starting to seed cities data from ArgentineCities.json...");

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

        _logger.LogInformation("🏙️ Successfully seeded cities data from JSON file");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error seeding cities data from JSON file");
        throw;
      }
    }

    private async Task<List<ArgentineCityDto>> LoadCitiesFromJsonFile()
    {
      try
      {
        _logger.LogInformation("📖 Loading cities from JSON file...");

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.ArgentineCities.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
          _logger.LogError("❌ ArgentineCities.json resource not found");
          throw new InvalidOperationException("ArgentineCities.json resource not found");
        }

        using var reader = new StreamReader(stream);
        var jsonContent = await reader.ReadToEndAsync();

        var cities = JsonSerializer.Deserialize<List<ArgentineCityDto>>(jsonContent, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (cities == null)
        {
          _logger.LogError("❌ Failed to deserialize cities from JSON file");
          throw new InvalidOperationException("Failed to deserialize cities from JSON file");
        }

        _logger.LogInformation("✅ Successfully loaded {CityCount} cities from JSON file", cities.Count);
        return cities;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error loading cities data from JSON file");
        throw;
      }
    }

    private async Task ProcessAndInsertCities(List<ArgentineCityDto> cities, List<State> states)
    {
      try
      {
        var cityEntities = new List<City>();
        var processedCount = 0;
        var skippedCount = 0;

        foreach (var citySeed in cities)
        {
          // Find the corresponding state (province in Argentina)
          var state = states.FirstOrDefault(s =>
            s.Name.Equals(citySeed.Provincia.Nombre, StringComparison.OrdinalIgnoreCase));

          if (state == null)
          {
            _logger.LogWarning("⚠️ State '{StateName}' not found for city '{CityName}'",
              citySeed.Provincia.Nombre, citySeed.Nombre);
            skippedCount++;
            continue;
          }

          var city = new City
          {
            Id = processedCount + 1, // Generate sequential ID
            Name = citySeed.Nombre,
            StateId = state.Id,
            Code = state.Iso2,
            Type = citySeed.Categoria,
            Latitude = (decimal)citySeed.Centroide.Lat,
            Longitude = (decimal)citySeed.Centroide.Lon,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
          };

          cityEntities.Add(city);
          processedCount++;

          // Insert in batches to avoid memory issues
          if (cityEntities.Count >= 1000)
          {
            _context.Cities.AddRange(cityEntities);
            await _context.SaveChangesAsync();
            _logger.LogInformation("✅ Inserted batch of {BatchCount} cities", cityEntities.Count);
            cityEntities.Clear();
          }
        }

        // Insert remaining cities
        if (cityEntities.Count > 0)
        {
          _context.Cities.AddRange(cityEntities);
          await _context.SaveChangesAsync();
          _logger.LogInformation("✅ Inserted final batch of {BatchCount} cities", cityEntities.Count);
        }

        _logger.LogInformation("📊 Processing summary:");
        _logger.LogInformation("  - Processed: {ProcessedCount} cities", processedCount);
        _logger.LogInformation("  - Skipped: {SkippedCount} cities", skippedCount);
        _logger.LogInformation("  - Total: {TotalCount} cities", processedCount + skippedCount);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error processing and inserting cities");
        throw;
      }
    }
  }
}
