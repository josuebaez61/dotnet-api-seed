using System;
using System.Collections.Generic;
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
  /// Seeder for countries and states data
  /// </summary>
  public class CountrySeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CountrySeeder> _logger;

    public string Name => "Countries";

    public CountrySeeder(ApplicationDbContext context, ILogger<CountrySeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task SeedAsync()
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
        _logger.LogInformation("📖 Loading countries from JSON file...");

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.Countries.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
          _logger.LogError("❌ Countries.json resource not found");
          throw new InvalidOperationException("Countries.json resource not found");
        }

        using var reader = new StreamReader(stream);
        var jsonContent = await reader.ReadToEndAsync();

        var countries = JsonSerializer.Deserialize<List<CountryJsonDto>>(jsonContent, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (countries == null || countries.Count == 0)
        {
          _logger.LogWarning("⚠️ No countries found in JSON file");
          return;
        }

        _logger.LogInformation($"📊 Found {countries.Count} countries in JSON file");

        // Convert to entities and insert in batches
        await InsertCountriesBatch(countries);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error loading countries data from JSON file");
        throw;
      }
    }

    private async Task LoadStatesFromJsonFile()
    {
      try
      {
        _logger.LogInformation("📖 Loading states from JSON file...");

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "CleanArchitecture.Infrastructure.Data.Seeds.States.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
          _logger.LogError("❌ States.json resource not found");
          throw new InvalidOperationException("States.json resource not found");
        }

        using var reader = new StreamReader(stream);
        var jsonContent = await reader.ReadToEndAsync();

        var states = JsonSerializer.Deserialize<List<StateJsonDto>>(jsonContent, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (states == null || states.Count == 0)
        {
          _logger.LogWarning("⚠️ No states found in JSON file");
          return;
        }

        _logger.LogInformation($"📊 Found {states.Count} states in JSON file");

        // Get all countries for matching
        var countries = await _context.Countries.ToListAsync();
        _logger.LogInformation($"🗺️ Found {countries.Count} countries in database");

        // Convert to entities and insert in batches
        await InsertStatesBatch(states, countries);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error loading states data from JSON file");
        throw;
      }
    }

    private async Task InsertCountriesBatch(List<CountryJsonDto> countries)
    {
      try
      {
        const int batchSize = 100;
        var batches = countries.Chunk(batchSize);

        foreach (var batch in batches)
        {
          var countryEntities = batch.Select(seedModel => new Country
          {
            Id = seedModel.Id,
            Name = seedModel.Name,
            Iso2 = seedModel.Iso2,
            Iso3 = seedModel.Iso3,
            NumericCode = seedModel.NumericCode,
            Phonecode = seedModel.Phonecode,
            Capital = seedModel.Capital,
            Currency = seedModel.Currency,
            CurrencyName = seedModel.CurrencyName,
            CurrencySymbol = seedModel.CurrencySymbol,
            Tld = seedModel.Tld,
            Native = seedModel.Native,
            Latitude = decimal.TryParse(seedModel.Latitude, out var lat) ? lat : null,
            Longitude = decimal.TryParse(seedModel.Longitude, out var lng) ? lng : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
          }).ToList();

          _context.Countries.AddRange(countryEntities);
          await _context.SaveChangesAsync();

          _logger.LogInformation($"✅ Inserted batch of {countryEntities.Count} countries");
        }

        _logger.LogInformation($"✅ Successfully inserted {countries.Count} countries");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error inserting countries batch");
        throw;
      }
    }

    private async Task InsertStatesBatch(List<StateJsonDto> states, List<Country> countries)
    {
      try
      {
        const int batchSize = 100;
        var batches = states.Chunk(batchSize);

        foreach (var batch in batches)
        {
          var stateEntities = new List<State>();

          foreach (var seedModel in batch)
          {
            var country = countries.FirstOrDefault(c => c.Name == seedModel.CountryName);
            if (country == null)
            {
              _logger.LogWarning($"⚠️ Country '{seedModel.CountryName}' not found for state '{seedModel.Name}'");
              continue;
            }

            stateEntities.Add(new State
            {
              Id = seedModel.Id,
              Name = seedModel.Name,
              CountryId = country.Id,
              CountryCode = seedModel.CountryCode ?? string.Empty,
              FipsCode = seedModel.FipsCode,
              Iso2 = seedModel.Iso2,
              Type = seedModel.Type,
              Latitude = decimal.TryParse(seedModel.Latitude, out var lat) ? lat : null,
              Longitude = decimal.TryParse(seedModel.Longitude, out var lng) ? lng : null,
              CreatedAt = DateTime.UtcNow,
              UpdatedAt = DateTime.UtcNow
            });
          }

          if (stateEntities.Count > 0)
          {
            _context.States.AddRange(stateEntities);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ Inserted batch of {stateEntities.Count} states");
          }
        }

        _logger.LogInformation($"✅ Successfully inserted states");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "❌ Error inserting states batch");
        throw;
      }
    }
  }
}
