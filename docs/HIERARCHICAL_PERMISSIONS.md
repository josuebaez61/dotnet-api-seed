# New Simplified Permissions System

This document describes the new simplified permissions system implemented in the Clean Architecture application.

## 🎯 **Overview**

The new permissions system uses a simplified approach with 6 core permissions that cover all system functionality. This eliminates complex hierarchical relationships and makes permission management more straightforward.

## 🏗️ **Architecture**

### **Core Components**

1. **`HierarchicalPermissionService`** - Wraps the base permission service and adds hierarchical logic
2. **`PermissionConstants.NewPermissions`** - Defines the new simplified permission constants
3. **`DatabaseInitializationService`** - Seeds the new permission system

### **How It Works**

The new system uses 6 core permissions that provide clear, non-overlapping access control:

## 📋 **New Permission System**

### **Core Permissions**

```
manage.roles          → Manage roles (create, update, delete, read)
manage.users          → Manage users (create, update, delete, read)
manage.user.roles     → Manage user-role assignments
manage.role.permissions → Manage role-permission assignments
admin                 → Administrative access
superAdmin            → Super administrative access (includes all permissions)
```

### **Permission Hierarchy**

```
superAdmin → All permissions (admin + manage.roles + manage.users + manage.user.roles + manage.role.permissions)
admin → Administrative access
manage.role.permissions → Can assign/remove permissions from roles
manage.user.roles → Can assign/remove roles from users
manage.roles → Can manage roles
manage.users → Can manage users
```

### **Legacy System (Deprecated)**

The old hierarchical system is still supported but marked as deprecated:

```
Users.Write → Users.Read
Users.Update → Users.Read
Users.Delete → Users.Read
Roles.Write → Roles.Read
Permissions.Write → Permissions.Read
System.Admin → System permissions
```

### **Audit Module**

```
Audit.ExportData → Audit.ViewReports
```

## 🚀 **API Endpoints**

### **Get Hierarchical Permissions**

```http
GET /api/v1/permissionhierarchy/hierarchical/{permissionName}
Authorization: Bearer {token}
```

**Example:**

```bash
curl -X GET "http://localhost:5103/api/v1/permissionhierarchy/hierarchical/Users.Write" \
  -H "Authorization: Bearer {token}"
```

**Response:**

```json
{
  "success": true,
  "message": "Hierarchical permissions for Users.Write",
  "data": ["Users.Read"],
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### **Get Parent Permissions**

```http
GET /api/v1/permissionhierarchy/parents/{permissionName}
Authorization: Bearer {token}
```

**Example:**

```bash
curl -X GET "http://localhost:5103/api/v1/permissionhierarchy/parents/Users.Read" \
  -H "Authorization: Bearer {token}"
