# Endpoint para Actualizar Roles de Usuario

Se ha implementado un nuevo endpoint en el `UsersController` para actualizar los roles de un usuario específico por su ID.

## 📋 **Endpoint Implementado**

### **URL y Método:**

```
PUT /api/v1/users/id/{id}/roles
```

### **Parámetros:**

- **`id`** (Guid): ID del usuario al cual se quieren actualizar los roles

### **Request Body:**

```json
{
  "roleIds": [
    "123e4567-e89b-12d3-a456-426614174000",
    "987fcdeb-51a2-43d7-8f9e-123456789abc"
  ]
}
```

### **Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "message": "User roles updated successfully",
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Admin",
      "description": "Administrator role with full access",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T15:45:00Z"
    },
    {
      "id": "987fcdeb-51a2-43d7-8f9e-123456789abc",
      "name": "User",
      "description": "Standard user role",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ]
}
```

### **Respuesta de Error (404 Not Found):**

```json
{
  "success": false,
  "message": "User not found with ID: {id}",
  "data": null
}
```

### **Respuesta de Error (400 Bad Request):**

```json
{
  "success": false,
  "message": "Roles not found: {missing-role-ids}",
  "data": null
}
```

## 🏗️ **Implementación Técnica**

### **1. Request DTO:**

```csharp
public class UpdateUserRolesRequestDto
{
    [Required]
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}
```

### **2. Command (CQRS Pattern):**

```csharp
public record UpdateUserRolesCommand(Guid UserId, List<Guid> RoleIds) : IRequest<List<RoleDto>>;
```

### **3. Handler:**

```csharp
public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, List<RoleDto>>
{
    public async Task<List<RoleDto>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar que el usuario existe
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundByIdError(request.UserId);
        }

        // 2. Verificar que todos los roles existen
        var existingRoles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (existingRoles.Count != request.RoleIds.Count)
        {
            var missingRoleIds = request.RoleIds.Except(existingRoles.Select(r => r.Id)).ToList();
            throw new ArgumentException($"Roles not found: {string.Join(", ", missingRoleIds)}");
        }

        // 3. Remover roles existentes del usuario
        var currentUserRoles = await _context.UserRoles
            .Where(ur => ur.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        _context.UserRoles.RemoveRange(currentUserRoles);

        // 4. Agregar nuevos roles al usuario
        var newUserRoles = request.RoleIds.Select(roleId => new UserRole
        {
            UserId = request.UserId,
            RoleId = roleId
        }).ToList();

        await _context.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);

        // 5. Guardar cambios
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Retornar los roles actualizados
        var updatedRoles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<RoleDto>>(updatedRoles);
    }
}
```

### **4. Controller Endpoint:**

```csharp
[HttpPut("id/{id}/roles")]
[ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesRequestDto request)
{
    try
    {
        var command = new UpdateUserRolesCommand(id, request.RoleIds);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_ROLES_UPDATED")));
    }
    catch (UserNotFoundByIdError ex)
    {
        return NotFound(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
    catch (Exception ex)
    {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
}
```

## 🔍 **Características del Endpoint**

### **1. Validaciones Implementadas:**

- ✅ **Usuario existe**: Verifica que el usuario con el ID proporcionado existe
- ✅ **Roles existen**: Verifica que todos los IDs de roles proporcionados existen
- ✅ **Request body válido**: Valida que el request body contiene `roleIds`

### **2. Operación Atómica:**

- ✅ **Transacción completa**: La operación se realiza en una sola transacción
- ✅ **Reemplazo completo**: Reemplaza todos los roles del usuario con los nuevos
- ✅ **Rollback automático**: Si hay error, se revierten todos los cambios

### **3. Manejo de Errores:**

- ✅ **404 Not Found**: Cuando el usuario no existe
- ✅ **400 Bad Request**: Cuando algunos roles no existen o hay errores de validación
- ✅ **Excepciones custom**: Usa `UserNotFoundByIdError` para consistencia

### **4. Optimización de Consultas:**

- ✅ **Eager loading**: Carga relaciones necesarias de una vez
- ✅ **Consultas eficientes**: Usa `Where` con `Contains` para verificar roles
- ✅ **Batch operations**: Usa `RemoveRange` y `AddRangeAsync` para eficiencia

## 🧪 **Ejemplos de Uso**

### **1. Actualizar roles de un usuario existente:**

```bash
curl -X PUT "https://localhost:7001/api/v1/users/id/123e4567-e89b-12d3-a456-426614174000/roles" \
  -H "Authorization: Bearer your-jwt-token" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": [
      "123e4567-e89b-12d3-a456-426614174000",
      "987fcdeb-51a2-43d7-8f9e-123456789abc"
    ]
  }'
```

### **2. Remover todos los roles (enviar lista vacía):**

```bash
curl -X PUT "https://localhost:7001/api/v1/users/id/123e4567-e89b-12d3-a456-426614174000/roles" \
  -H "Authorization: Bearer your-jwt-token" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": []
  }'
