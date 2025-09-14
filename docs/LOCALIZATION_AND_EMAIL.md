# Localization and Email System

This document describes the new features implemented: localization system (es/en) with .resx files, standardized API responses, email service and password reset with codes.

## 🌍 Localization System

### Supported Languages
- **Spanish (es)** - Alternative language
- **English (en)** - Default language

### Configuration
Localization is automatically configured in `Program.cs`:

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

### Resource Files
- `src/CleanArchitecture.API/Resources/Messages.resx` - English resources
- `src/CleanArchitecture.API/Resources/Messages.es.resx` - Spanish resources
- `src/CleanArchitecture.API/Resources/Messages.cs` - Shared resource class

### Usage in Code
```csharp
// Inject the localization service
private readonly ILocalizationService _localizationService;

// Get messages
var successMessage = _localizationService.GetSuccessMessage("UserCreated");
var errorMessage = _localizationService.GetErrorMessage("UserNotFound");
var validationMessage = _localizationService.GetValidationMessage("Required", "Email");
```

## 📧 Email Service

### SMTP Configuration
Configure in `appsettings.json`:

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

### Email Types
1. **Welcome Email** - Upon registration
2. **Password Reset** - With 6-digit code
3. **Password Changed** - Change confirmation
4. **Email Change Verification** - With verification link
5. **Email Change Confirmation** - Change confirmation

### HTML Templates
Emails include:
- **Responsive design** with modern CSS
- **Corporate colors** (blue, green, red depending on type)
- **Security information** and warnings
- **Footer** with company information

### Usage Example
```csharp
// Send welcome email
await _emailService.SendWelcomeEmailAsync(user.Email, user.UserName);

// Send reset code
await _emailService.SendPasswordResetEmailAsync(user.Email, user.UserName, resetCode);

// Send change confirmation
await _emailService.SendPasswordChangedEmailAsync(user.Email, user.UserName);
```

## 🔐 Password Reset with Codes

### Reset Flow
1. **Request Reset**: `POST /api/v1/auth/request-password-reset`
2. **Receive Code**: Email with 6-digit code (expires in 15 minutes)
3. **Reset Password**: `POST /api/v1/auth/reset-password`

### Security Features
- **6-digit codes** randomly generated
- **15-minute expiration** for security
- **Single use** - codes are marked as used
- **Automatic cleanup** of expired codes
- **No email existence revelation** (for security)

### Endpoints

#### Request Password Reset
```http
POST /api/v1/auth/request-password-reset
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Password reset code sent",
  "data": {
    "message": "Password reset code sent",
    "expiresAt": "2024-01-01T12:15:00Z"
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

#### Reset Password
```http
POST /api/v1/auth/reset-password
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "123456",
  "newPassword": "NewPassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Password reset successfully",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## 📋 Standardized API Responses

### Response Structure
```json
{
  "success": true,
  "message": "Descriptive message",
  "data": { /* Response data */ },
  "errors": { /* Validation errors */ },
  "timestamp": "2024-01-01T12:00:00Z",
  "requestId": "optional-guid"
}
```

### Response Types

#### Success Response
```csharp
return Ok(ApiResponse<UserDto>.SuccessResponse(userData, "User created successfully"));
```

#### Error Response
```csharp
return BadRequest(ApiResponse<UserDto>.ErrorResponse("Error creating user"));
```

#### Validation Response
```csharp
return BadRequest(ApiResponse<UserDto>.ValidationErrorResponse("Validation error", validationErrors));
```

### Response Examples

#### Successful Login
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
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "userName": "jdoe",
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

