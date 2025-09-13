# 🚨 Sistema de Manejo de Errores

Este documento describe el sistema completo de manejo de errores implementado en la aplicación Clean Architecture, que proporciona un manejo consistente, localizable y estructurado de errores en toda la aplicación.

## 📋 Tabla de Contenidos

- [Arquitectura del Sistema](#arquitectura-del-sistema)
- [Clases de Error](#clases-de-error)
- [Middleware de Manejo de Excepciones](#middleware-de-manejo-de-excepciones)
- [Localización de Errores](#localización-de-errores)
- [Respuestas Estándar](#respuestas-estándar)
- [Ejemplos de Uso](#ejemplos-de-uso)
- [Códigos HTTP](#códigos-http)
- [Mejores Prácticas](#mejores-prácticas)

## 🏗️ Arquitectura del Sistema

El sistema de manejo de errores sigue los principios de Clean Architecture y está estructurado en las siguientes capas:

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   AuthController│  │  UsersController│  │ Other Controllers │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
├─────────────────────────────────────────────────────────────┤
│              ExceptionHandlingMiddleware                    │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  • Captura excepciones                                 │ │
│  │  • Mapea códigos HTTP                                  │ │
│  │  • Localiza mensajes                                   │ │
│  │  • Estructura respuestas                               │ │
│  │  • Registra logs                                       │ │
│  └─────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                Application Layer (Services)                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   AuthService   │  │ PermissionService│  │ Other Services │
│  │                 │  │                 │  │               │ │
│  │ Throws:         │  │ Throws:         │  │ Throws:       │ │
│  │ • UserNotFound  │  │ • Insufficient  │  │ • Validation │ │
│  │ • InvalidCreds  │  │   Permissions   │  │ • Custom     │ │
│  │ • AccountDeact  │  │ • RoleNotFound  │  │   Exceptions │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                 Domain Layer (Exceptions)                   │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              ApplicationException Base                  │ │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────────┐   │ │
│  │  │ Auth        │ │ Validation  │ │ Permission      │   │ │
│  │  │ Exceptions  │ │ Exceptions  │ │ Exceptions      │   │ │
│  │  │             │ │             │ │                 │   │ │
│  │  │ • ErrorCode │ │ • ErrorCode │ │ • ErrorCode     │   │ │
│  │  │ • Parameters│ │ • Parameters│ │ • Parameters    │   │ │
│  │  │ • Message   │ │ • Message   │ │ • Message       │   │ │
│  │  └─────────────┘ └─────────────┘ └─────────────────┘   │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Flujo de Manejo de Errores

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Controller    │    │   Service Layer  │    │   Domain Layer  │
│                 │    │                  │    │                 │
│  POST /login    │───▶│  AuthService     │───▶│  UserNotFound   │
│                 │    │                  │    │  Exception      │
│  No try-catch   │    │  Throws specific │    │                 │
│  needed!        │    │  exceptions      │    │  • ErrorCode    │
└─────────────────┘    └──────────────────┘    │  • Parameters   │
                                                │  • Message      │
                                                └─────────────────┘
                                                         │
                                                         ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Client        │    │   Middleware     │    │ Localization    │
│                 │    │                  │    │                 │
│ Receives:       │◀───│  Exception       │◀───│  Service        │
│ • HTTP 404      │    │  Handler         │    │                 │
│ • JSON Response │    │                  │    │  • Gets error   │
│ • Error Code    │    │  • Captures      │    │    code         │
│ • Localized     │    │  • Maps HTTP     │    │  • Translates   │
│   message       │    │  • Structures    │    │    message      │
│                 │    │  • Logs error    │    │  • Returns      │
│                 │    │                  │    │    localized    │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

**Proceso paso a paso:**

1. **Controller** recibe request y delega a service
2. **Service** valida y lanza excepción específica con código de error
3. **ExceptionHandlingMiddleware** captura la excepción automáticamente
4. **LocalizationService** traduce el mensaje según idioma del usuario
5. **Middleware** estructura respuesta JSON consistente
6. **Cliente** recibe respuesta localizada con código HTTP apropiado

## 🔧 Clases de Error

### Clase Base: `ApplicationException`

```csharp
public abstract class ApplicationException : Exception
{
    public string ErrorCode { get; }
    public object? Parameters { get; }

    protected ApplicationException(string errorCode, string message, object? parameters = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }
}
```

**Características:**
- ✅ Código de error único para identificación
- ✅ Parámetros para personalización de mensajes
- ✅ Base para todas las excepciones de la aplicación

### Errores de Autenticación (`AuthExceptions.cs`)

| Error | Código | Descripción | HTTP Status |
|-------|--------|-------------|-------------|
| `UserNotFoundError` | `USER_NOT_FOUND` | Usuario no encontrado | 404 |
| `InvalidCredentialsError` | `INVALID_CREDENTIALS` | Credenciales inválidas | 401 |
| `AccountDeactivatedError` | `ACCOUNT_DEACTIVATED` | Cuenta desactivada | 403 |
| `UserAlreadyExistsError` | `USER_ALREADY_EXISTS` | Usuario ya existe | 409 |
| `InvalidPasswordError` | `INVALID_PASSWORD` | Contraseña inválida | 400 |
| `InvalidRefreshTokenError` | `INVALID_REFRESH_TOKEN` | Token de renovación inválido | 401 |
| `PasswordResetCodeInvalidError` | `PASSWORD_RESET_CODE_INVALID` | Código de reset inválido | 400 |
| `PasswordResetCodeExpiredError` | `PASSWORD_RESET_CODE_EXPIRED` | Código de reset expirado | 400 |
| `PasswordResetCodeAlreadyUsedError` | `PASSWORD_RESET_CODE_ALREADY_USED` | Código ya utilizado | 400 |
| `CurrentPasswordIncorrectError` | `CURRENT_PASSWORD_INCORRECT` | Contraseña actual incorrecta | 400 |
| `PasswordChangeFailedError` | `PASSWORD_CHANGE_FAILED` | Error al cambiar contraseña | 400 |

### Errores de Validación (`ValidationExceptions.cs`)

| Error | Código | Descripción | HTTP Status |
|-------|--------|-------------|-------------|
| `ValidationError` | `VALIDATION_ERROR` | Error de validación genérico | 400 |
| `RequiredFieldError` | `REQUIRED_FIELD` | Campo requerido | 400 |
| `InvalidEmailFormatError` | `INVALID_EMAIL_FORMAT` | Formato de email inválido | 400 |
| `PasswordTooWeakError` | `PASSWORD_TOO_WEAK` | Contraseña débil | 400 |
| `InvalidDateOfBirthError` | `INVALID_DATE_OF_BIRTH` | Fecha de nacimiento inválida | 400 |
| `InvalidAgeError` | `INVALID_AGE` | Edad inválida | 400 |
| `UsernameTooShortError` | `USERNAME_TOO_SHORT` | Usuario muy corto | 400 |
| `UsernameTooLongError` | `USERNAME_TOO_LONG` | Usuario muy largo | 400 |
| `InvalidUsernameFormatError` | `INVALID_USERNAME_FORMAT` | Formato de usuario inválido | 400 |

### Errores de Permisos (`PermissionExceptions.cs`)

| Error | Código | Descripción | HTTP Status |
|-------|--------|-------------|-------------|
| `InsufficientPermissionsError` | `INSUFFICIENT_PERMISSIONS` | Permisos insuficientes | 403 |
| `RoleNotFoundError` | `ROLE_NOT_FOUND` | Rol no encontrado | 404 |
| `RoleNotFoundByIdError` | `ROLE_NOT_FOUND_BY_ID` | Rol no encontrado por ID | 404 |
| `RoleAlreadyExistsError` | `ROLE_ALREADY_EXISTS` | Rol ya existe | 409 |
| `PermissionNotFoundError` | `PERMISSION_NOT_FOUND` | Permiso no encontrado | 404 |
| `PermissionNotFoundByIdError` | `PERMISSION_NOT_FOUND_BY_ID` | Permiso no encontrado por ID | 404 |
| `PermissionAlreadyExistsError` | `PERMISSION_ALREADY_EXISTS` | Permiso ya existe | 409 |
| `UserNotInRoleError` | `USER_NOT_IN_ROLE` | Usuario no está en rol | 400 |
| `RolePermissionNotFoundError` | `ROLE_PERMISSION_NOT_FOUND` | Rol no tiene permiso | 404 |

## 🔄 Middleware de Manejo de Excepciones

El `ExceptionHandlingMiddleware` se ejecuta automáticamente y maneja todas las excepciones no controladas:

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

### Configuración en Program.cs

```csharp
// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

**Características:**
- ✅ Captura automática de todas las excepciones
- ✅ Mapeo de códigos de error a códigos HTTP apropiados
- ✅ Localización automática de mensajes
- ✅ Logging estructurado de errores
- ✅ Respuestas JSON consistentes

## 🌍 Localización de Errores

### Archivos de Localización

Los mensajes de error se localizan usando archivos JSON:

#### Español (`es.json`)
```json
{
  "Messages": {
    "Errors": {
      "USER_NOT_FOUND": "Usuario no encontrado",
      "INVALID_CREDENTIALS": "Credenciales inválidas",
      "USER_ALREADY_EXISTS": "El usuario ya existe",
      "PASSWORD_TOO_WEAK": "La contraseña no cumple con los requisitos de seguridad"
    }
  }
}
```

#### Inglés (`en.json`)
```json
{
  "Messages": {
    "Errors": {
      "USER_NOT_FOUND": "User not found",
      "INVALID_CREDENTIALS": "Invalid credentials",
      "USER_ALREADY_EXISTS": "User already exists",
      "PASSWORD_TOO_WEAK": "Password does not meet security requirements"
    }
  }
}
```

### Configuración de Idiomas

```csharp
var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
```

**Idiomas soportados:**
- 🇺🇸 **Inglés (en)** - Idioma por defecto
- 🇪🇸 **Español (es)** - Idioma alternativo

## 📊 Respuestas Estándar

### Estructura de Respuesta de Error

```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-01-13T12:00:00.000Z",
  "requestId": "req_123456789"
}
```

### Estructura de Respuesta de Éxito

```json
{
  "success": true,
  "message": "Usuario creado exitosamente",
  "data": {
    "id": "guid",
    "firstName": "Juan",
    "lastName": "Pérez"
  },
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

### Clase ApiResponse

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RequestId { get; set; }
}
```

## 💡 Ejemplos de Uso

### 1. En Command Handlers

```csharp
public async Task<ApiResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
{
    var existingUser = await _userManager.FindByEmailAsync(request.Email);
    if (existingUser != null)
    {
        throw new UserAlreadyExistsError("email", request.Email);
    }
    
    // ... resto del código
}
```

### 2. En Services

```csharp
public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
{
    var user = await GetUserByEmailOrUsernameAsync(request.EmailOrUsername);
    if (user == null)
    {
        throw new UserNotFoundError(request.EmailOrUsername);
    }

    if (!user.IsActive)
    {
        throw new AccountDeactivatedError(user.Id.ToString());
    }
    
    // ... resto del código
}
```

### 3. En Controllers (Opcional)

Los controllers pueden manejar excepciones específicas si es necesario:

```csharp
[HttpPost("login")]
public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
{
    try
    {
        var command = new LoginCommand { Request = request };
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }
    catch (ApplicationException ex)
    {
        // El middleware manejará automáticamente estas excepciones
        throw;
    }
}
```

## 🔢 Códigos HTTP

### Mapeo Automático de Códigos

| Tipo de Error | Código HTTP | Descripción |
|---------------|-------------|-------------|
| `UserNotFoundError` | 404 | Not Found |
| `InvalidCredentialsError` | 401 | Unauthorized |
| `AccountDeactivatedError` | 403 | Forbidden |
| `UserAlreadyExistsError` | 409 | Conflict |
| `InvalidPasswordError` | 400 | Bad Request |
| `InsufficientPermissionsError` | 403 | Forbidden |
| `ValidationError` | 400 | Bad Request |
| `UnauthorizedAccessException` | 401 | Unauthorized |
| `ArgumentException` | 400 | Bad Request |
| Otros errores | 500 | Internal Server Error |

### Función de Mapeo

```csharp
private static int GetStatusCodeForApplicationException(ApplicationException exception)
{
    return exception switch
    {
        UserNotFoundError => (int)HttpStatusCode.NotFound,
        InvalidCredentialsError => (int)HttpStatusCode.Unauthorized,
        AccountDeactivatedError => (int)HttpStatusCode.Forbidden,
        UserAlreadyExistsError => (int)HttpStatusCode.Conflict,
        InvalidPasswordError => (int)HttpStatusCode.BadRequest,
        InsufficientPermissionsError => (int)HttpStatusCode.Forbidden,
        ValidationError => (int)HttpStatusCode.BadRequest,
        _ => (int)HttpStatusCode.BadRequest
    };
}
```

## ✅ Mejores Prácticas

### 1. **Usar Excepciones Específicas**

❌ **Mal:**
```csharp
throw new Exception("User not found");
```

✅ **Bien:**
```csharp
throw new UserNotFoundError(request.Email);
```

### 2. **Incluir Contexto en las Excepciones**

❌ **Mal:**
```csharp
throw new UserNotFoundError("User not found");
```

✅ **Bien:**
```csharp
throw new UserNotFoundError(request.Email);
```

### 3. **No Manejar Excepciones en Controllers**

❌ **Mal:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    try
    {
        // ... código
    }
    catch (UserNotFoundError ex)
    {
        return NotFound(ApiResponse.ErrorResponse("User not found"));
    }
}
```

✅ **Bien:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    var command = new LoginCommand { Request = request };
    var result = await _mediator.Send(command);
    return Ok(ApiResponse.SuccessResponse(result));
}
```

### 4. **Usar Códigos de Error Consistentes**

❌ **Mal:**
```csharp
throw new ApplicationException("USER_NOT_FOUND", "User not found");
throw new ApplicationException("user-not-found", "User not found");
```

✅ **Bien:**
```csharp
throw new UserNotFoundError(request.Email); // Siempre usa "USER_NOT_FOUND"
```

### 5. **Localizar Mensajes de Error**

❌ **Mal:**
```csharp
throw new ApplicationException("USER_NOT_FOUND", "Usuario no encontrado");
```

✅ **Bien:**
```csharp
throw new UserNotFoundError(request.Email); // El middleware localiza automáticamente
```

### 6. **Logging de Errores**

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

## 🔍 Debugging y Testing

### Testing de Excepciones

```csharp
[Test]
public async Task Login_WithNonExistentUser_ThrowsUserNotFoundError()
{
    // Arrange
    var request = new LoginRequestDto { EmailOrUsername = "nonexistent@example.com" };
    
    // Act & Assert
    var exception = await Assert.ThrowsAsync<UserNotFoundError>(
        () => _authService.LoginAsync(request));
    
    Assert.Equal("USER_NOT_FOUND", exception.ErrorCode);
}
```

### Logging Estructurado

```csharp
_logger.LogError("User not found with email {Email}", request.Email);
_logger.LogWarning("Invalid password attempt for user {UserId}", user.Id);
_logger.LogInformation("Password reset code generated for user {UserId}", user.Id);
```

## 🚀 Extensiones Futuras

### Posibles Mejoras

- [ ] **Rate Limiting**: Límites de intentos de login
- [ ] **Error Analytics**: Métricas de errores más frecuentes
- [ ] **Error Recovery**: Sugerencias de recuperación
- [ ] **Error Context**: Más contexto en las respuestas de error
- [ ] **Error Correlation**: IDs de correlación para debugging
- [ ] **Error Notifications**: Notificaciones automáticas de errores críticos

### Integración con Herramientas Externas

- [ ] **Sentry**: Para tracking de errores en producción
- [ ] **Application Insights**: Para métricas y análisis
- [ ] **Elasticsearch**: Para búsqueda de logs de errores
- [ ] **Grafana**: Para dashboards de errores

## 📚 Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Exception Handling](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)

---

**Última actualización:** 13 de Enero, 2025  
**Versión:** 1.0.0
