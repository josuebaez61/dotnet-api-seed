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
                var sqlFilePath = "/Users/josuebaez/Downloads/countries.sql";
                if (!File.Exists(sqlFilePath))
                {
                    _logger.LogWarning("⚠️ Countries SQL file not found at: {FilePath}", sqlFilePath);
                    return;
                }

                var sqlContent = await File.ReadAllTextAsync(sqlFilePath);
                
                // Extract INSERT statements and execute them
                var insertStatements = ExtractInsertStatements(sqlContent, "countries");
                
                foreach (var statement in insertStatements)
                {
                    // Convert MySQL INSERT to PostgreSQL compatible format
                    var postgresStatement = ConvertMySqlToPostgres(statement, "countries");
                    if (!string.IsNullOrEmpty(postgresStatement))
                    {
                        await _context.Database.ExecuteSqlRawAsync(postgresStatement);
                    }
                }

                _logger.LogInformation($"✅ Loaded {insertStatements.Count} countries from SQL file");
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
                var sqlFilePath = "/Users/josuebaez/Downloads/states.sql";
                if (!File.Exists(sqlFilePath))
                {
                    _logger.LogWarning("⚠️ States SQL file not found at: {FilePath}", sqlFilePath);
                    return;
                }

                var sqlContent = await File.ReadAllTextAsync(sqlFilePath);
                
                // Extract INSERT statements and execute them
                var insertStatements = ExtractInsertStatements(sqlContent, "states");
                
                foreach (var statement in insertStatements)
                {
                    // Convert MySQL INSERT to PostgreSQL compatible format
                    var postgresStatement = ConvertMySqlToPostgres(statement, "states");
                    if (!string.IsNullOrEmpty(postgresStatement))
                    {
                        await _context.Database.ExecuteSqlRawAsync(postgresStatement);
                    }
                }

                _logger.LogInformation($"✅ Loaded {insertStatements.Count} states from SQL file");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading states from SQL file");
                throw;
            }
        }

        private List<string> ExtractInsertStatements(string sqlContent, string tableName)
        {
            var statements = new List<string>();
            var pattern = $@"INSERT INTO `{tableName}` VALUES\s*\(([^;]+)\);";
            var matches = System.Text.RegularExpressions.Regex.Matches(sqlContent, pattern, System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.Singleline);
            
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                statements.Add(match.Groups[0].Value);
            }
            
            return statements;
        }

        private string ConvertMySqlToPostgres(string mysqlStatement, string tableName)
        {
            try
            {
                // Convert MySQL backticks to PostgreSQL quotes
                var postgresStatement = mysqlStatement.Replace("`", "\"");
                
                // Convert table name to snake_case
                postgresStatement = postgresStatement.Replace($"\"{tableName}\"", $"\"{tableName}\"");
                
                // Handle MySQL specific syntax
                postgresStatement = postgresStatement.Replace("AUTO_INCREMENT", "");
                postgresStatement = postgresStatement.Replace("ENGINE=InnoDB", "");
                postgresStatement = postgresStatement.Replace("DEFAULT CHARSET=utf8mb4", "");
                postgresStatement = postgresStatement.Replace("COLLATE=utf8mb4_unicode_ci", "");
                postgresStatement = postgresStatement.Replace("ROW_FORMAT=COMPACT", "");
                
                // Remove MySQL specific clauses
                postgresStatement = System.Text.RegularExpressions.Regex.Replace(postgresStatement, @"KEY `[^`]+` \([^)]+\)", "");
                postgresStatement = System.Text.RegularExpressions.Regex.Replace(postgresStatement, @"CONSTRAINT `[^`]+` FOREIGN KEY \([^)]+\) REFERENCES `[^`]+` \(`[^`]+`\)", "");
                
                // Clean up extra spaces and commas
                postgresStatement = System.Text.RegularExpressions.Regex.Replace(postgresStatement, @"\s+", " ");
                postgresStatement = System.Text.RegularExpressions.Regex.Replace(postgresStatement, @",\s*\)", ")");
                
                return postgresStatement.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error converting MySQL to PostgreSQL statement: {Statement}", mysqlStatement);
                return string.Empty;
            }
        }

    }
}
