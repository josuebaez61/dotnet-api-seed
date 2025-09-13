# Sistema de Localización y Correos Electrónicos

Este documento describe las nuevas funcionalidades implementadas: sistema de localización (es/en) con archivos .resx, respuestas estandarizadas de API, servicio de correos electrónicos y reset de contraseña con códigos.

## 🌍 Sistema de Localización

### Idiomas Soportados
- **Español (es)** - Idioma alternativo
- **Inglés (en)** - Idioma por defecto

### Configuración
La localización se configura automáticamente en `Program.cs`:

```csharp
// Configure localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "es" };
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});
```

### Archivos de Recursos
- `src/CleanArchitecture.API/Resources/Messages.resx` - Recursos en inglés
- `src/CleanArchitecture.API/Resources/Messages.es.resx` - Recursos en español
- `src/CleanArchitecture.API/Resources/Messages.cs` - Clase de recursos compartidos

### Uso en el Código
```csharp
// Inyectar el servicio de localización
private readonly ILocalizationService _localizationService;

// Obtener mensajes
var successMessage = _localizationService.GetSuccessMessage("UserCreated");
var errorMessage = _localizationService.GetErrorMessage("UserNotFound");
var validationMessage = _localizationService.GetValidationMessage("Required", "Email");
```

## 📧 Servicio de Correos Electrónicos

### Configuración SMTP
Configurar en `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@cleanarchitecture.com",
    "FromName": "Clean Architecture"
  }
}
```

### Tipos de Correos
1. **Correo de Bienvenida** - Al registrarse
2. **Reset de Contraseña** - Con código de 6 dígitos
3. **Contraseña Cambiada** - Confirmación de cambio

### Templates HTML
Los correos incluyen:
- **Diseño responsivo** con CSS moderno
- **Colores corporativos** (azul, verde, rojo según el tipo)
- **Información de seguridad** y advertencias
- **Footer** con información de la empresa

### Ejemplo de Uso
```csharp
// Enviar correo de bienvenida
await _emailService.SendWelcomeEmailAsync(user.Email, user.UserName);

// Enviar código de reset
await _emailService.SendPasswordResetEmailAsync(user.Email, user.UserName, resetCode);

// Enviar confirmación de cambio
await _emailService.SendPasswordChangedEmailAsync(user.Email, user.UserName);
```

## 🔐 Reset de Contraseña con Códigos

### Flujo de Reset
1. **Solicitar Reset**: `POST /api/auth/request-password-reset`
2. **Recibir Código**: Correo con código de 6 dígitos (expira en 15 minutos)
3. **Resetear Contraseña**: `POST /api/auth/reset-password`

### Características de Seguridad
- **Códigos de 6 dígitos** generados aleatoriamente
- **Expiración en 15 minutos** por seguridad
- **Un solo uso** - los códigos se marcan como usados
- **Limpieza automática** de códigos expirados
- **No revelación** de existencia de emails (por seguridad)

### Endpoints

#### Solicitar Reset de Contraseña
```http
POST /api/auth/request-password-reset
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Código de restablecimiento de contraseña enviado",
  "data": {
    "message": "Código de restablecimiento de contraseña enviado",
    "expiresAt": "2024-01-01T12:15:00Z"
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

#### Resetear Contraseña
```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "123456",
  "newPassword": "NewPassword123!"
}
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Contraseña restablecida exitosamente",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## 📋 Respuestas Estandarizadas de API

### Estructura de Respuesta
```json
{
  "success": true,
  "message": "Mensaje descriptivo",
  "data": { /* Datos de respuesta */ },
  "errors": { /* Errores de validación */ },
  "timestamp": "2024-01-01T12:00:00Z",
  "requestId": "guid-opcional"
}
```

### Tipos de Respuesta

#### Respuesta Exitosa
```csharp
return Ok(ApiResponse<UserDto>.SuccessResponse(userData, "Usuario creado exitosamente"));
```

