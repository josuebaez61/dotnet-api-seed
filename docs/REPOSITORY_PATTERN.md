# Repository Pattern Implementation

This document describes the Repository Pattern implementation in the Clean Architecture project, including the generic repository, specific repositories, and Unit of Work pattern.

## Overview

The Repository Pattern provides an abstraction layer between the business logic and data access layers. This implementation uses Entity Framework Core with PostgreSQL and follows Clean Architecture principles, keeping data access concerns in the Infrastructure layer while exposing interfaces through the Domain layer.

## Architecture

The Repository Pattern is implemented across three layers:

- **Domain Layer**: Contains repository interfaces (`IRepository<T>`, `IUnitOfWork`)
- **Infrastructure Layer**: Contains concrete implementations (`Repository<T>`, `UnitOfWork`, specific repositories)
- **Application Layer**: Uses repositories through dependency injection

## Components

### 1. Generic Repository Interface

**Location**: `src/CleanArchitecture.Domain/Common/Interfaces/IRepository.cs`

The `IRepository<T>` interface defines the contract for all repositories:

```csharp
public interface IRepository<T> where T : class
{
  Task<T?> GetByIdAsync(Guid id);
  Task<IEnumerable<T>> GetAllAsync();
  Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
  Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
  Task<T> AddAsync(T entity);
  Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
  Task UpdateAsync(T entity);
  Task DeleteAsync(T entity);
  Task DeleteByIdAsync(Guid id);
  Task<int> CountAsync();
  Task<int> CountAsync(Expression<Func<T, bool>> predicate);
  Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
```

### 2. Generic Repository Implementation

**Location**: `src/CleanArchitecture.Infrastructure/Repositories/Repository.cs`

The `Repository<T>` class provides a generic implementation of `IRepository<T>` using Entity Framework Core:

```csharp
public class Repository<T> : IRepository<T> where T : class
{
  protected readonly ApplicationDbContext _context;
  protected readonly DbSet<T> _dbSet;

  public Repository(ApplicationDbContext context)
  {
    _context = context;
    _dbSet = context.Set<T>();
  }

  // Implementation of all IRepository<T> methods
}
```

**Key Features**:

- Generic implementation that works with any entity type
- All methods are `virtual` to allow overriding in derived classes
- Uses Entity Framework Core's `DbSet<T>` for data access
- Async/await pattern for all database operations

### 3. Specific Repositories

For entities that require custom business logic, specific repositories can extend the generic repository or implement the interface directly.

**Example**: `PasswordResetCodeRepository`

**Location**: `src/CleanArchitecture.Infrastructure/Repositories/PasswordResetCodeRepository.cs`

```csharp
public class PasswordResetCodeRepository : IRepository<PasswordResetCode>, IPasswordResetCodeRepository
{
  // Implements IRepository<PasswordResetCode> with soft delete support
  // Adds specific methods like GetByCodeAsync, GetActiveCodeByUserIdAsync
}
```

**Features of Specific Repositories**:

- Can override base repository methods for custom behavior (e.g., soft delete)
- Add domain-specific query methods
- Implement additional interfaces for specific functionality

### 4. Unit of Work Pattern

**Location**: `src/CleanArchitecture.Domain/Common/Interfaces/IUnitOfWork.cs`

The Unit of Work pattern manages transactions and coordinates multiple repositories:

```csharp
public interface IUnitOfWork : IDisposable
{
  IPasswordResetCodeRepository PasswordResetCodes { get; }
  IRepository<User> Users { get; }
  IRepository<Role> Roles { get; }
  IRepository<Permission> Permissions { get; }
  IRepository<RolePermission> RolePermissions { get; }
  IRepository<UserRole> UserRoles { get; }
  IRepository<Country> Countries { get; }
  IRepository<City> Cities { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  Task BeginTransactionAsync(CancellationToken cancellationToken = default);
  Task CommitTransactionAsync(CancellationToken cancellationToken = default);
  Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

**Implementation**: `src/CleanArchitecture.Infrastructure/Repositories/UnitOfWork.cs`

**Benefits**:

- Ensures all repositories share the same `DbContext` instance
- Provides transaction management
- Single point for saving changes across multiple repositories
- Maintains consistency across related operations

## Usage

### Dependency Injection

Repositories are registered in the Infrastructure layer's `DependencyInjection.cs`:

```csharp
// Generic repository registration
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Unit of Work registration
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Specific repositories
services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();
services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();
```

### Using Generic Repository

Inject `IRepository<T>` directly in your handlers or services:

```csharp
public class MyCommandHandler : IRequestHandler<MyCommand, MyResult>
{
  private readonly IRepository<Country> _countryRepository;

  public MyCommandHandler(IRepository<Country> countryRepository)
  {
    _countryRepository = countryRepository;
  }

