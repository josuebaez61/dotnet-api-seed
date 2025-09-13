using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services
{
  public class ManualCleanupService : ICleanupService
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ManualCleanupService> _logger;

    public ManualCleanupService(ApplicationDbContext context, ILogger<ManualCleanupService> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task CleanupExpiredCodesAsync()
    {
      _logger.LogInformation("Starting manual cleanup of expired verification codes");

      var currentTime = DateTime.UtcNow;
      var deletedCount = 0;

      // Limpiar códigos de verificación de email expirados
      var expiredEmailCodes = await _context.EmailVerificationCodes
          .Where(evc => evc.ExpiresAt < currentTime && !evc.IsDeleted)
          .ToListAsync();

      foreach (var code in expiredEmailCodes)
      {
        code.IsDeleted = true;
        code.UpdatedAt = currentTime;
        deletedCount++;
      }

      // Limpiar códigos de reset de contraseña expirados
      var expiredPasswordCodes = await _context.PasswordResetCodes
          .Where(prc => prc.ExpiresAt < currentTime && !prc.IsDeleted)
          .ToListAsync();

      foreach (var code in expiredPasswordCodes)
      {
        code.IsDeleted = true;
        code.UpdatedAt = currentTime;
        deletedCount++;
      }

      if (deletedCount > 0)
      {
        await _context.SaveChangesAsync();
        _logger.LogInformation("Manual cleanup completed. Marked {Count} expired codes as deleted", deletedCount);
      }
      else
      {
        _logger.LogInformation("Manual cleanup completed. No expired codes found");
      }
    }
  }
}
