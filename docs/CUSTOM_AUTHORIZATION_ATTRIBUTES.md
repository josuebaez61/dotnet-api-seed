# Custom Authorization Attributes

This document describes how to use the custom authorization attributes that allow flexible verification of multiple permissions.

## 🎯 Problem Solved

The default `[Authorize]` attribute only allows one policy at a time and always requires the user to have ALL specified permissions. With the new attributes, you can:

- ✅ Require ANY of multiple permissions (OR logic)
- ✅ Require ALL permissions (AND logic)
- ✅ Combine different types of authorization
- ✅ Have granular control over which permissions are required

## 📋 Available Attributes

### 1. `[RequireAnyPermission]` - Any Permission

```csharp
[RequireAnyPermission("manage.users", "admin")]
public ActionResult ExampleAnyPermission()
{
    // User needs to have ANY of these permissions:
    // - manage.users OR admin
    return Ok();
}
```

**Use cases:**

- Users who can manage users OR be administrators
- Access to features that require multiple alternative roles

### 2. `[RequireAllPermissions]` - All Permissions

```csharp
[RequireAllPermissions("manage.users", "manage.roles")]
public ActionResult ExampleAllPermissions()
{
    // User needs to have ALL these permissions:
    // - manage.users AND manage.roles
    return Ok();
}
```

**Use cases:**

- Critical features that require multiple permissions
- Actions that need authorization from multiple roles

### 3. `[RequirePermission]` - Flexible Mode

```csharp
// ANY mode (any of the permissions)
[RequirePermission(RequirePermissionAttribute.RequireMode.Any, "manage.users", "admin")]
public ActionResult ExampleFlexibleAny()
{
    return Ok();
}

// ALL mode (all permissions)
[RequirePermission(RequirePermissionAttribute.RequireMode.All, "manage.users", "manage.roles", "admin")]
public ActionResult ExampleFlexibleAll()
{
    return Ok();
}
```

**Use cases:**

- When you need maximum flexibility
- Complex authorization logic
- When the mode can change dynamically

## 🔧 Practical Examples

### Example 1: Administrative Dashboard

```csharp
[HttpGet("admin/dashboard")]
[RequireAnyPermission(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
public ActionResult GetAdminDashboard()
{
    // Only administrators can view this dashboard
    return Ok();
}
```

### Example 2: Complete User Management

```csharp
[HttpDelete("users/{id}")]
[RequireAllPermissions(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
public ActionResult DeleteUser(Guid id)
{
    // Only users with BOTH permissions can delete users
    return Ok();
}
```

### Example 3: Multi-Role Functionality

```csharp
[HttpGet("reports")]
[RequireAnyPermission(
    PermissionConstants.ManageUsers,
    PermissionConstants.ManageRoles,
    PermissionConstants.Admin
)]
public ActionResult GetReports()
{
    // Users with any of these permissions can view reports
    return Ok();
}
```

### Example 4: Combining Authorization

```csharp
[HttpGet("sensitive-data")]
[Authorize] // First verifies authentication
[RequireAllPermissions(PermissionConstants.Admin, PermissionConstants.ManageUsers)] // Then permissions
public ActionResult GetSensitiveData()
{
    // User must be authenticated AND have both permissions
    return Ok();
}
```

## 🚀 Advantages of the New Attributes

### ✅ Flexibility

- **ANY**: Allows access with any of the permissions
- **ALL**: Requires all specified permissions
- **Combinable**: Can be mixed with traditional `[Authorize]`

### ✅ Readability

- **Intuitive**: The attribute name clearly indicates what it does
- **Explicit**: Required permissions are clearly defined
- **Documented**: Each attribute has XML documentation

### ✅ Maintainability

- **Reusable**: Attributes can be used in any controller
- **Consistent**: Same behavior throughout the application
- **Extensible**: Easy to add new verification types

### ✅ Debugging

- **Detailed logs**: Information about missing permissions
- **Clear messages**: Easy to identify which permissions are needed
- **Traceability**: Logging of failed access attempts

## 📊 Comparison with Traditional `[Authorize]`

| Aspect                   | Traditional `[Authorize]` | New Attributes               |
| ------------------------ | ------------------------- | ---------------------------- |
| **Multiple permissions** | ❌ Only one at a time     | ✅ Multiple permissions      |
| **OR logic**             | ❌ Not supported          | ✅ `[RequireAnyPermission]`  |
| **AND logic**            | ❌ Limited                | ✅ `[RequireAllPermissions]` |
| **Flexibility**          | ❌ Rigid                  | ✅ Very flexible             |
| **Readability**          | ⚠️ Predefined policies    | ✅ Explicit in code          |
| **Maintenance**          | ⚠️ Requires configuration | ✅ Self-contained            |

## 🔍 Common Use Cases

### 1. **User Management**

```csharp
// Only administrators can create users
[HttpPost]
[RequireAllPermissions(PermissionConstants.ManageUsers, PermissionConstants.Admin)]

// Moderators or administrators can view users
[HttpGet]
[RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
```

### 2. **Reports and Analytics**

```csharp
// Any management role can view basic reports
[HttpGet("basic")]
[RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.ManageRoles)]

// Only super administrators can view advanced reports
[HttpGet("advanced")]
[RequireAllPermissions(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
```

### 3. **System Configuration**

```csharp
// Basic configuration: admin or superadmin
[HttpPut("basic-config")]
[RequireAnyPermission(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]

// Critical configuration: requires both permissions
[HttpPut("critical-config")]
[RequireAllPermissions(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
```

## 🛠️ Technical Implementation

The attributes implement `IAsyncAuthorizationFilter` and:

1. **Verify authentication**: If the user is not authenticated, returns `UnauthorizedResult`
2. **Extract permissions**: Gets permissions from the user's JWT token
3. **Apply logic**: Verifies according to the mode (ANY/ALL)
4. **Return result**: `ForbidResult` if no permissions, continues if it has them
5. **Log information**: Detailed information for debugging

## 📝 Migration from Traditional `[Authorize]`

### Before:

```csharp
[Authorize(Policy = "manage.users.or.admin")] // Predefined policy
public ActionResult SomeAction()
```

### After:

```csharp
[RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
public ActionResult SomeAction()
```

**Migration advantages:**

- ✅ More explicit and clear
- ✅ No prior configuration required in `Program.cs`
- ✅ Permissions visible directly in code
- ✅ Easier to maintain and understand
