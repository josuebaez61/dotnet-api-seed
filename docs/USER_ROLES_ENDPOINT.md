# Endpoint para Obtener Roles de Usuario

Se ha implementado un nuevo endpoint en el `UsersController` para obtener todos los roles de un usuario específico por su ID.

## 📋 **Endpoint Implementado**

### **URL y Método:**

```
GET /api/v1/users/id/{id}/roles
```

### **Parámetros:**

- **`id`** (Guid): ID del usuario del cual se quieren obtener los roles

### **Respuesta Exitosa (200 OK):**

```json
{
  "success": true,
  "message": "User roles retrieved successfully",
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
  "message": "User with ID {id} not found.",
  "data": null
}
```

### **Respuesta de Error (400 Bad Request):**

```json
{
  "success": false,
  "message": "Error message details",
  "data": null
}
```

## 🏗️ **Implementación Técnica**

### **1. Query (CQRS Pattern):**

```csharp
// GetUserRolesQuery.cs
public record GetUserRolesQuery(Guid UserId) : IRequest<List<RoleDto>>;
```

### **2. Handler:**

```csharp
// GetUserRolesQueryHandler.cs
public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<RoleDto>>
{
    public async Task<List<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundByIdError(request.UserId);
        }

        var roles = user.UserRoles.Select(ur => ur.Role).ToList();
        return _mapper.Map<List<RoleDto>>(roles);
    }
}
```

### **3. Controller Endpoint:**

```csharp
[HttpGet("id/{id}/roles")]
[ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> GetUserRoles(Guid id)
{
    try
    {
        var query = new GetUserRolesQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_ROLES_RETRIEVED")));
    }
    catch (UserNotFoundByIdError ex)
    {
        return NotFound(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
    catch (Exception ex)
    {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
}
```

## 🔍 **Características del Endpoint**

### **1. Manejo de Relaciones:**

- ✅ Utiliza `Include` y `ThenInclude` para cargar las relaciones `UserRoles` y `Role`
- ✅ Evita el problema N+1 mediante eager loading
- ✅ Mapea automáticamente a DTOs usando AutoMapper

### **2. Manejo de Errores:**

- ✅ **404 Not Found**: Cuando el usuario no existe (usando `UserNotFoundByIdError`)
- ✅ **400 Bad Request**: Para otros errores (validación, base de datos, etc.)
- ✅ Mensajes de error descriptivos y localizados
- ✅ Excepciones custom para mejor manejo de errores específicos

### **3. Patrón CQRS:**

- ✅ Separación clara entre Query y Handler
- ✅ Uso de MediatR para desacoplar el controller del handler
- ✅ Fácil testing y mantenimiento

### **4. Documentación OpenAPI:**

- ✅ Documentación XML completa con `<summary>` y `<param>`
- ✅ Especificación de tipos de respuesta con `ProducesResponseType`
- ✅ Documentación automática en Swagger/OpenAPI

## 🧪 **Ejemplos de Uso**

### **1. Obtener roles de un usuario existente:**

```bash
curl -X GET "https://localhost:7001/api/v1/users/id/123e4567-e89b-12d3-a456-426614174000/roles" \
  -H "Authorization: Bearer your-jwt-token"
```

### **2. Usuario no encontrado:**

```bash
curl -X GET "https://localhost:7001/api/v1/users/id/00000000-0000-0000-0000-000000000000/roles" \
  -H "Authorization: Bearer your-jwt-token"

# Respuesta: 404 Not Found
```

### **3. Con JavaScript/Fetch:**

```javascript
const userId = "123e4567-e89b-12d3-a456-426614174000";
const response = await fetch(`/api/v1/users/id/${userId}/roles`, {
  method: "GET",
  headers: {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  },
});

if (response.ok) {
  const result = await response.json();
  console.log("User roles:", result.data);
} else {
  console.error("Error:", await response.text());
}
```

## 🚨 **Manejo de Excepciones Custom**

### **1. UserNotFoundByIdError:**

```csharp
public class UserNotFoundByIdError : ApplicationException
{
    public UserNotFoundByIdError(Guid userId)
        : base("USER_NOT_FOUND", $"User not found with ID: {userId}", new { UserId = userId })
    {
    }
}
```

### **2. Ventajas de las Excepciones Custom:**

- ✅ **Mensajes específicos**: Información detallada sobre el error
- ✅ **Códigos de error**: Identificadores únicos para cada tipo de error
- ✅ **Metadatos**: Información adicional (como el UserId) en el objeto de error
- ✅ **Manejo centralizado**: Fácil identificación y manejo de errores específicos
- ✅ **Localización**: Soporte para mensajes de error localizados

### **3. Flujo de Manejo de Errores:**

1. **Handler** lanza `UserNotFoundByIdError` con el ID del usuario
2. **Controller** captura la excepción específica
3. **Response** devuelve 404 Not Found con mensaje descriptivo
4. **Cliente** recibe información clara sobre el error

## 🔐 **Seguridad y Autorización**

### **1. Autorización:**

- El endpoint hereda la autorización del `UsersController`
- Requiere autenticación JWT válida
- Puede requerir permisos específicos según la configuración

### **2. Validación:**

- ✅ Validación automática del formato GUID del parámetro `id`
- ✅ Verificación de existencia del usuario antes de procesar
- ✅ Manejo seguro de excepciones

## 📊 **Rendimiento**

### **1. Optimización de Consultas:**

- ✅ Una sola consulta a la base de datos con `Include`
- ✅ No hay problema N+1 gracias al eager loading
- ✅ Mapeo eficiente con AutoMapper

### **2. Caching:**

- Puede implementarse caching a nivel de handler si es necesario
- Los roles de usuario no cambian frecuentemente

## 🚀 **Próximos Pasos Sugeridos**

### **1. Testing:**

```csharp
[Test]
public async Task GetUserRoles_WithValidUserId_ReturnsUserRoles()
{
    // Arrange
    var userId = Guid.NewGuid();
    var expectedRoles = new List<Role> { /* roles de prueba */ };

    // Act
    var result = await _handler.Handle(new GetUserRolesQuery(userId), CancellationToken.None);

    // Assert
    Assert.That(result.Count, Is.EqualTo(expectedRoles.Count));
}
```

### **2. Paginación (si es necesario):**

Si un usuario puede tener muchos roles, considerar implementar paginación:

```csharp
public record GetUserRolesPaginatedQuery(Guid UserId, int Page, int PageSize) : IRequest<PaginationResponse<RoleDto>>;
```

### **3. Filtros:**

Agregar filtros por tipo de rol o estado:

```csharp
public record GetUserRolesQuery(Guid UserId, string? RoleType = null, bool? IsActive = null) : IRequest<List<RoleDto>>;
```

## ✅ **Resumen**

El endpoint `GET /api/v1/users/id/{id}/roles` está completamente implementado y funcional:

- ✅ **Query y Handler** implementados siguiendo el patrón CQRS
- ✅ **Endpoint** agregado al `UsersController` con documentación completa
- ✅ **Manejo de errores** robusto con respuestas HTTP apropiadas
- ✅ **Optimización** de consultas con eager loading
- ✅ **Compilación exitosa** sin errores
- ✅ **Documentación OpenAPI** completa

¡El endpoint está listo para ser usado! 🎉
