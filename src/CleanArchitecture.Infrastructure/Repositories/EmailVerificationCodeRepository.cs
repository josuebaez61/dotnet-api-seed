using System.Linq.Expressions;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
  public class EmailVerificationCodeRepository : IRepository<EmailVerificationCode>, IEmailVerificationCodeRepository
  {
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<EmailVerificationCode> _dbSet;

    public EmailVerificationCodeRepository(ApplicationDbContext context)
    {
      _context = context;
      _dbSet = context.Set<EmailVerificationCode>();
    }

    #region IRepository Implementation

    public virtual async Task<EmailVerificationCode?> GetByIdAsync(Guid id)
    {
      return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<EmailVerificationCode>> GetAllAsync()
    {
      return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
    }

    public virtual async Task<IEnumerable<EmailVerificationCode>> FindAsync(Expression<Func<EmailVerificationCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).Where(predicate).ToListAsync();
    }

    public virtual async Task<EmailVerificationCode?> FirstOrDefaultAsync(Expression<Func<EmailVerificationCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<EmailVerificationCode> AddAsync(EmailVerificationCode entity)
    {
      await _dbSet.AddAsync(entity);
      return entity;
    }

    public virtual async Task<IEnumerable<EmailVerificationCode>> AddRangeAsync(IEnumerable<EmailVerificationCode> entities)
    {
      await _dbSet.AddRangeAsync(entities);
      return entities;
    }

    public virtual Task UpdateAsync(EmailVerificationCode entity)
    {
      _dbSet.Update(entity);
      return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(EmailVerificationCode entity)
    {
      // Soft delete
      entity.IsDeleted = true;
      entity.UpdatedAt = DateTime.UtcNow;
      _dbSet.Update(entity);
      return Task.CompletedTask;
    }

    public virtual async Task DeleteByIdAsync(Guid id)
    {
      var entity = await GetByIdAsync(id);
      if (entity != null)
      {
        await DeleteAsync(entity);
      }
    }

    public virtual async Task<int> CountAsync()
    {
      return await _dbSet.Where(x => !x.IsDeleted).CountAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<EmailVerificationCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).Where(predicate).CountAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<EmailVerificationCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).AnyAsync(predicate);
    }

    #endregion

    #region Specific Methods

    public async Task<EmailVerificationCode?> GetByVerificationCodeAsync(string verificationCode)
    {
      return await _dbSet
          .Where(x => !x.IsDeleted)
          .FirstOrDefaultAsync(x => x.VerificationCode == verificationCode);
    }

    public async Task<EmailVerificationCode?> GetActiveCodeByUserIdAsync(Guid userId)
    {
      return await _dbSet
          .Where(x => !x.IsDeleted && !x.IsUsed)
          .Where(x => x.UserId == userId)
          .Where(x => x.ExpiresAt > DateTime.UtcNow)
          .OrderByDescending(x => x.CreatedAt)
          .FirstOrDefaultAsync();
    }

    public async Task<EmailVerificationCode?> GetActiveCodeByEmailAsync(string email)
    {
      return await _dbSet
          .Where(x => !x.IsDeleted && !x.IsUsed)
          .Where(x => x.Email == email)
          .Where(x => x.ExpiresAt > DateTime.UtcNow)
          .OrderByDescending(x => x.CreatedAt)
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<EmailVerificationCode>> GetExpiredCodesAsync()
    {
      return await _dbSet
          .Where(x => !x.IsDeleted)
          .Where(x => x.ExpiresAt < DateTime.UtcNow)
          .ToListAsync();
    }

    public async Task<IEnumerable<EmailVerificationCode>> GetUsedCodesAsync()
    {
      return await _dbSet
          .Where(x => !x.IsDeleted)
          .Where(x => x.IsUsed)
          .ToListAsync();
    }

    public async Task CleanupExpiredCodesAsync()
    {
      var expiredCodes = await GetExpiredCodesAsync();
      foreach (var code in expiredCodes)
      {
        code.IsDeleted = true;
        code.UpdatedAt = DateTime.UtcNow;
      }
      await _context.SaveChangesAsync();
    }

    public async Task MarkAsUsedAsync(Guid id)
    {
      var code = await GetByIdAsync(id);
      if (code != null)
      {
        code.IsUsed = true;
        code.UsedAt = DateTime.UtcNow;
        code.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(code);
      }
    }

    public async Task<bool> HasActiveCodeForEmailAsync(string email)
    {
      return await _dbSet
          .Where(x => !x.IsDeleted && !x.IsUsed)
          .Where(x => x.Email == email)
          .Where(x => x.ExpiresAt > DateTime.UtcNow)
          .AnyAsync();
    }

    #endregion
  }
}
