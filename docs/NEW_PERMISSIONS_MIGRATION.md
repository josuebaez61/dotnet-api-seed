# Migración al Nuevo Sistema de Permisos

Este documento describe cómo migrar del sistema de permisos anterior al nuevo sistema simplificado.

## 🎯 **Resumen del Cambio**

### **Sistema Anterior (Legacy)**

```
Users.Read, Users.Write, Users.Delete
Roles.Read, Roles.Write
Permissions.Read, Permissions.Write
System.Admin, System.ViewLogs, etc.
```

### **Nuevo Sistema Simplificado**

```
manage.roles          → Manage roles (create, update, delete, read)
manage.users          → Manage users (create, update, delete, read)
manage.user.roles     → Manage user-role assignments
manage.role.permissions → Manage role-permission assignments
admin                 → Administrative access
superAdmin            → Super administrative access (includes all permissions)
```

## 🔄 **Compatibilidad**

### **Retrocompatibilidad**

- ✅ Los permisos legacy siguen funcionando
- ✅ Las políticas de autorización legacy están disponibles
- ✅ Los controladores pueden usar ambos sistemas
- ⚠️ Los permisos legacy están marcados como `[Obsolete]`

### **Migración Gradual**

El sistema permite una migración gradual:

1. **Fase 1**: Ambos sistemas funcionan en paralelo
2. **Fase 2**: Actualizar controladores a nuevos permisos
3. **Fase 3**: Remover permisos legacy (futuro)

## 📋 **Mapeo de Permisos**

### **Para Administradores**

```csharp
// Antes
[Authorize(Policy = PermissionConstants.Users.Read)]
[Authorize(Policy = PermissionConstants.Users.Write)]
[Authorize(Policy = PermissionConstants.Roles.Read)]
[Authorize(Policy = PermissionConstants.Roles.Write)]

// Ahora
[Authorize(Policy = PermissionConstants.NewPermissions.ManageUsers)]
[Authorize(Policy = PermissionConstants.NewPermissions.ManageRoles)]
```

### **Para Gestión de Roles de Usuario**

```csharp
// Antes
[Authorize(Policy = PermissionConstants.Users.ManageRoles)]

// Ahora
[Authorize(Policy = PermissionConstants.NewPermissions.ManageUserRoles)]
```

### **Para Gestión de Permisos de Roles**

```csharp
// Antes
[Authorize(Policy = PermissionConstants.Permissions.Write)]

// Ahora
[Authorize(Policy = PermissionConstants.NewPermissions.ManageRolePermissions)]
```

## 🗄️ **Base de Datos**

### **Nuevos Permisos Creados**

El seeder ahora crea ambos conjuntos de permisos:

#### **Nuevos Permisos**

- `manage.roles` (ID: 32edea54-6b49-4f4f-8257-aa1992f23c28)
- `manage.users` (ID: e1d015ea-0d8a-42b5-a0c1-237a8e018999)
- `manage.user.roles` (ID: 3c883108-b93d-4142-acc8-bbd67f694fb1)
- `manage.role.permissions` (ID: 02033fae-fccd-4a7f-8cea-06a43178ec73)
- `admin` (ID: 082a40e0-2ff4-4c05-a078-4dfaf778172f)
- `superAdmin` (ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890)

#### **Permisos Legacy (Mantenidos)**

- `Users.Read`, `Users.Write`, `Users.Delete`
- `Roles.Read`, `Roles.Write`
- `Permissions.Read`, `Permissions.Write`

### **Asignación de Roles**

El rol `Admin` ahora tiene **todos** los permisos (nuevos y legacy) para garantizar acceso completo.

## 🔧 **Actualización de Código**

### **1. Constantes de Permisos**

```csharp
// Antes
PermissionConstants.Users.Read
PermissionConstants.Roles.Write

// Ahora
PermissionConstants.NewPermissions.ManageUsers
PermissionConstants.NewPermissions.ManageRoles
```

### **2. Políticas de Autorización**

```csharp
// Program.cs - Ya actualizado
options.AddPolicy("manage.users", policy => policy.RequireClaim("permission", "manage.users"));
options.AddPolicy("manage.roles", policy => policy.RequireClaim("permission", "manage.roles"));
```

### **3. Controladores**

```csharp
// Antes
[Authorize(Policy = PermissionConstants.Users.Read)]

// Ahora
[Authorize(Policy = PermissionConstants.NewPermissions.ManageUsers)]
```

## 🚀 **Cómo Probar**

### **1. Verificar Permisos en Base de Datos**

```sql
SELECT * FROM permissions WHERE name LIKE 'manage.%' OR name IN ('admin', 'superAdmin');
```

### **2. Verificar JWT Token**

El token JWT ahora incluirá los nuevos permisos:

```json
{
  "permission": "manage.users",
  "permission": "manage.roles",
  "permission": "admin"
}
```

### **3. Probar Endpoints**

- ✅ `GET /api/v1/users` → Requiere `manage.users`
- ✅ `GET /api/v1/roles/all` → Requiere `manage.roles`
- ✅ `GET /api/v1/users/id/{id}/roles` → Requiere `manage.user.roles`
- ✅ `GET /api/v1/permissions` → Requiere `manage.role.permissions`

## 📊 **Jerarquía de Permisos**

```
superAdmin
├── admin
├── manage.roles
├── manage.users
├── manage.user.roles
└── manage.role.permissions

admin
└── manage.roles (implícito)

manage.user.roles
└── manage.users (implícito)

manage.role.permissions
└── manage.roles (implícito)
```

## ⚠️ **Consideraciones**

### **Seguridad**

- Los permisos legacy siguen siendo válidos
- No hay degradación de seguridad durante la migración
- Los nuevos permisos son más granulares y específicos

### **Rendimiento**

- El número de permisos se redujo de ~15 a 6
- Menos complejidad en la lógica de autorización
- Mejor rendimiento en verificaciones de permisos

### **Mantenimiento**

- Código más limpio y fácil de entender
- Menos duplicación de lógica de permisos
- Mejor separación de responsabilidades

## 🔮 **Próximos Pasos**

1. **Monitorear**: Verificar que todos los endpoints funcionen correctamente
2. **Actualizar Frontend**: Cambiar las verificaciones de permisos en el cliente
3. **Documentar**: Actualizar documentación de API con nuevos permisos
4. **Limpiar**: Eventualmente remover permisos legacy (en versión futura)

## 📞 **Soporte**

Si encuentras algún problema durante la migración:

1. Verifica que los permisos estén correctamente asignados al rol Admin
2. Revisa los logs de autenticación
3. Confirma que el JWT token incluya los permisos esperados
4. Consulta la documentación de permisos jerárquicos