```

**Response:**

```json
{
  "success": true,
  "message": "Parent permissions for Users.Read",
  "data": [
    "Users.Write",
    "Users.Update",
    "Users.Delete",
    "Users.ViewSensitive",
    "Users.ManageRoles",
    "Users.Manage",
    "System.Admin"
  ],
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### **Validate Hierarchy**

```http
GET /api/v1/permissionhierarchy/validate
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "message": "Permission hierarchy validation passed",
  "data": [],
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### **Get Examples**

```http
GET /api/v1/permissionhierarchy/examples
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "message": "Hierarchical permission examples",
  "data": {
    "description": "Examples of how hierarchical permissions work",
    "examples": [
      {
        "permission": "Users.Write",
        "includes": ["Users.Read"],
        "explanation": "Users.Write automatically includes Users.Read"
      },
      {
        "permission": "Users.Manage",
        "includes": [
          "Users.Read",
          "Users.Write",
          "Users.Update",
          "Users.Delete",
          "Users.ManageRoles",
          "Users.ViewSensitive"
        ],
        "explanation": "Users.Manage includes all user-related permissions"
      }
    ]
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## 💡 **Usage Examples**

### **Frontend Permission Checks**

```typescript
// Instead of checking multiple permissions
const canManageUsers =
  userPermissions.includes("Users.Manage") ||
  userPermissions.includes("Users.Write") ||
  userPermissions.includes("Users.Update") ||
  userPermissions.includes("Users.Delete");

// Now you can just check one permission
const canManageUsers = userPermissions.includes("Users.Manage");
```

### **Backend Authorization**

```csharp
// The hierarchical service automatically includes related permissions
[HttpGet]
[Authorize(Policy = PermissionConstants.Users.Read)]
public async Task<ActionResult> GetUsers()
{
    // This endpoint is accessible to users with:
    // - Users.Read (direct)
    // - Users.Write (includes Users.Read)
    // - Users.Update (includes Users.Read)
    // - Users.Delete (includes Users.Read)
    // - Users.Manage (includes Users.Read)
    // - System.Admin (includes Users.Read)
}
```

### **Role Assignment**

```csharp
// When assigning Users.Write to a role, the user automatically gets Users.Read
await permissionService.AssignPermissionToRoleAsync(roleId, usersWritePermissionId);

// The role now has both Users.Write and Users.Read permissions
var rolePermissions = await permissionService.GetRolePermissionsAsync(roleId);
// Returns: [Users.Write, Users.Read]
```

## 🔧 **Configuration**

### **Adding New Hierarchical Relationships**

To add new hierarchical relationships, modify `HierarchicalPermissionConfiguration.cs`:

```csharp
[PermissionConstants.Users.SuperAdmin] = new List<string>
{
    PermissionConstants.Users.Manage,
    PermissionConstants.Roles.Manage,
    PermissionConstants.Permissions.Read
}
```

### **Custom Hierarchies**

You can create custom hierarchies for any module:

```csharp
[PermissionConstants.Products.Manage] = new List<string>
{
    PermissionConstants.Products.Read,
    PermissionConstants.Products.Write,
    PermissionConstants.Products.Update,
    PermissionConstants.Products.Delete
}
```

## 🎯 **Benefits**

### **1. Simplified Permission Management**

- Assign one permission instead of multiple related permissions
- Automatic inclusion of related permissions
- Reduced complexity in role management

### **2. Consistent Permission Structure**

- Clear hierarchy relationships
- Predictable permission behavior
- Easy to understand and maintain

### **3. Frontend Simplification**

- Single permission check instead of multiple
- Cleaner UI logic
- Better user experience

### **4. Backend Efficiency**

- Automatic permission expansion
- Consistent authorization behavior
- Reduced code duplication

## 🔒 **Security Considerations**

### **Permission Inheritance**

- Hierarchical permissions are **additive only**
- Cannot remove permissions through hierarchy
- Must explicitly remove permissions to revoke access

### **Validation**

- All permissions in hierarchy must exist in the system
- Validation endpoint ensures configuration integrity
- Prevents broken permission relationships

### **Audit Trail**

- All permissions (including hierarchical) are logged
- Clear visibility into effective permissions
- Easy to track permission changes

## 🧪 **Testing**

### **Unit Tests**

```csharp
[Test]
public async Task GetUserPermissionsAsync_WithWritePermission_IncludesRead()
{
    // Arrange
    var user = await CreateUserWithPermission("Users.Write");

    // Act
    var permissions = await hierarchicalPermissionService.GetUserPermissionsAsync(user.Id);

    // Assert
    Assert.Contains(permissions, p => p.Name == "Users.Write");
    Assert.Contains(permissions, p => p.Name == "Users.Read");
}

[Test]
public async Task GetRolePermissionsAsync_WithManagePermission_IncludesAllUserPermissions()
{
    // Arrange
    var role = await CreateRoleWithPermission("Users.Manage");

    // Act
    var permissions = await hierarchicalPermissionService.GetRolePermissionsAsync(role.Id);

    // Assert
    Assert.Contains(permissions, p => p.Name == "Users.Manage");
    Assert.Contains(permissions, p => p.Name == "Users.Read");
    Assert.Contains(permissions, p => p.Name == "Users.Write");
    Assert.Contains(permissions, p => p.Name == "Users.Update");
    Assert.Contains(permissions, p => p.Name == "Users.Delete");
}
```

### **Integration Tests**

```csharp
[Test]
public async Task PermissionHierarchyController_GetHierarchicalPermissions_ReturnsCorrectPermissions()
{
    // Act
    var response = await _client.GetAsync("/api/v1/permissionhierarchy/hierarchical/Users.Write");

    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponse<List<string>>>(content);

    Assert.Contains("Users.Read", result.Data);
}
```

## 📊 **Performance**

### **Caching**

- Hierarchical permissions are calculated on-demand
- Consider caching for high-traffic applications
- Virtual permissions are lightweight objects

### **Memory Usage**

- Minimal memory overhead
- Virtual permissions are not persisted
- Efficient permission expansion algorithm

## 🔮 **Future Enhancements**

### **Planned Features**

1. **Permission Groups** - Logical groupings of permissions
2. **Conditional Hierarchies** - Context-dependent permission inheritance
3. **Permission Templates** - Predefined permission sets for common roles
4. **Audit Dashboard** - Visual representation of permission hierarchies

### **Extensibility**

- Easy to add new permission modules
- Configurable hierarchy relationships
- Plugin architecture for custom logic

## 📚 **Related Documentation**

- [Permissions and Roles System](PERMISSIONS_AND_ROLES.md)
- [Authentication and Authorization](AUTHENTICATION.md)
- [API Documentation](API.md)

## 🎉 **Conclusion**

The hierarchical permissions system provides a powerful and flexible way to manage permissions in your application. It simplifies permission management while maintaining security and providing clear, predictable behavior.

By using hierarchical permissions, you can:

- ✅ Reduce complexity in permission assignment
- ✅ Ensure consistent permission relationships
- ✅ Simplify frontend permission checks
- ✅ Maintain security best practices
- ✅ Scale permission management easily
