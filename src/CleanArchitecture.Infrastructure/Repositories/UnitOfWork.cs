using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitecture.Infrastructure.Repositories
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly IApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(IApplicationDbContext context)
    {
      _context = context;
      PasswordResetCodes = new PasswordResetCodeRepository(_context);
      Users = new Repository<User>(_context);
      Roles = new Repository<Role>(_context);
      Permissions = new Repository<Permission>(_context);
      RolePermissions = new Repository<RolePermission>(_context);
      UserRoles = new Repository<UserRole>(_context);
      Countries = new Repository<Country>(_context);
      Cities = new Repository<City>(_context);
    }

    public IPasswordResetCodeRepository PasswordResetCodes { get; }
    public IRepository<User> Users { get; }
    public IRepository<Role> Roles { get; }
    public IRepository<Permission> Permissions { get; }
    public IRepository<RolePermission> RolePermissions { get; }
    public IRepository<UserRole> UserRoles { get; }
    public IRepository<Country> Countries { get; }
    public IRepository<City> Cities { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
      _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
      if (_transaction != null)
      {
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
      }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
      if (_transaction != null)
      {
        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
      }
    }

    public void Dispose()
    {
      _transaction?.Dispose();
    }
  }
}