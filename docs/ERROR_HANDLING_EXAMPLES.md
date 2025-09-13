# 🔍 Ejemplos Prácticos del Sistema de Manejo de Errores

Este documento contiene ejemplos prácticos de cómo funciona el sistema de manejo de errores en diferentes escenarios.

## 📋 Tabla de Contenidos

- [Ejemplos de API](#ejemplos-de-api)
- [Ejemplos de Código](#ejemplos-de-código)
- [Casos de Uso Comunes](#casos-de-uso-comunes)
- [Testing de Errores](#testing-de-errores)

## 🌐 Ejemplos de API

### 1. Login con Usuario Inexistente

**Request:**
```bash
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "usuario.inexistente@example.com",
  "password": "Password123!"
}
```

**Response (Español):**
```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**Response (English):**
```json
{
  "success": false,
  "message": "User not found",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `404 Not Found`

### 2. Registro con Email Duplicado

**Request:**
```bash
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "admin@example.com",
  "userName": "jperez",
  "password": "Password123!",
  "dateOfBirth": "1990-01-01T00:00:00Z"
}
```

**Response:**
```json
{
  "success": false,
  "message": "El usuario ya existe",
  "errorCode": "USER_ALREADY_EXISTS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `409 Conflict`

### 3. Contraseña Incorrecta

**Request:**
```bash
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "admin",
  "password": "contraseña_incorrecta"
}
```

**Response:**
```json
{
  "success": false,
  "message": "Credenciales inválidas",
  "errorCode": "INVALID_CREDENTIALS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `401 Unauthorized`

### 4. Token de Renovación Inválido

**Request:**
```bash
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "token_invalido_o_expirado"
}
```

**Response:**
```json
{
  "success": false,
  "message": "Token de renovación inválido",
  "errorCode": "INVALID_REFRESH_TOKEN",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `401 Unauthorized`

### 5. Código de Reset Inválido

**Request:**
```bash
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "admin@example.com",
  "code": "123456",
  "newPassword": "NewPassword123!"
}
```

**Response:**
```json
{
  "success": false,
  "message": "Código de restablecimiento inválido",
  "errorCode": "PASSWORD_RESET_CODE_INVALID",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `400 Bad Request`

### 6. Permisos Insuficientes

**Request:**
```bash
GET /api/users
Authorization: Bearer token_con_permisos_insuficientes
```

**Response:**
```json
{
  "success": false,
  "message": "Permisos insuficientes. Requerido: Users.Read",
  "errorCode": "INSUFFICIENT_PERMISSIONS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `403 Forbidden`

### 7. Validación de Datos

**Request:**
```bash
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "",
  "lastName": "Pérez",
  "email": "email_invalido",
  "userName": "a",
  "password": "123",
  "dateOfBirth": "2030-01-01T00:00:00Z"
}
```

**Response:**
```json
{
  "success": false,
  "message": "Error de validación",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "FirstName": ["El campo 'FirstName' es requerido"],
    "Email": ["Formato de correo electrónico inválido"],
    "UserName": ["El nombre de usuario debe tener al menos 3 caracteres"],
    "Password": ["La contraseña debe tener al menos 8 caracteres"]
  },
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `400 Bad Request`

## 💻 Ejemplos de Código

### 1. Command Handler con Manejo de Errores

```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly ILocalizationService _localizationService;

    public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Verificar si el email ya existe
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
        {
            throw new UserAlreadyExistsError("email", request.Email);
        }

        // Verificar si el username ya existe
        var existingUserByUsername = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserByUsername != null)
        {
            throw new UserAlreadyExistsError("username", request.UserName);
        }

        // Validar fecha de nacimiento
        var age = DateTime.UtcNow.Year - request.DateOfBirth.Year;
        if (age < 13 || age > 120)
        {
            throw new InvalidAgeError(age);
        }

        // Crear usuario
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidPasswordError();
        }

        return ApiResponse<UserDto>.SuccessResponse(
            new UserDto { /* ... */ },
            _localizationService.GetSuccessMessage("UserCreated")
        );
    }
}
```

### 2. Service con Validaciones

```csharp
public class AuthService : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(request.EmailOrUsername))
        {
            throw new RequiredFieldError("EmailOrUsername");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new RequiredFieldError("Password");
        }

        // Buscar usuario
        var user = await GetUserByEmailOrUsernameAsync(request.EmailOrUsername);
        if (user == null)
        {
            throw new UserNotFoundError(request.EmailOrUsername);
        }

        // Verificar estado de la cuenta
        if (!user.IsActive)
        {
            throw new AccountDeactivatedError(user.Id.ToString());
        }

        // Verificar contraseña
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new InvalidCredentialsError();
        }

        // Generar tokens
        var token = await GenerateJwtTokenAsync(user);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user)
        };
    }
}
```

### 3. Controller Simplificado

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        // No necesitamos try-catch, el middleware maneja todo
        var command = new LoginCommand { Request = request };
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var command = new RegisterCommand { Request = request };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Login), ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }
}
```

## 🎯 Casos de Uso Comunes

### 1. Flujo de Login Completo

```csharp
// 1. Usuario intenta login con credenciales incorrectas
var loginRequest = new LoginRequestDto 
{ 
    EmailOrUsername = "admin@example.com", 
    Password = "password_incorrecta" 
};

// 2. AuthService valida y lanza excepción
try
{
    var result = await _authService.LoginAsync(loginRequest);
}
catch (InvalidCredentialsError ex)
{
    // 3. Middleware captura la excepción
    // 4. Localiza el mensaje según el idioma del usuario
    // 5. Retorna respuesta estructurada
}
```

**Resultado:**
```json
{
  "success": false,
  "message": "Credenciales inválidas",
  "errorCode": "INVALID_CREDENTIALS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

### 2. Flujo de Registro con Validaciones

```csharp
// 1. Usuario intenta registrarse con datos inválidos
var registerRequest = new RegisterRequestDto
{
    FirstName = "", // Inválido
    Email = "email_invalido", // Inválido
    Password = "123" // Inválido
};

// 2. FluentValidation valida y lanza excepción
// 3. Middleware captura y localiza
// 4. Retorna respuesta con errores detallados
```

**Resultado:**
```json
{
  "success": false,
  "message": "Error de validación",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "FirstName": ["El campo 'FirstName' es requerido"],
    "Email": ["Formato de correo electrónico inválido"],
    "Password": ["La contraseña debe tener al menos 8 caracteres"]
  },
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

### 3. Flujo de Recuperación de Contraseña

```csharp
// 1. Usuario solicita reset con email inexistente
var resetRequest = new RequestPasswordResetDto
{
    Email = "usuario.inexistente@example.com"
};

// 2. AuthService busca usuario
var user = await _userManager.FindByEmailAsync(resetRequest.Email);
if (user == null)
{
    throw new UserNotFoundError(resetRequest.Email);
}

// 3. Middleware maneja y localiza
// 4. Retorna error 404 con mensaje localizado
```

**Resultado:**
```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

## 🧪 Testing de Errores

### 1. Test Unitario de Excepciones

```csharp
[Test]
public async Task Login_WithNonExistentUser_ThrowsUserNotFoundError()
{
    // Arrange
    var request = new LoginRequestDto 
    { 
        EmailOrUsername = "nonexistent@example.com",
        Password = "Password123!"
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<UserNotFoundError>(
        () => _authService.LoginAsync(request));
    
    Assert.Equal("USER_NOT_FOUND", exception.ErrorCode);
    Assert.Contains("nonexistent@example.com", exception.Message);
}

[Test]
public async Task Register_WithExistingEmail_ThrowsUserAlreadyExistsError()
{
    // Arrange
    var existingUser = new User { Email = "existing@example.com" };
    _userManager.Setup(x => x.FindByEmailAsync("existing@example.com"))
                .ReturnsAsync(existingUser);

    var request = new RegisterRequestDto
    {
        Email = "existing@example.com",
        UserName = "newuser",
        Password = "Password123!"
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<UserAlreadyExistsError>(
        () => _authService.RegisterAsync(request));
    
    Assert.Equal("USER_ALREADY_EXISTS", exception.ErrorCode);
    Assert.Equal("email", ((UserAlreadyExistsError)exception).Parameters?.GetType().GetProperty("Field")?.GetValue(((UserAlreadyExistsError)exception).Parameters));
}
```

### 2. Test de Integración de API

```csharp
[Test]
public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
{
    // Arrange
    var request = new LoginRequestDto
    {
        EmailOrUsername = "admin",
        Password = "wrong_password"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    
    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);
    
    Assert.False(apiResponse.Success);
    Assert.Equal("INVALID_CREDENTIALS", apiResponse.ErrorCode);
    Assert.Equal("Credenciales inválidas", apiResponse.Message);
}

[Test]
public async Task Register_WithInvalidData_ReturnsBadRequest()
{
    // Arrange
    var request = new RegisterRequestDto
    {
        FirstName = "", // Inválido
        Email = "invalid_email", // Inválido
        Password = "123" // Inválido
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/auth/register", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    
    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);
    
    Assert.False(apiResponse.Success);
    Assert.Equal("VALIDATION_ERROR", apiResponse.ErrorCode);
}
```

### 3. Test de Localización

```csharp
[Test]
public async Task Login_WithSpanishLanguage_ReturnsSpanishError()
{
    // Arrange
    var request = new LoginRequestDto
    {
        EmailOrUsername = "nonexistent@example.com",
        Password = "Password123!"
    };

    _client.DefaultRequestHeaders.Add("Accept-Language", "es");

    // Act
    var response = await _client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);
    
    Assert.Equal("Usuario no encontrado", apiResponse.Message);
}

[Test]
public async Task Login_WithEnglishLanguage_ReturnsEnglishError()
{
    // Arrange
    var request = new LoginRequestDto
    {
        EmailOrUsername = "nonexistent@example.com",
        Password = "Password123!"
    };

    _client.DefaultRequestHeaders.Add("Accept-Language", "en");

    // Act
    var response = await _client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);
    
    Assert.Equal("User not found", apiResponse.Message);
}
```

## 🔧 Configuración de Testing

### 1. Setup de Test Base

```csharp
public class ErrorHandlingTestBase
{
    protected readonly HttpClient _client;
    protected readonly IServiceProvider _serviceProvider;

    public ErrorHandlingTestBase()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
        _serviceProvider = factory.Services;
    }

    protected async Task<ApiResponse<T>> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
```

### 2. Mock de Servicios

```csharp
public class MockAuthService : IAuthService
{
    public Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (request.EmailOrUsername == "nonexistent@example.com")
        {
            throw new UserNotFoundError(request.EmailOrUsername);
        }
        
        if (request.Password == "wrong_password")
        {
            throw new InvalidCredentialsError();
        }

        return Task.FromResult(new AuthResponseDto { /* ... */ });
    }
}
```

## 📊 Monitoreo y Logging

### 1. Logging Estructurado

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
            // Log estructurado con contexto
            _logger.LogError(ex, 
                "Error occurred for user {UserId} on endpoint {Endpoint} with error code {ErrorCode}",
                context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous",
                context.Request.Path,
                ex is ApplicationException appEx ? appEx.ErrorCode : "UNKNOWN");

            await HandleExceptionAsync(context, ex);
        }
    }
}
```

### 2. Métricas de Errores

```csharp
public class ErrorMetricsService
{
    private readonly ILogger<ErrorMetricsService> _logger;
    private readonly IMetrics _metrics;

    public void RecordError(string errorCode, string endpoint, string userId = null)
    {
        _metrics.Measure.Counter.Increment(new MetricTags("error_code", errorCode, "endpoint", endpoint), "api.errors");
        
        _logger.LogWarning("Error recorded: {ErrorCode} on {Endpoint} for user {UserId}", 
            errorCode, endpoint, userId ?? "Anonymous");
    }
}
```

---

**Última actualización:** 13 de Enero, 2025  
**Versión:** 1.0.0
