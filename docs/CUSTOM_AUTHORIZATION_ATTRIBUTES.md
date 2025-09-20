# Atributos de Autorización Personalizados

Este documento describe cómo usar los nuevos atributos de autorización personalizados que permiten verificar múltiples permisos de manera flexible.

## 🎯 Problema Resuelto

El atributo `[Authorize]` por defecto solo permite una política a la vez y siempre requiere que el usuario tenga TODOS los permisos especificados. Con los nuevos atributos, puedes:

- ✅ Requerir CUALQUIERA de varios permisos (OR lógico)
- ✅ Requerir TODOS los permisos (AND lógico)
- ✅ Combinar diferentes tipos de autorización
- ✅ Tener control granular sobre qué permisos se requieren

## 📋 Atributos Disponibles

### 1. `[RequireAnyPermission]` - Cualquier Permiso

```csharp
[RequireAnyPermission("manage.users", "admin")]
public ActionResult ExampleAnyPermission()
{
    // El usuario necesita tener CUALQUIERA de estos permisos:
    // - manage.users O admin
    return Ok();
}
```

**Casos de uso:**

- Usuarios que pueden gestionar usuarios O ser administradores
- Acceso a funcionalidades que requieren múltiples roles alternativos

### 2. `[RequireAllPermissions]` - Todos los Permisos

```csharp
[RequireAllPermissions("manage.users", "manage.roles")]
public ActionResult ExampleAllPermissions()
{
    // El usuario necesita tener TODOS estos permisos:
    // - manage.users Y manage.roles
    return Ok();
}
```

**Casos de uso:**

- Funcionalidades críticas que requieren múltiples permisos
- Acciones que necesitan autorización de múltiples roles

### 3. `[RequirePermission]` - Modo Flexible

```csharp
// Modo ANY (cualquiera de los permisos)
[RequirePermission(RequirePermissionAttribute.RequireMode.Any, "manage.users", "admin")]
public ActionResult ExampleFlexibleAny()
{
    return Ok();
}

// Modo ALL (todos los permisos)
[RequirePermission(RequirePermissionAttribute.RequireMode.All, "manage.users", "manage.roles", "admin")]
public ActionResult ExampleFlexibleAll()
{
    return Ok();
}
```

**Casos de uso:**

- Cuando necesitas máxima flexibilidad
- Lógica de autorización compleja
- Cuando el modo puede cambiar dinámicamente

## 🔧 Ejemplos Prácticos

### Ejemplo 1: Dashboard Administrativo

```csharp
[HttpGet("admin/dashboard")]
[RequireAnyPermission(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
public ActionResult GetAdminDashboard()
{
    // Solo administradores pueden ver este dashboard
    return Ok();
}
```

### Ejemplo 2: Gestión Completa de Usuarios

```csharp
[HttpDelete("users/{id}")]
[RequireAllPermissions(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
public ActionResult DeleteUser(Guid id)
{
    // Solo usuarios con AMBOS permisos pueden eliminar usuarios
    return Ok();
}
```

### Ejemplo 3: Funcionalidad Multi-Rol

```csharp
[HttpGet("reports")]
[RequireAnyPermission(
    PermissionConstants.ManageUsers,
    PermissionConstants.ManageRoles,
    PermissionConstants.Admin
)]
public ActionResult GetReports()
{
    // Usuarios con cualquiera de estos permisos pueden ver reportes
    return Ok();
}
```

### Ejemplo 4: Combinando Autorización

```csharp
[HttpGet("sensitive-data")]
[Authorize] // Primero verifica autenticación
[RequireAllPermissions(PermissionConstants.Admin, PermissionConstants.ManageUsers)] // Luego permisos
public ActionResult GetSensitiveData()
{
    // Usuario debe estar autenticado Y tener ambos permisos
    return Ok();
}
```

## 🚀 Ventajas de los Nuevos Atributos

### ✅ Flexibilidad

- **ANY**: Permite acceso con cualquiera de los permisos
- **ALL**: Requiere todos los permisos especificados
- **Combinable**: Puedes mezclar con `[Authorize]` tradicional

### ✅ Legibilidad

- **Intuitivo**: El nombre del atributo indica claramente qué hace
- **Explícito**: Los permisos requeridos están claramente definidos
- **Documentado**: Cada atributo tiene documentación XML

### ✅ Mantenibilidad

- **Reutilizable**: Los atributos se pueden usar en cualquier controlador
- **Consistente**: Mismo comportamiento en toda la aplicación
- **Extensible**: Fácil agregar nuevos tipos de verificación

### ✅ Debugging

- **Logs detallados**: Información sobre permisos faltantes
- **Mensajes claros**: Fácil identificar qué permisos se necesitan
- **Trazabilidad**: Registro de intentos de acceso fallidos

## 📊 Comparación con `[Authorize]` Tradicional

| Aspecto                | `[Authorize]` Tradicional | Nuevos Atributos             |
| ---------------------- | ------------------------- | ---------------------------- |
| **Múltiples permisos** | ❌ Solo uno por vez       | ✅ Múltiples permisos        |
| **Lógica OR**          | ❌ No soportado           | ✅ `[RequireAnyPermission]`  |
| **Lógica AND**         | ❌ Limitado               | ✅ `[RequireAllPermissions]` |
| **Flexibilidad**       | ❌ Rígido                 | ✅ Muy flexible              |
| **Legibilidad**        | ⚠️ Políticas predefinidas | ✅ Explícito en el código    |
| **Mantenimiento**      | ⚠️ Requiere configuración | ✅ Autocontenido             |

## 🔍 Casos de Uso Comunes

### 1. **Gestión de Usuarios**

```csharp
// Solo administradores pueden crear usuarios
[HttpPost]
[RequireAllPermissions(PermissionConstants.ManageUsers, PermissionConstants.Admin)]

// Moderadores o administradores pueden ver usuarios
[HttpGet]
[RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
```

### 2. **Reportes y Analytics**

```csharp
// Cualquier rol de gestión puede ver reportes básicos
[HttpGet("basic")]
[RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.ManageRoles)]

// Solo super administradores pueden ver reportes avanzados
[HttpGet("advanced")]
[RequireAllPermissions(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
```

### 3. **Configuración del Sistema**

```csharp
// Configuración básica: admin o superadmin
[HttpPut("basic-config")]
[RequireAnyPermission(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]

// Configuración crítica: requiere ambos permisos
[HttpPut("critical-config")]
[RequireAllPermissions(PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
```

## 🛠️ Implementación Técnica

Los atributos implementan `IAsyncAuthorizationFilter` y:

1. **Verifican autenticación**: Si el usuario no está autenticado, retorna `UnauthorizedResult`
2. **Extraen permisos**: Obtienen los permisos del JWT token del usuario
3. **Aplican lógica**: Verifican según el modo (ANY/ALL)
4. **Retornan resultado**: `ForbidResult` si no tiene permisos, continúa si los tiene
5. **Registran logs**: Información detallada para debugging

## 📝 Migración desde `[Authorize]` Tradicional

### Antes:

```csharp
[Authorize(Policy = "manage.users.or.admin")] // Política predefinida
public ActionResult SomeAction()
```

### Después:

```csharp
[RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
public ActionResult SomeAction()
```

**Ventajas de la migración:**

- ✅ Más explícito y claro
- ✅ No requiere configuración previa en `Program.cs`
- ✅ Permisos visibles directamente en el código
- ✅ Más fácil de mantener y entender
