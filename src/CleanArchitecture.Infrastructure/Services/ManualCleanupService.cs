using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services
{
  public class ManualCleanupService : ICleanupService
  {
    private readonly IEmailVerificationCodeRepository _emailCodeRepository;
    private readonly IPasswordResetCodeRepository _passwordCodeRepository;
    private readonly ILogger<ManualCleanupService> _logger;

    public ManualCleanupService(
        IEmailVerificationCodeRepository emailCodeRepository,
        IPasswordResetCodeRepository passwordCodeRepository,
        ILogger<ManualCleanupService> logger)
    {
      _emailCodeRepository = emailCodeRepository;
      _passwordCodeRepository = passwordCodeRepository;
      _logger = logger;
    }

    public async Task CleanupExpiredCodesAsync()
    {
      _logger.LogInformation("Starting manual cleanup of expired verification codes");

      // Limpiar códigos de verificación de email expirados
      await _emailCodeRepository.CleanupExpiredCodesAsync();

      // Limpiar códigos de reset de contraseña expirados
      await _passwordCodeRepository.CleanupExpiredCodesAsync();

      _logger.LogInformation("Manual cleanup completed successfully");
    }
  }
}
