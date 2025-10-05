using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitecture.Infrastructure.Repositories
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ApplicationDbContext context,
        IPasswordResetCodeRepository passwordResetCodes,
        IRepository<User> users,
        IRepository<Role> roles,
        IRepository<Permission> permissions,
        IRepository<RolePermission> rolePermissions,
        IRepository<UserRole> userRoles,
        IRepository<Country> countries,
        IRepository<City> cities)
    {
      _context = context;
      PasswordResetCodes = passwordResetCodes;
      Users = users;
      Roles = roles;
      Permissions = permissions;
      RolePermissions = rolePermissions;
      UserRoles = userRoles;
      Countries = countries;
      Cities = cities;
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