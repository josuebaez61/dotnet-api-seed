using CleanArchitecture.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services
{
  public class CleanupService : BackgroundService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupService> _logger;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _cleanupInterval;
    private readonly bool _enabled;

    public CleanupService(IServiceProvider serviceProvider, ILogger<CleanupService> logger, IConfiguration configuration)
    {
      _serviceProvider = serviceProvider;
      _logger = logger;
      _configuration = configuration;

      var intervalHours = _configuration.GetValue<double>("CleanupSettings:IntervalHours", 1.0);
      _cleanupInterval = TimeSpan.FromHours(intervalHours);
      _enabled = _configuration.GetValue<bool>("CleanupSettings:Enabled", true);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      if (!_enabled)
      {
        _logger.LogInformation("CleanupService is disabled in configuration");
        return;
      }

      _logger.LogInformation("CleanupService started. Cleanup interval: {Interval}, Enabled: {Enabled}", _cleanupInterval, _enabled);

      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          await PerformCleanupAsync();
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error occurred during cleanup process");
        }

        await Task.Delay(_cleanupInterval, stoppingToken);
      }

      _logger.LogInformation("CleanupService stopped");
    }

    private async Task PerformCleanupAsync()
    {
      using var scope = _serviceProvider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

      _logger.LogInformation("Starting cleanup of expired verification codes");

      var currentTime = DateTime.UtcNow;
      var deletedCount = 0;

      // Limpiar códigos de verificación de email expirados
      var expiredEmailCodes = context.EmailVerificationCodes
          .Where(evc => evc.ExpiresAt < currentTime && !evc.IsDeleted)
          .ToList();

      foreach (var code in expiredEmailCodes)
      {
        code.IsDeleted = true;
        code.UpdatedAt = currentTime;
        deletedCount++;
      }

      // Limpiar códigos de reset de contraseña expirados
      var expiredPasswordCodes = context.PasswordResetCodes
          .Where(prc => prc.ExpiresAt < currentTime && !prc.IsDeleted)
          .ToList();

      foreach (var code in expiredPasswordCodes)
      {
        code.IsDeleted = true;
        code.UpdatedAt = currentTime;
        deletedCount++;
      }

      if (deletedCount > 0)
      {
        await context.SaveChangesAsync();
        _logger.LogInformation("Cleanup completed. Marked {Count} expired codes as deleted", deletedCount);
      }
      else
      {
        _logger.LogInformation("Cleanup completed. No expired codes found");
      }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("CleanupService is stopping");
      await base.StopAsync(cancellationToken);
    }
  }
}