#### Respuesta de Error
```csharp
return BadRequest(ApiResponse<UserDto>.ErrorResponse("Error al crear usuario"));
```

#### Respuesta de Validación
```csharp
return BadRequest(ApiResponse<UserDto>.ValidationErrorResponse("Error de validación", validationErrors));
```

### Ejemplos de Respuestas

#### Login Exitoso
```json
{
  "success": true,
  "message": "",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiresAt": "2024-01-01T13:00:00Z",
    "user": {
      "id": "guid",
      "firstName": "Juan",
      "lastName": "Pérez",
      "email": "juan@example.com",
      "userName": "jperez",
      "dateOfBirth": "1990-01-01T00:00:00Z",
      "profilePicture": "https://example.com/photo.jpg",
      "createdAt": "2024-01-01T10:00:00Z",
      "updatedAt": null,
      "isActive": true,
      "emailConfirmed": true
    }
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

#### Error de Validación
```json
{
  "success": false,
  "message": "Error de validación",
  "errors": {
    "email": ["El formato del correo electrónico no es válido"],
    "password": ["La contraseña debe tener al menos 8 caracteres"]
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## 🔧 Configuración de Idiomas

El sistema soporta múltiples métodos para especificar el idioma, con el siguiente orden de prioridad:

### 1. Query Parameter (Mayor Prioridad)
```http
POST /api/auth/login?culture=es
GET /api/users?culture=en
```

**Ejemplos:**
- `?culture=es` - Español
- `?culture=en` - Inglés
- `?culture=es-ES` - Español (España)
- `?culture=en-US` - Inglés (Estados Unidos)

### 2. Accept-Language Header
```http
Accept-Language: es-ES
Accept-Language: en-US
Accept-Language: es
```

### 3. Idioma por Defecto
Si no se especifica ningún idioma, se usa **Inglés (en)** como predeterminado.

### Ejemplos de Uso

#### Prioridad de Query Parameter sobre Header
```bash
# Este request devolverá mensajes en español, aunque el header diga inglés
curl -X POST "http://localhost:5103/api/auth/login?culture=es" \
  -H "Accept-Language: en" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "test", "password": "test"}'
```

#### Solo Header
```bash
# Este request devolverá mensajes en español
curl -X POST "http://localhost:5103/api/auth/login" \
  -H "Accept-Language: es" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "test", "password": "test"}'
```

#### Idioma por Defecto
```bash
# Este request devolverá mensajes en inglés (por defecto)
curl -X POST "http://localhost:5103/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "test", "password": "test"}'
```

## 📝 Mensajes de Localización

### Estructura de Archivos .resx

Los mensajes están organizados en archivos `.resx` con la siguiente estructura:

#### Messages.resx (Inglés)
```xml
<data name="Error_USER_NOT_FOUND" xml:space="preserve">
  <value>User not found</value>
</data>
<data name="Error_INVALID_CREDENTIALS" xml:space="preserve">
  <value>Invalid credentials</value>
</data>
<data name="Success_LoginSuccessful" xml:space="preserve">
  <value>Login successful</value>
</data>
```

#### Messages.es.resx (Español)
```xml
<data name="Error_USER_NOT_FOUND" xml:space="preserve">
  <value>Usuario no encontrado</value>
</data>
<data name="Error_INVALID_CREDENTIALS" xml:space="preserve">
  <value>Credenciales inválidas</value>
</data>
<data name="Success_LoginSuccessful" xml:space="preserve">
  <value>Inicio de sesión exitoso</value>
</data>
```

### Convención de Nombres
- **Error_** - Mensajes de error: `Error_USER_NOT_FOUND`
- **Success_** - Mensajes de éxito: `Success_LoginSuccessful`
- **Validation_** - Mensajes de validación: `Validation_Required`

### Manejo de Errores Localizado

El sistema incluye un middleware centralizado que maneja todas las excepciones y las traduce automáticamente:

#### Ejemplos de Respuestas Localizadas

**Usuario no encontrado (Español):**
```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "data": null,
  "errors": null,
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-09-13T14:48:20.161813Z",
  "requestId": null
}
```

**Usuario no encontrado (Inglés):**
```json
{
  "success": false,
  "message": "User not found",
  "data": null,
  "errors": null,
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-09-13T14:48:20.161813Z",
  "requestId": null
}
```

#### Flujo de Localización de Errores

1. **Excepción lanzada** en la capa de aplicación
2. **Middleware captura** la excepción
3. **Detecta idioma** del request (query param o header)
4. **Traduce mensaje** usando archivos .resx
5. **Devuelve respuesta** estandarizada localizada

## 🚀 Nuevos Endpoints

### Autenticación
- `POST /api/auth/request-password-reset` - Solicitar reset de contraseña
- `POST /api/auth/reset-password` - Resetear contraseña con código

### Todos los Endpoints Actualizados
Todos los endpoints ahora devuelven respuestas estandarizadas con:
- Estructura consistente
- Mensajes localizados
- Timestamps
- Manejo de errores uniforme

## 🔒 Seguridad de Correos

### Características de Seguridad
1. **Autenticación SMTP** con credenciales seguras
2. **TLS/SSL** para encriptación en tránsito
3. **Códigos de un solo uso** con expiración
4. **No almacenamiento** de contraseñas en logs
5. **Validación de entrada** en todos los endpoints

### Mejores Prácticas
1. **Usar contraseñas de aplicación** para Gmail
2. **Configurar SPF/DKIM** para evitar spam
3. **Monitorear logs** de envío de correos
4. **Implementar rate limiting** para reset de contraseñas
5. **Usar HTTPS** en producción

## 📊 Monitoreo y Logs

### Logs de Correos
```csharp
_logger.LogInformation("Email sent successfully to {Email}", to);
_logger.LogError(ex, "Failed to send email to {Email}", to);
```

### Métricas Recomendadas
- Tasa de entrega de correos
- Tiempo de respuesta de SMTP
- Errores de validación por endpoint
- Uso de códigos de reset

## 🧪 Testing

### Ejemplos de Pruebas

#### Test de Localización con Query Parameter
```csharp
[Test]
public async Task Should_Return_Spanish_Message_When_Culture_Query_Parameter_Is_ES()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { emailOrUsername = "test", password = "test" };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login?culture=es", request);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponse>(content);
    
    // Assert
    Assert.AreEqual("Usuario no encontrado", result.Message);
}

[Test]
public async Task Should_Return_English_Message_When_Culture_Query_Parameter_Is_EN()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { emailOrUsername = "test", password = "test" };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login?culture=en", request);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponse>(content);
    
    // Assert
    Assert.AreEqual("User not found", result.Message);
}

[Test]
public async Task Should_Prioritize_Query_Parameter_Over_Header()
{
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Add("Accept-Language", "en");
    var request = new { emailOrUsername = "test", password = "test" };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login?culture=es", request);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponse>(content);
    
    // Assert
    Assert.AreEqual("Usuario no encontrado", result.Message); // Query param should win
}
```

#### Test de Reset de Contraseña
```csharp
[Test]
public async Task Should_Send_Reset_Email_When_Valid_Email()
{
    // Arrange
    var request = new RequestPasswordResetDto { Email = "test@example.com" };
    
    // Act
    var result = await _authController.RequestPasswordReset(request);
    
    // Assert
    Assert.IsTrue(result.Value.Success);
    _emailService.Verify(x => x.SendPasswordResetEmailAsync(
        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
}
```

## 🔄 Migración de Base de Datos

Para agregar la nueva tabla de códigos de reset:

```bash
dotnet ef migrations add AddPasswordResetCodes
dotnet ef database update
```

## 📚 Recursos Adicionales

- [Documentación de Localización de ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)
- [MailKit Documentation](https://github.com/jstedfast/MailKit)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Email Security Guidelines](https://owasp.org/www-project-email-security-verification-standard/)
