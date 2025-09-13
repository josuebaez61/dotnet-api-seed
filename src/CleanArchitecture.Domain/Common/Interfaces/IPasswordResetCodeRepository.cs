using System.Linq.Expressions;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Common.Interfaces
{
  public interface IPasswordResetCodeRepository : IRepository<PasswordResetCode>
  {
    Task<PasswordResetCode?> GetByCodeAsync(string code);
    Task<PasswordResetCode?> GetActiveCodeByUserIdAsync(Guid userId);
    Task<IEnumerable<PasswordResetCode>> GetExpiredCodesAsync();
    Task<IEnumerable<PasswordResetCode>> GetUsedCodesAsync();
    Task CleanupExpiredCodesAsync();
    Task MarkAsUsedAsync(Guid id);
  }
}
