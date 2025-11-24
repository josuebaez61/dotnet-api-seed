using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
  public class AddressRespository : Repository<Address>
  {
    public AddressRespository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Address?> GetByIdAsync(Guid id)
    {
      return await _dbSet
          .Include(a => a.City)
          .Include(a => a.State)
          .Include(a => a.Country)
          .Where(x => !x.IsDeleted && x.Id == id)
          .FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<Address>> GetAllAsync()
    {
      return await _dbSet
          .Include(a => a.City)
          .Include(a => a.State)
          .Include(a => a.Country)
          .Where(x => !x.IsDeleted)
          .ToListAsync();
    }

    public override async Task<IEnumerable<Address>> FindAsync(Expression<Func<Address, bool>> predicate)
    {
      return await _dbSet
          .Include(a => a.City)
          .Include(a => a.State)
          .Include(a => a.Country)
          .Where(x => !x.IsDeleted)
          .Where(predicate)
          .ToListAsync();
    }

    public override async Task<Address?> FirstOrDefaultAsync(Expression<Func<Address, bool>> predicate)
    {
      return await _dbSet
          .Include(a => a.City)
          .Include(a => a.State)
          .Include(a => a.Country)
          .Where(x => !x.IsDeleted)
          .FirstOrDefaultAsync(predicate);
    }

    public override Task UpdateAsync(Address entity)
    {
      entity.UpdatedAt = DateTime.UtcNow;
      return base.UpdateAsync(entity);
    }

    public override Task DeleteAsync(Address entity)
    {
      // Soft delete
      entity.IsDeleted = true;
      entity.UpdatedAt = DateTime.UtcNow;
      _dbSet.Update(entity);
      return Task.CompletedTask;
    }

    public override async Task<int> CountAsync()
    {
      return await _dbSet.Where(x => !x.IsDeleted).CountAsync();
    }

    public override async Task<int> CountAsync(Expression<Func<Address, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).Where(predicate).CountAsync();
    }

    public override async Task<bool> ExistsAsync(Expression<Func<Address, bool>> predicate)
    {
      return await _dbSet.Where(x => !x.IsDeleted).AnyAsync(predicate);
    }
  }
}

