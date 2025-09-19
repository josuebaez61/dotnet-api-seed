# User Permissions Endpoint

## Overview

This document describes the implementation of the user permissions endpoint that allows retrieving all permissions for a specific user through their roles.

## Endpoint

### GET /api/v1/users/id/{id}/permissions

Retrieves all permissions for a specific user through their roles.

#### Parameters

- `id` (Guid, required): The unique identifier of the user

#### Response

**Success (200 OK):**

```json
{
  "success": true,
  "message": "User permissions retrieved successfully",
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "ReadUsers",
      "description": "Permission to read user information",
      "resource": "Users",
      "action": "Read",
      "module": "UserManagement",
      "createdAt": "2024-01-01T00:00:00Z",
      "lastModifiedAt": "2024-01-01T00:00:00Z"
    },
    {
      "id": "123e4567-e89b-12d3-a456-426614174001",
      "name": "CreateUsers",
      "description": "Permission to create new users",
      "resource": "Users",
      "action": "Create",
      "module": "UserManagement",
      "createdAt": "2024-01-01T00:00:00Z",
      "lastModifiedAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

**Error (404 Not Found):**

```json
{
  "success": false,
  "message": "User not found with ID: 123e4567-e89b-12d3-a456-426614174000"
}
```

**Error (400 Bad Request):**

```json
{
  "success": false,
  "message": "An error occurred while retrieving user permissions"
}
```

## Implementation Details

### Architecture

The endpoint follows the CQRS pattern:

1. **Query**: `GetUserPermissionsQuery`
2. **Handler**: `GetUserPermissionsQueryHandler`
3. **Service**: `IPermissionService` with implementation `PermissionService`

### Permission Resolution Flow

```
User -> UserRoles -> Role -> RolePermissions -> Permission
```

The permission resolution follows this path:

1. User has multiple UserRoles
2. Each UserRole has a Role
3. Each Role has multiple RolePermissions
4. Each RolePermission has a Permission

### Key Components

#### 1. PermissionService

The `PermissionService` provides efficient methods to work with user permissions:

```csharp
public interface IPermissionService
{
    Task<ICollection<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionName);
    Task<bool> UserHasAnyPermissionAsync(Guid userId, params string[] permissionNames);
    Task<ICollection<User>> GetUsersWithPermissionAsync(string permissionName);
}
```

#### 2. User Entity

The `User` entity includes a computed property for permissions:

```csharp
[NotMapped]
public virtual ICollection<Permission> Permissions =>
    UserRoles.SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission)).ToList();
```

**Note**: This property is marked with `[NotMapped]` because it's a computed property that doesn't exist in the database.

#### 3. Database Configuration

The `UserConfiguration` includes:

- Relationship configuration between User and UserRoles
- Relationship configuration between User and Roles (many-to-many)
- Query filter for active users only

## Usage Examples

### Basic Usage

```csharp
// Get all permissions for a user
var permissions = await _permissionService.GetUserPermissionsAsync(userId);

// Check if user has specific permission
var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "ReadUsers");

// Check if user has any of multiple permissions
var hasAnyPermission = await _permissionService.UserHasAnyPermissionAsync(
    userId,
    "ReadUsers",
    "CreateUsers",
    "UpdateUsers"
);
```

### Advanced Queries

```csharp
// Get all users with a specific permission
var usersWithPermission = await _permissionService.GetUsersWithPermissionAsync("AdminAccess");

// Using LINQ with the computed property (after loading user with roles)
var user = await _context.Users
    .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
    .FirstOrDefaultAsync(u => u.Id == userId);

var userPermissions = user.Permissions; // This uses the computed property
```

## Performance Considerations

### Efficient Queries

The `PermissionService` uses optimized LINQ queries that translate to efficient SQL:

```sql
-- Example SQL generated for GetUserPermissionsAsync
SELECT DISTINCT p.*
FROM users u
INNER JOIN user_roles ur ON u.id = ur.user_id
INNER JOIN role_permissions rp ON ur.role_id = rp.role_id
INNER JOIN permissions p ON rp.permission_id = p.id
WHERE u.id = @userId
```

### Caching Considerations

For high-traffic applications, consider implementing caching:

```csharp
public class CachedPermissionService : IPermissionService
{
    private readonly IPermissionService _permissionService;
    private readonly IMemoryCache _cache;

    public async Task<ICollection<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        var cacheKey = $"user_permissions_{userId}";

        if (_cache.TryGetValue(cacheKey, out ICollection<Permission> cachedPermissions))
        {
            return cachedPermissions;
        }

        var permissions = await _permissionService.GetUserPermissionsAsync(userId);
        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(15));

        return permissions;
    }
}
```

## Testing

### Unit Tests

```csharp
[Test]
public async Task GetUserPermissions_ShouldReturnUserPermissions()
{
    // Arrange
    var userId = Guid.NewGuid();
    var expectedPermissions = new List<Permission>
    {
        new Permission { Name = "ReadUsers" },
        new Permission { Name = "CreateUsers" }
    };

    _mockContext.Setup(x => x.Users)
        .Returns(CreateMockUsersWithPermissions(userId, expectedPermissions));

    // Act
    var result = await _permissionService.GetUserPermissionsAsync(userId);

    // Assert
    Assert.That(result, Has.Count.EqualTo(2));
    Assert.That(result.Any(p => p.Name == "ReadUsers"), Is.True);
}
```

### Integration Tests

```csharp
[Test]
public async Task GetUserPermissions_Endpoint_ShouldReturnUserPermissions()
{
    // Arrange
    var userId = await CreateTestUserWithRolesAndPermissions();
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/api/v1/users/id/{userId}/permissions");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponse<List<PermissionDto>>>(content);

    result.Success.Should().BeTrue();
    result.Data.Should().NotBeEmpty();
}
```

## Security Considerations

1. **Authorization**: Ensure proper authorization checks before allowing access to user permissions
2. **Input Validation**: Validate user IDs to prevent unauthorized access
3. **Rate Limiting**: Implement rate limiting for permission-related endpoints
4. **Audit Logging**: Log permission access for security auditing

## Related Documentation

- [User Roles Endpoint](USER_ROLES_ENDPOINT.md)
- [Update User Roles Endpoint](UPDATE_USER_ROLES_ENDPOINT.md)
- [Permission Management](PERMISSION_MANAGEMENT.md)