#### Validation Error
```json
{
  "success": false,
  "message": "Validation error",
  "errors": {
    "email": ["Email format is not valid"],
    "password": ["Password must be at least 8 characters"]
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## 🔧 Language Configuration

The system supports multiple methods for specifying the language, with the following priority order:

### 1. Query Parameter (Highest Priority)
```http
POST /api/v1/auth/login?culture=es
GET /api/v1/users?culture=en
```

**Examples:**
- `?culture=es` - Spanish
- `?culture=en` - English
- `?culture=es-ES` - Spanish (Spain)
- `?culture=en-US` - English (United States)

### 2. Accept-Language Header
```http
Accept-Language: es-ES
Accept-Language: en-US
Accept-Language: es
```

### 3. Default Language
If no language is specified, **English (en)** is used as default.

### Usage Examples

#### Query Parameter Priority over Header
```bash
# This request will return Spanish messages, even though the header says English
curl -X POST "http://localhost:5103/api/v1/auth/login?culture=es" \
  -H "Accept-Language: en" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "test", "password": "test"}'
```

#### Header Only
```bash
# This request will return Spanish messages
curl -X POST "http://localhost:5103/api/v1/auth/login" \
  -H "Accept-Language: es" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "test", "password": "test"}'
```

#### Default Language
```bash
# This request will return English messages (by default)
curl -X POST "http://localhost:5103/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "test", "password": "test"}'
```

## 📝 Localization Messages

### .resx File Structure

Messages are organized in `.resx` files with the following structure:

#### Messages.resx (English)
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

#### Messages.es.resx (Spanish)
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

### Naming Convention
- **Error_** - Error messages: `Error_USER_NOT_FOUND`
- **Success_** - Success messages: `Success_LoginSuccessful`
- **Validation_** - Validation messages: `Validation_Required`

### Localized Error Handling

The system includes centralized middleware that handles all exceptions and translates them automatically:

#### Localized Response Examples

**User not found (Spanish):**
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

**User not found (English):**
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

#### Error Localization Flow

1. **Exception thrown** in the application layer
2. **Middleware captures** the exception
3. **Detects language** from request (query param or header)
4. **Translates message** using .resx files
5. **Returns localized** standardized response

## 🚀 New Endpoints

### Authentication
- `POST /api/v1/auth/request-password-reset` - Request password reset
- `POST /api/v1/auth/reset-password` - Reset password with code
- `POST /api/v1/auth/request-email-change` - Request email change
- `POST /api/v1/auth/verify-email-change` - Verify email change

### All Updated Endpoints
All endpoints now return standardized responses with:
- Consistent structure
- Localized messages
- Timestamps
- Uniform error handling

## 🔒 Email Security

### Security Features
1. **SMTP authentication** with secure credentials
2. **TLS/SSL** for in-transit encryption
3. **Single-use codes** with expiration
4. **No password storage** in logs
5. **Input validation** on all endpoints

### Best Practices
1. **Use application passwords** for Gmail
2. **Configure SPF/DKIM** to avoid spam
3. **Monitor email** sending logs
4. **Implement rate limiting** for password resets
5. **Use HTTPS** in production

## 📊 Monitoring and Logs

### Email Logs
```csharp
_logger.LogInformation("Email sent successfully to {Email}", to);
_logger.LogError(ex, "Failed to send email to {Email}", to);
```

### Recommended Metrics
- Email delivery rate
- SMTP response time
- Validation errors per endpoint
- Reset code usage

## 🧪 Testing

### Test Examples

#### Localization Test with Query Parameter
```csharp
[Test]
public async Task Should_Return_Spanish_Message_When_Culture_Query_Parameter_Is_ES()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { emailOrUsername = "test", password = "test" };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/v1/auth/login?culture=es", request);
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
    var response = await client.PostAsJsonAsync("/api/v1/auth/login?culture=en", request);
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
    var response = await client.PostAsJsonAsync("/api/v1/auth/login?culture=es", request);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponse>(content);
    
    // Assert
    Assert.AreEqual("Usuario no encontrado", result.Message); // Query param should win
}
```

#### Password Reset Test
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

## 🔄 Database Migration

To add the new password reset codes table:

```bash
dotnet ef migrations add AddPasswordResetCodes
dotnet ef database update
```

## 📚 Additional Resources

- [ASP.NET Core Localization Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)
- [MailKit Documentation](https://github.com/jstedfast/MailKit)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Email Security Guidelines](https://owasp.org/www-project-email-security-verification-standard/)