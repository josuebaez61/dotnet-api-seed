using System.Linq.Expressions;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
  public class PasswordResetCodeRepository : IRepository<PasswordResetCode>, IPasswordResetCodeRepository
  {
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<PasswordResetCode> _dbSet;

    public PasswordResetCodeRepository(ApplicationDbContext context)
    {
      _context = context;
      _dbSet = context.Set<PasswordResetCode>();
    }

    #region IRepository Implementation

    public virtual async Task<PasswordResetCode?> GetByIdAsync(Guid id)
    {
      return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<PasswordResetCode>> GetAllAsync()
    {
      return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
    }

    public virtual async Task<IEnumerable<PasswordResetCode>> FindAsync(Expression<Func<PasswordResetCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).Where(predicate).ToListAsync();
    }

    public virtual async Task<PasswordResetCode?> FirstOrDefaultAsync(Expression<Func<PasswordResetCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<PasswordResetCode> AddAsync(PasswordResetCode entity)
    {
      await _dbSet.AddAsync(entity);
      return entity;
    }

    public virtual async Task<IEnumerable<PasswordResetCode>> AddRangeAsync(IEnumerable<PasswordResetCode> entities)
    {
      await _dbSet.AddRangeAsync(entities);
      return entities;
    }

    public virtual Task UpdateAsync(PasswordResetCode entity)
    {
      _dbSet.Update(entity);
      return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(PasswordResetCode entity)
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

    public virtual async Task<int> CountAsync(Expression<Func<PasswordResetCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).Where(predicate).CountAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<PasswordResetCode, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).AnyAsync(predicate);
    }

    #endregion

    #region Specific Methods

    public async Task<PasswordResetCode?> GetByCodeAsync(string code)
    {
      return await _dbSet
          .Where(x => !x.IsDeleted)
          .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<PasswordResetCode?> GetActiveCodeByUserIdAsync(Guid userId)
    {
      return await _dbSet
          .Where(x => !x.IsDeleted && !x.IsUsed)
          .Where(x => x.UserId == userId)
          .Where(x => x.ExpiresAt > DateTime.UtcNow)
          .OrderByDescending(x => x.CreatedAt)
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PasswordResetCode>> GetExpiredCodesAsync()
    {
      return await _dbSet
          .Where(x => !x.IsDeleted)
          .Where(x => x.ExpiresAt < DateTime.UtcNow)
          .ToListAsync();
    }

    public async Task<IEnumerable<PasswordResetCode>> GetUsedCodesAsync()
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

    #endregion
  }
}
