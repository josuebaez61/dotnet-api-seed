# Reorganización de Excepciones de Usuario

Se ha creado un archivo dedicado `UserExceptions.cs` para organizar mejor las excepciones relacionadas con usuarios, separándolas de las excepciones de autenticación.

## 📁 **Estructura de Archivos**

### **Antes:**

```
Application/Common/Exceptions/
├── AuthExceptions.cs (todas las excepciones)
```

### **Después:**

```
Application/Common/Exceptions/
├── AuthExceptions.cs (excepciones de autenticación)
├── UserExceptions.cs (excepciones de usuarios)
```

## 🔄 **Excepciones Movidas**

### **De `AuthExceptions.cs` a `UserExceptions.cs`:**

#### **1. UserNotFoundError:**

```csharp
public class UserNotFoundError : ApplicationException
{
    public UserNotFoundError(string emailOrUsername)
        : base("USER_NOT_FOUND", $"User not found: {emailOrUsername}", new { EmailOrUsername = emailOrUsername })
    {
    }
}
```

#### **2. UserNotFoundByIdError:**

```csharp
public class UserNotFoundByIdError : ApplicationException
{
    public UserNotFoundByIdError(Guid userId)
        : base("USER_NOT_FOUND", $"User not found with ID: {userId}", new { UserId = userId })
    {
    }
}
```

#### **3. UserAlreadyExistsError:**

```csharp
public class UserAlreadyExistsError : ApplicationException
{
    public UserAlreadyExistsError(string field, string value)
        : base("USER_ALREADY_EXISTS", $"User already exists with {field}: {value}", new { Field = field, Value = value })
    {
    }
}
```

#### **4. AccountDeactivatedError:**

```csharp
public class AccountDeactivatedError : ApplicationException
{
    public AccountDeactivatedError(string userId)
        : base("ACCOUNT_DEACTIVATED", $"Account is deactivated for user: {userId}", new { UserId = userId })
    {
    }
}
```

## 📋 **Excepciones en UserExceptions.cs**

### **Excepciones Principales:**

- ✅ `UserNotFoundError` - Usuario no encontrado por email/username
- ✅ `UserNotFoundByIdError` - Usuario no encontrado por ID
- ✅ `UserAlreadyExistsError` - Usuario ya existe
- ✅ `AccountDeactivatedError` - Cuenta desactivada

### **Excepciones de Estado:**

- ✅ `UserNotActiveError` - Usuario inactivo
- ✅ `UserEmailNotConfirmedError` - Email no confirmado
- ✅ `UserMustChangePasswordError` - Debe cambiar contraseña

### **Excepciones de Códigos:**

- ✅ `UserPasswordResetCodeExpiredError` - Código de reset expirado
- ✅ `UserPasswordResetCodeUsedError` - Código de reset ya usado
- ✅ `UserEmailVerificationCodeExpiredError` - Código de verificación expirado
- ✅ `UserEmailVerificationCodeUsedError` - Código de verificación ya usado

## 🔧 **Excepciones que Permanecen en AuthExceptions.cs**

### **Excepciones de Autenticación:**

- ✅ `InvalidCredentialsError` - Credenciales inválidas
- ✅ `InvalidPasswordError` - Contraseña inválida
- ✅ `PasswordTooWeakError` - Contraseña muy débil
- ✅ `TokenExpiredError` - Token expirado
- ✅ `InvalidTokenError` - Token inválido
- ✅ `RefreshTokenExpiredError` - Refresh token expirado
- ✅ `InvalidRefreshTokenError` - Refresh token inválido

## 🎯 **Ventajas de la Reorganización**

### **1. Separación de Responsabilidades:**

- **AuthExceptions**: Solo excepciones relacionadas con autenticación/autorización
- **UserExceptions**: Solo excepciones relacionadas con gestión de usuarios

### **2. Mejor Organización:**

- ✅ **Fácil localización**: Las excepciones están donde se esperan
- ✅ **Mantenimiento**: Cambios más fáciles de realizar
- ✅ **Escalabilidad**: Fácil agregar nuevas excepciones por categoría

### **3. Claridad de Código:**

- ✅ **Imports específicos**: Solo importar lo que se necesita
- ✅ **Contexto claro**: El nombre del archivo indica el propósito
- ✅ **Cohesión**: Excepciones relacionadas están juntas

## 📝 **Uso en el Código**

### **1. Import Statements:**

```csharp
// Para excepciones de usuario
using CleanArchitecture.Application.Common.Exceptions;

// Las excepciones están en el mismo namespace, así que no hay cambios
throw new UserNotFoundByIdError(userId);
```

### **2. Handler de Ejemplo:**

```csharp
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
            throw new UserNotFoundByIdError(request.UserId); // ✅ Ahora en UserExceptions.cs
        }

        var roles = user.UserRoles.Select(ur => ur.Role).ToList();
        return _mapper.Map<List<RoleDto>>(roles);
    }
}
```

### **3. Controller de Ejemplo:**

```csharp
[HttpGet("id/{id}/roles")]
public async Task<IActionResult> GetUserRoles(Guid id)
{
    try
    {
        var query = new GetUserRolesQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result));
    }
    catch (UserNotFoundByIdError ex) // ✅ Excepción específica de usuario
    {
        return NotFound(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
    catch (Exception ex)
    {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
    }
}
```

## 🚀 **Próximos Pasos Sugeridos**

### **1. Otras Categorías de Excepciones:**

Considerar crear archivos adicionales para:

- `RoleExceptions.cs` - Excepciones relacionadas con roles
- `PermissionExceptions.cs` - Excepciones relacionadas con permisos
- `ValidationExceptions.cs` - Excepciones de validación general

### **2. Middleware de Manejo de Errores:**

Implementar un middleware global para manejar excepciones de forma centralizada:

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (UserNotFoundByIdError ex)
        {
            await HandleUserNotFoundExceptionAsync(context, ex);
        }
        catch (InvalidCredentialsError ex)
        {
            await HandleAuthenticationExceptionAsync(context, ex);
        }
        // ... más excepciones
    }
}
```

### **3. Documentación de Excepciones:**

Crear documentación detallada de todas las excepciones con:

- Cuándo se lanzan
- Códigos de error
- Metadatos incluidos
- Ejemplos de uso

## ✅ **Resumen**

La reorganización de excepciones ha sido exitosa:

- ✅ **UserExceptions.cs** creado con todas las excepciones de usuario
- ✅ **AuthExceptions.cs** limpiado, manteniendo solo excepciones de autenticación
- ✅ **Compilación exitosa** sin errores
- ✅ **Compatibilidad mantenida** - no hay cambios en el uso
- ✅ **Mejor organización** del código
- ✅ **Separación de responsabilidades** clara

¡La estructura de excepciones ahora está mejor organizada y es más mantenible! 🎉
