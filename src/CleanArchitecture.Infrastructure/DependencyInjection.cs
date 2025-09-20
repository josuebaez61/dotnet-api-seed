using System;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Infrastructure.Services.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      // Database
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                 .UseSnakeCaseNamingConvention());

      // Register IApplicationDbContext
      services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

      // Identity - Full configuration with SignInManager
      services.AddIdentity<User, Role>(options =>
      {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;

        // User settings
        options.User.RequireUniqueEmail = true;
      })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

      // Repositories
      services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      services.AddScoped<IUnitOfWork, UnitOfWork>();

      // Specific repositories
      services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();
      services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();

      // Seeders
      services.AddScoped<ISeeder, RoleSeeder>();
      services.AddScoped<ISeeder, PermissionSeeder>();
      services.AddScoped<ISeeder, RolePermissionSeeder>();
      services.AddScoped<ISeeder, AdminUserSeeder>();
      services.AddScoped<ISeeder, CountrySeeder>();
      services.AddScoped<ISeeder, CitySeeder>();
      services.AddScoped<SeederRunner>();


      // Database initialization
      services.AddScoped<DatabaseInitializationService>();

      // Cleanup services
      services.AddHostedService<CleanupService>();
      services.AddScoped<ICleanupService, ManualCleanupService>();

      return services;
    }
  }
}
