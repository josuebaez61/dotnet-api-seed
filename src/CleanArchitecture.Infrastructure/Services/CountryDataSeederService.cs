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

                // Parse and insert countries using Entity Framework
                var countries = ParseCountriesFromSql(sqlContent);

                // Insert in batches to avoid memory issues
                const int batchSize = 100;
                for (int i = 0; i < countries.Count; i += batchSize)
                {
                    var batch = countries.Skip(i).Take(batchSize);
                    _context.Countries.AddRange(batch);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"✅ Loaded {countries.Count} countries from SQL file");
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

                // Parse and insert states using Entity Framework
                var states = ParseStatesFromSql(sqlContent);

                // Insert in batches to avoid memory issues
                const int batchSize = 100;
                for (int i = 0; i < states.Count; i += batchSize)
                {
                    var batch = states.Skip(i).Take(batchSize);
                    _context.States.AddRange(batch);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"✅ Loaded {states.Count} states from SQL file");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading states from SQL file");
                throw;
            }
        }

        private List<Country> ParseCountriesFromSql(string sqlContent)
        {
            var countries = new List<Country>();

            // Find all INSERT statements for countries
            var insertPattern = @"INSERT INTO `countries` VALUES\s*";
            var insertMatches = System.Text.RegularExpressions.Regex.Matches(sqlContent, insertPattern, System.Text.RegularExpressions.RegexOptions.Multiline);

            foreach (System.Text.RegularExpressions.Match insertMatch in insertMatches)
            {
                var startIndex = insertMatch.Index;
                var endIndex = sqlContent.IndexOf(";", startIndex);

                if (endIndex > startIndex)
                {
                    var statement = sqlContent.Substring(startIndex, endIndex - startIndex);

                    // Extract VALUES part
                    var valuesStart = statement.IndexOf("VALUES") + 6;
                    var valuesPart = statement.Substring(valuesStart).Trim();

                    // Parse individual records
                    var records = ParseSqlRecords(valuesPart);

                    foreach (var record in records)
                    {
                        if (record.Count >= 20) // Ensure we have enough fields
                        {
                            var country = new Country
                            {
                                Id = ParseInt(record[0]),
                                Name = ParseString(record[1]),
                                Iso3 = ParseString(record[2]),
                                NumericCode = ParseString(record[3]),
                                Iso2 = ParseString(record[4]),
                                Phonecode = ParseString(record[5]),
                                Capital = ParseString(record[6]),
                                Currency = ParseString(record[7]),
                                CurrencyName = ParseString(record[8]),
                                CurrencySymbol = ParseString(record[9]),
                                Tld = ParseString(record[10]),
                                Native = ParseString(record[11]),
                                Nationality = ParseString(record[12]),
                                Timezones = ParseString(record[13]),
                                Translations = ParseString(record[14]),
                                Latitude = ParseDecimal(record[15]),
                                Longitude = ParseDecimal(record[16]),
                                CreatedAt = ParseDateTime(record[17]),
                                UpdatedAt = ParseDateTime(record[18]) ?? DateTime.UtcNow,
                                Flag = ParseBool(record[19])
                            };

                            countries.Add(country);
                        }
                    }
                }
            }

            return countries;
        }

        private List<State> ParseStatesFromSql(string sqlContent)
        {
            var states = new List<State>();

            // Find all INSERT statements for states
            var insertPattern = @"INSERT INTO `states` VALUES\s*";
            var insertMatches = System.Text.RegularExpressions.Regex.Matches(sqlContent, insertPattern, System.Text.RegularExpressions.RegexOptions.Multiline);

            foreach (System.Text.RegularExpressions.Match insertMatch in insertMatches)
            {
                var startIndex = insertMatch.Index;
                var endIndex = sqlContent.IndexOf(";", startIndex);

                if (endIndex > startIndex)
                {
                    var statement = sqlContent.Substring(startIndex, endIndex - startIndex);

                    // Extract VALUES part
                    var valuesStart = statement.IndexOf("VALUES") + 6;
                    var valuesPart = statement.Substring(valuesStart).Trim();

                    // Parse individual records
                    var records = ParseSqlRecords(valuesPart);

                    foreach (var record in records)
                    {
                        if (record.Count >= 16) // Ensure we have enough fields
                        {
                            var state = new State
                            {
                                Id = ParseInt(record[0]),
                                Name = ParseString(record[1]),
                                CountryId = ParseInt(record[2]),
                                CountryCode = ParseString(record[3]) ?? "",
                                FipsCode = ParseString(record[4]),
                                Iso2 = ParseString(record[5]),
                                Iso31662 = ParseString(record[6]),
                                Type = ParseString(record[7]),
                                Level = ParseInt(record[8]),
                                ParentId = ParseInt(record[9]),
                                Native = ParseString(record[10]),
                                Latitude = ParseDecimal(record[11]),
                                Longitude = ParseDecimal(record[12]),
                                Timezone = ParseString(record[13]),
                                CreatedAt = ParseDateTime(record[14]),
                                UpdatedAt = ParseDateTime(record[15]) ?? DateTime.UtcNow,
                                Flag = ParseBool(record.Count > 16 ? record[16] : "1")
                            };

                            states.Add(state);
                        }
                    }
                }
            }

            return states;
        }

        private List<List<string>> ParseSqlRecords(string valuesPart)
        {
            var records = new List<List<string>>();

            // Remove outer parentheses
            valuesPart = valuesPart.Trim();
            if (valuesPart.StartsWith("("))
            {
                valuesPart = valuesPart.Substring(1);
            }
            if (valuesPart.EndsWith(")"))
            {
                valuesPart = valuesPart.Substring(0, valuesPart.Length - 1);
            }

            // Split by ),( to separate records
            var recordStrings = valuesPart.Split(new[] { "),(" }, StringSplitOptions.None);

            foreach (var recordString in recordStrings)
            {
                var fields = ParseSqlFields(recordString);
                if (fields.Count > 0)
                {
                    records.Add(fields);
                }
            }

            return records;
        }

        private List<string> ParseSqlFields(string recordString)
        {
            var fields = new List<string>();
            var currentField = "";
            var inQuotes = false;
            var quoteChar = '\0';

            for (int i = 0; i < recordString.Length; i++)
            {
                var currentChar = recordString[i];

                if ((currentChar == '\'' || currentChar == '"') && !inQuotes)
                {
                    inQuotes = true;
                    quoteChar = currentChar;
                    currentField += currentChar;
                }
                else if (currentChar == quoteChar && inQuotes)
                {
                    inQuotes = false;
                    quoteChar = '\0';
                    currentField += currentChar;
                }
                else if (currentChar == ',' && !inQuotes)
                {
                    fields.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += currentChar;
                }
            }

            if (!string.IsNullOrEmpty(currentField))
            {
                fields.Add(currentField.Trim());
            }

            return fields;
        }

        private int ParseInt(string value)
        {
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
                return 0;

            if (int.TryParse(value.Trim('\'', '"'), out var result))
                return result;

            return 0;
        }

        private string? ParseString(string value)
        {
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
                return null;

            return value.Trim('\'', '"');
        }

        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
                return null;

            if (decimal.TryParse(value.Trim('\'', '"'), out var result))
                return result;

            return null;
        }

        private DateTime? ParseDateTime(string value)
        {
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
                return null;

            if (DateTime.TryParse(value.Trim('\'', '"'), out var result))
                return result;

            return null;
        }

        private bool ParseBool(string value)
        {
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
                return false;

            var cleanValue = value.Trim('\'', '"');
            return cleanValue == "1" || cleanValue.ToLower() == "true";
        }

    }
}
