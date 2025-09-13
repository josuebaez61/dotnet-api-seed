using CleanArchitecture.Domain.Common.Interfaces;
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
      var emailCodeRepository = scope.ServiceProvider.GetRequiredService<IEmailVerificationCodeRepository>();
      var passwordCodeRepository = scope.ServiceProvider.GetRequiredService<IPasswordResetCodeRepository>();

      _logger.LogInformation("Starting cleanup of expired verification codes");

      // Limpiar códigos de verificación de email expirados
      await emailCodeRepository.CleanupExpiredCodesAsync();

      // Limpiar códigos de reset de contraseña expirados
      await passwordCodeRepository.CleanupExpiredCodesAsync();

      _logger.LogInformation("Cleanup completed successfully");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("CleanupService is stopping");
      await base.StopAsync(cancellationToken);
    }
  }
}
