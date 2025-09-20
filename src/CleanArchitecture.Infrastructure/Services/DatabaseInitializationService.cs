using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Services.Seeders;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Services
{
  public class DatabaseInitializationService
  {
    private readonly ApplicationDbContext _context;
    private readonly SeederRunner _seederRunner;

    public DatabaseInitializationService(
        ApplicationDbContext context,
        SeederRunner seederRunner)
    {
      _context = context;
      _seederRunner = seederRunner;
    }

    public async Task InitializeAsync()
    {
      Console.WriteLine("🔍 DEBUG: Starting database initialization...");

      // Ensure database is created
      await _context.Database.EnsureCreatedAsync();
      Console.WriteLine("🔍 DEBUG: Database ensured created");

      // Run all seeders using SeederRunner
      await _seederRunner.RunAllSeedersAsync();
    }
  }
}
