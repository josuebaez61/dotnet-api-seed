using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitecture.Infrastructure.Repositories
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(ApplicationDbContext context)
    {
      _context = context;
      _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
      var type = typeof(T);
      if (!_repositories.ContainsKey(type))
      {
        _repositories[type] = new Repository<T>(_context);
      }
      return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
      return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
      _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
      if (_transaction != null)
      {
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
      }
    }

    public async Task RollbackTransactionAsync()
    {
      if (_transaction != null)
      {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
      }
    }

    public void Dispose()
    {
      _transaction?.Dispose();
      _context.Dispose();
    }
  }
}