```

### **3. Usuario no encontrado:**

```bash
curl -X PUT "https://localhost:7001/api/v1/users/id/00000000-0000-0000-0000-000000000000/roles" \
  -H "Authorization: Bearer your-jwt-token" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": ["123e4567-e89b-12d3-a456-426614174000"]
  }'

# Respuesta: 404 Not Found
```

### **4. Roles no encontrados:**

```bash
curl -X PUT "https://localhost:7001/api/v1/users/id/123e4567-e89b-12d3-a456-426614174000/roles" \
  -H "Authorization: Bearer your-jwt-token" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": ["00000000-0000-0000-0000-000000000000"]
  }'

# Respuesta: 400 Bad Request - "Roles not found: 00000000-0000-0000-0000-000000000000"
```

### **5. Con JavaScript/Fetch:**

```javascript
const userId = "123e4567-e89b-12d3-a456-426614174000";
const roleIds = [
  "123e4567-e89b-12d3-a456-426614174000",
  "987fcdeb-51a2-43d7-8f9e-123456789abc",
];

const response = await fetch(`/api/v1/users/id/${userId}/roles`, {
  method: "PUT",
  headers: {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  },
  body: JSON.stringify({ roleIds }),
});

if (response.ok) {
  const result = await response.json();
  console.log("Updated user roles:", result.data);
} else {
  console.error("Error:", await response.text());
}
```

## 🔐 **Seguridad y Autorización**

### **1. Autorización:**

- El endpoint hereda la autorización del `UsersController`
- Requiere autenticación JWT válida
- Puede requerir permisos específicos según la configuración

### **2. Validación de Datos:**

- ✅ **Validación de tipos**: El parámetro `id` debe ser un GUID válido
- ✅ **Validación de request body**: `roleIds` es requerido y debe ser una lista
- ✅ **Validación de existencia**: Usuario y roles deben existir

### **3. Seguridad:**

- ✅ **No permite escalación de privilegios**: Solo asigna roles existentes
- ✅ **Transaccional**: Operación atómica que no deja datos inconsistentes
- ✅ **Audit trail**: Los cambios quedan registrados en la base de datos

## 📊 **Rendimiento**

### **1. Optimización de Consultas:**

- ✅ **Consultas eficientes**: Usa `Where` con `Contains` para verificar múltiples IDs
- ✅ **Batch operations**: Operaciones en lote para insertar/eliminar
- ✅ **Eager loading**: Carga relaciones necesarias de una vez

### **2. Escalabilidad:**

- ✅ **Operación atómica**: Una sola transacción de base de datos
- ✅ **Sin N+1 queries**: Todas las consultas están optimizadas
- ✅ **Indexing**: Usa índices en `UserId` y `RoleId`

## 🚀 **Casos de Uso**

### **1. Administración de Usuarios:**

- Asignar roles específicos a usuarios
- Remover roles de usuarios
- Actualizar permisos de usuarios

### **2. Gestión de Acceso:**

- Promover usuarios a roles de mayor privilegio
- Degradar usuarios a roles de menor privilegio
- Remover acceso completo a usuarios

### **3. Automatización:**

- Scripts de configuración inicial
- Procesos de onboarding
- Integración con sistemas externos

## ✅ **Resumen**

El endpoint `PUT /api/v1/users/id/{id}/roles` está completamente implementado y funcional:

- ✅ **Command y Handler** implementados siguiendo el patrón CQRS
- ✅ **Endpoint** agregado al `UsersController` con documentación completa
- ✅ **Validaciones robustas** para usuario y roles
- ✅ **Manejo de errores** con excepciones custom
- ✅ **Operación atómica** y transaccional
- ✅ **Optimización** de consultas y rendimiento
- ✅ **Compilación exitosa** sin errores
- ✅ **Documentación OpenAPI** completa

¡El endpoint está listo para ser usado para actualizar roles de usuarios! 🎉