  public async Task<MyResult> Handle(MyCommand request, CancellationToken cancellationToken)
  {
    // Get by ID
    var country = await _countryRepository.GetByIdAsync(request.CountryId);

    // Find with predicate
    var countries = await _countryRepository.FindAsync(c => c.Name.Contains("United"));

    // Add new entity
    var newCountry = new Country { Name = "New Country" };
    await _countryRepository.AddAsync(newCountry);
    await _countryRepository.SaveChangesAsync(); // Note: SaveChanges is on UnitOfWork

    return new MyResult();
  }
}
```

### Using Unit of Work

For operations involving multiple repositories or transactions:

```csharp
public class MyCommandHandler : IRequestHandler<MyCommand, MyResult>
{
  private readonly IUnitOfWork _unitOfWork;

  public MyCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<MyResult> Handle(MyCommand request, CancellationToken cancellationToken)
  {
    try
    {
      // Begin transaction
      await _unitOfWork.BeginTransactionAsync(cancellationToken);

      // Use multiple repositories
      var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
      var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);

      // Create user role relationship
      var userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
      await _unitOfWork.UserRoles.AddAsync(userRole);

      // Save all changes
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      // Commit transaction
      await _unitOfWork.CommitTransactionAsync(cancellationToken);

      return new MyResult();
    }
    catch
    {
      // Rollback on error
      await _unitOfWork.RollbackTransactionAsync(cancellationToken);
      throw;
    }
  }
}
```

### Using Specific Repository

For repositories with custom methods:

```csharp
public class MyCommandHandler : IRequestHandler<MyCommand, MyResult>
{
  private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;

  public MyCommandHandler(IPasswordResetCodeRepository passwordResetCodeRepository)
  {
    _passwordResetCodeRepository = passwordResetCodeRepository;
  }

  public async Task<MyResult> Handle(MyCommand request, CancellationToken cancellationToken)
  {
    // Use specific repository method
    var code = await _passwordResetCodeRepository.GetByCodeAsync(request.Code);

    // Use standard repository methods
    var activeCode = await _passwordResetCodeRepository.GetActiveCodeByUserIdAsync(request.UserId);

    return new MyResult();
  }
}
```

## Examples

### Example 1: Basic CRUD Operations

```csharp
public class CountryCommandHandler : IRequestHandler<CreateCountryCommand, CountryDto>
{
  private readonly IRepository<Country> _countryRepository;
  private readonly IMapper _mapper;

  public CountryCommandHandler(IRepository<Country> countryRepository, IMapper mapper)
  {
    _countryRepository = countryRepository;
    _mapper = mapper;
  }

  public async Task<CountryDto> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
  {
    // Check if exists
    var exists = await _countryRepository.ExistsAsync(c => c.Name == request.Name);
    if (exists)
    {
      throw new CountryAlreadyExistsError(request.Name);
    }

    // Create new entity
    var country = new Country
    {
      Id = Guid.NewGuid(),
      Name = request.Name,
      Code = request.Code
    };

    await _countryRepository.AddAsync(country);
    // Note: SaveChanges must be called via UnitOfWork or ApplicationDbContext

    return _mapper.Map<CountryDto>(country);
  }
}
```

### Example 2: Query with Predicates

```csharp
public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IEnumerable<CountryDto>>
{
  private readonly IRepository<Country> _countryRepository;
  private readonly IMapper _mapper;

  public GetCountriesQueryHandler(IRepository<Country> countryRepository, IMapper mapper)
  {
    _countryRepository = countryRepository;
    _mapper = mapper;
  }

  public async Task<IEnumerable<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
  {
    // Find with complex predicate
    var countries = await _countryRepository.FindAsync(c =>
      c.Name.Contains(request.SearchTerm) ||
      c.Code.Contains(request.SearchTerm)
    );

    // Count matching records
    var count = await _countryRepository.CountAsync(c => c.Name.Contains(request.SearchTerm));

    return _mapper.Map<IEnumerable<CountryDto>>(countries);
  }
}
```

### Example 3: Transaction with Multiple Repositories

```csharp
public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Unit>
{
  private readonly IUnitOfWork _unitOfWork;

  public AssignRoleToUserCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Unit> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
  {
    await _unitOfWork.BeginTransactionAsync(cancellationToken);

    try
    {
      // Verify user exists
      var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Verify role exists
      var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Check if relationship already exists
      var existing = await _unitOfWork.UserRoles.FirstOrDefaultAsync(
        ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId
      );

      if (existing != null)
      {
        throw new UserRoleAlreadyExistsError(request.UserId, request.RoleId);
      }

      // Create relationship
      var userRole = new UserRole
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        RoleId = request.RoleId
      };

      await _unitOfWork.UserRoles.AddAsync(userRole);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
      await _unitOfWork.CommitTransactionAsync(cancellationToken);

      return Unit.Value;
    }
    catch
    {
      await _unitOfWork.RollbackTransactionAsync(cancellationToken);
      throw;
    }
  }
}
```

### Example 4: Extending Repository for Soft Delete

```csharp
public class SoftDeleteRepository<T> : Repository<T> where T : class, ISoftDeletable
{
  public SoftDeleteRepository(ApplicationDbContext context) : base(context)
  {
  }

