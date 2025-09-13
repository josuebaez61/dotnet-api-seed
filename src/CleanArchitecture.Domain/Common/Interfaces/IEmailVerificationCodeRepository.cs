using System.Linq.Expressions;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Common.Interfaces
{
  public interface IEmailVerificationCodeRepository : IRepository<EmailVerificationCode>
  {
    Task<EmailVerificationCode?> GetByVerificationCodeAsync(string verificationCode);
    Task<EmailVerificationCode?> GetActiveCodeByUserIdAsync(Guid userId);
    Task<EmailVerificationCode?> GetActiveCodeByEmailAsync(string email);
    Task<IEnumerable<EmailVerificationCode>> GetExpiredCodesAsync();
    Task<IEnumerable<EmailVerificationCode>> GetUsedCodesAsync();
    Task CleanupExpiredCodesAsync();
    Task MarkAsUsedAsync(Guid id);
    Task<bool> HasActiveCodeForEmailAsync(string email);
  }
}
