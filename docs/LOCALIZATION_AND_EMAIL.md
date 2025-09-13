# Sistema de Localización y Correos Electrónicos

Este documento describe las nuevas funcionalidades implementadas: sistema de localización (es/en), respuestas estandarizadas de API, servicio de correos electrónicos y reset de contraseña con códigos.

## 🌍 Sistema de Localización

### Idiomas Soportados
- **Español (es)** - Idioma por defecto
- **Inglés (en)** - Idioma alternativo

### Configuración
La localización se configura automáticamente en `Program.cs`:

```csharp
// Configure localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "es" };
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});
```

### Archivos de Recursos
- `Resources/es.json` - Traducciones en español
- `Resources/en.json` - Traducciones en inglés
- `Resources/SharedResource.cs` - Clase de recursos compartidos

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

### Cambiar Idioma por Header
```http
Accept-Language: es-ES
```

### Cambiar Idioma por Query String
```http
GET /api/users?culture=es
```

### Cambiar Idioma por Cookie
```http
Cookie: .AspNetCore.Culture=c=es-ES|uic=es-ES
```

## 📝 Mensajes de Localización

### Estructura de Mensajes
```json
{
  "Messages": {
    "Success": {
      "UserCreated": "Usuario creado exitosamente",
      "LoginSuccessful": "Inicio de sesión exitoso"
    },
    "Errors": {
      "InvalidCredentials": "Credenciales inválidas",
      "UserNotFound": "Usuario no encontrado"
    },
    "Validation": {
      "Required": "El campo {0} es requerido",
      "EmailInvalid": "El formato del correo electrónico no es válido"
    }
  },
  "Email": {
    "PasswordReset": {
      "Subject": "Restablecer Contraseña - Clean Architecture"
    }
  }
}
```

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

#### Test de Localización
```csharp
[Test]
public void Should_Return_Spanish_Message_When_Culture_Is_ES()
{
    // Arrange
    var culture = "es-ES";
    
    // Act
    var message = _localizationService.GetSuccessMessage("UserCreated");
    
    // Assert
    Assert.AreEqual("Usuario creado exitosamente", message);
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