  public override async Task<IEnumerable<T>> GetAllAsync()
  {
    return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
  }

  public override async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
  {
    return await _dbSet
      .Where(x => !x.IsDeleted)
      .Where(predicate)
      .ToListAsync();
  }

  public override Task DeleteAsync(T entity)
  {
    // Soft delete instead of hard delete
    entity.IsDeleted = true;
    entity.UpdatedAt = DateTime.UtcNow;
    _dbSet.Update(entity);
    return Task.CompletedTask;
  }
}
```

## Available Repositories

The following repositories are available through `IUnitOfWork`:

- `Users` - `IRepository<User>`
- `Roles` - `IRepository<Role>`
- `Permissions` - `IRepository<Permission>`
- `RolePermissions` - `IRepository<RolePermission>`
- `UserRoles` - `IRepository<UserRole>`
- `Countries` - `IRepository<Country>`
- `Cities` - `IRepository<City>`
- `PasswordResetCodes` - `IPasswordResetCodeRepository` (specific repository)

## Important Notes

### SaveChanges

**Important**: The generic `Repository<T>` does not call `SaveChangesAsync()`. You must either:

1. Use `IUnitOfWork.SaveChangesAsync()` after operations
2. Inject `IApplicationDbContext` and call `SaveChangesAsync()` directly
3. Use Entity Framework's change tracking (changes are saved when the request completes)

### Transactions

Always use `IUnitOfWork` for transaction management:

```csharp
await _unitOfWork.BeginTransactionAsync(cancellationToken);
try
{
  // Your operations
  await _unitOfWork.SaveChangesAsync(cancellationToken);
  await _unitOfWork.CommitTransactionAsync(cancellationToken);
}
catch
{
  await _unitOfWork.RollbackTransactionAsync(cancellationToken);
  throw;
}
```

### Overriding Repository Methods

When creating specific repositories, you can override base methods:

```csharp
public override async Task<IEnumerable<MyEntity>> GetAllAsync()
{
  // Custom implementation with filtering, ordering, etc.
  return await _dbSet
    .Where(x => !x.IsDeleted)
    .OrderBy(x => x.CreatedAt)
    .ToListAsync();
}
```

## Best Practices

1. **Use Generic Repository for Simple CRUD**: Use `IRepository<T>` for standard operations
2. **Use Specific Repository for Complex Logic**: Create specific repositories when you need custom queries or business logic
3. **Use Unit of Work for Transactions**: Always use `IUnitOfWork` when working with multiple repositories or transactions
4. **Don't Mix DbContext and Repository**: Avoid injecting `ApplicationDbContext` directly when using repositories
5. **Override Methods Carefully**: When overriding repository methods, ensure you maintain the expected behavior
6. **Handle Transactions Properly**: Always use try-catch with rollback for transactions

## Troubleshooting

### Changes Not Saved

**Problem**: Changes made through repository are not persisted.

**Solution**: Ensure you call `SaveChangesAsync()` via `IUnitOfWork` or `IApplicationDbContext`:

```csharp
await _repository.AddAsync(entity);
await _unitOfWork.SaveChangesAsync(cancellationToken); // Required!
```

### Transaction Not Working

**Problem**: Transaction methods don't seem to work.

**Solution**: Ensure you're using the same `IUnitOfWork` instance for all operations in a transaction:

```csharp
// ✅ Correct
await _unitOfWork.BeginTransactionAsync();
await _unitOfWork.Users.AddAsync(user);
await _unitOfWork.SaveChangesAsync();
await _unitOfWork.CommitTransactionAsync();

// ❌ Wrong - using different instances
await _unitOfWork1.BeginTransactionAsync();
await _unitOfWork2.Users.AddAsync(user); // Different instance!
```

### Soft Delete Not Working

**Problem**: Soft delete is not filtering deleted records.

**Solution**: Ensure your specific repository overrides the query methods (`GetAllAsync`, `FindAsync`, etc.) to filter `IsDeleted`:

```csharp
public override async Task<IEnumerable<T>> GetAllAsync()
{
  return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
}
```

## Related Documentation

- [Error Handling](docs/ERROR_HANDLING.md) - How exceptions are handled in the application
- [Authentication](docs/AUTHENTICATION.md) - Authentication system documentation
- [Permissions and Roles](docs/PERMISSIONS_AND_ROLES.md) - Role and permission management
