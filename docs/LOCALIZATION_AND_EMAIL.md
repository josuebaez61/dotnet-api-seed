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

// Get messages using clean error codes (prefixes added automatically)
var successMessage = _localizationService.GetSuccessMessage("USER_CREATED");
var errorMessage = _localizationService.GetErrorMessage("USER_NOT_FOUND");
var validationMessage = _localizationService.GetValidationMessage("REQUIRED_FIELD", "Email");

// Direct string access (for custom keys)
var customMessage = _localizationService.GetString("CUSTOM_MESSAGE_KEY");
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
  },
  "FrontendSettings": {
    "BaseUrl": "http://localhost:4200"
  }
}
```

### Frontend URL Configuration

The system now supports configurable frontend URLs for email links:

#### Development
```json
{
  "FrontendSettings": {
    "BaseUrl": "http://localhost:4200"
  }
}
```

#### Production
```json
{
  "FrontendSettings": {
    "BaseUrl": "https://yourapp.com"
  }
}
```

#### Docker Environment
```json
{
  "FrontendSettings": {
    "BaseUrl": "http://frontend:4200"
  }
}
```

#### Environment Variables
```bash
export FrontendSettings__BaseUrl="https://staging.yourapp.com"
```

#### Generated Links
- **Password Reset**: `{BaseUrl}/auth/reset-password?code={CODE}`
- **Email Verification**: `{BaseUrl}/auth/confirm-email?code={CODE}`

### Email Types

1. **Welcome Email** - Upon registration
2. **Password Reset** - With reset link (string code)
3. **Password Changed** - Change confirmation
4. **Email Change Verification** - With verification link
5. **Email Change Confirmation** - Change confirmation
6. **Temporary Password** - For admin-created accounts

### HTML Templates

The email system now uses external HTML templates with the following structure:

#### Base Template
- **File**: `email-template.html` - Common wrapper with styles, header, and footer
- **Features**: Responsive design, corporate branding, consistent styling

#### Specific Templates
Each email type has its own template per language:
- **Password Reset**: `password-reset.html` (en/es)
- **Welcome**: `welcome.html` (en/es)
- **Password Changed**: `password-changed.html` (en/es)
- **Email Change Verification**: `email-change-verification.html` (en/es)
- **Email Change Confirmation**: `email-change-confirmation.html` (en/es)
- **Temporary Password**: `temporary-password.html` (en/es)

#### Template Features
- **Responsive design** with modern CSS
- **Corporate colors** (blue, green, red depending on type)
- **Security information** and warnings
- **Footer** with company information
- **Parameter substitution** for dynamic content
- **Multi-language support** with separate templates

### Email Template Service

The system uses a dedicated `IEmailTemplateService` for rendering HTML emails:

#### Template Structure
```
Common/Templates/Email/
├── Base/
│   └── email-template.html          # Base template with styles
├── PasswordReset/
│   ├── en/password-reset.html       # English template
│   └── es/password-reset.html       # Spanish template
├── Welcome/
│   ├── en/welcome.html
│   └── es/welcome.html
└── ... (other email types)
```

#### Template Parameters
Templates support dynamic parameter substitution:

```csharp
var parameters = new Dictionary<string, object>
{
    ["UserName"] = "John Doe",
    ["ResetLink"] = "https://app.com/auth/reset-password?code=ABC123",
    ["VerificationLink"] = "https://app.com/auth/confirm-email?code=XYZ789"
};
```

#### Template Rendering
```csharp
// The service automatically combines base template with specific content
var html = await _emailTemplateService.RenderEmailAsync("PasswordReset", "en", parameters);
```

#### Embedded Resources
All templates are configured as embedded resources in `CleanArchitecture.Application.csproj`:

```xml
<ItemGroup>
  <EmbeddedResource Include="Common\Templates\Email\Base\email-template.html" />
  <EmbeddedResource Include="Common\Templates\Email\PasswordReset\en\password-reset.html" />
  <EmbeddedResource Include="Common\Templates\Email\PasswordReset\es\password-reset.html" />
  <!-- ... other templates ... -->
</ItemGroup>
```

### Usage Example

```csharp
// Send welcome email
await _emailService.SendWelcomeEmailAsync(user.Email, user.UserName);

// Send password reset link
await _emailService.SendPasswordResetEmailAsync(user.Email, user.UserName, resetCode);

// Send email change verification link
await _emailService.SendEmailChangeVerificationEmailAsync(user.Email, user.UserName, verificationCode);

// Send change confirmation
await _emailService.SendPasswordChangedEmailAsync(user.Email, user.UserName);

// Send temporary password
await _emailService.SendTemporaryPasswordEmailAsync(user.Email, user.UserName, temporaryPassword);
```

## 🔐 Password Reset with Links

### Reset Flow

1. **Request Reset**: `POST /api/v1/auth/request-password-reset`
2. **Receive Link**: Email with reset link (expires in 15 minutes)
3. **Reset Password**: `POST /api/v1/auth/reset-password`

### Security Features

- **String-based codes** (16-32 characters) randomly generated
- **15-minute expiration** for security
- **Single use** - codes are marked as used
- **Automatic cleanup** of expired codes
- **No email existence revelation** (for security)
- **Frontend link integration** with configurable URLs

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
  "code": "ABC123DEF456GHI789",
  "newPassword": "NewPassword123!"
}
```

**Note**: The `email` field has been removed as the user is identified by the reset code.

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
  "data": {
    /* Response data */
  },
  "errors": {
    /* Validation errors */
  },
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
<data name="ERROR_USER_NOT_FOUND" xml:space="preserve">
  <value>User not found</value>
</data>
<data name="ERROR_INVALID_CREDENTIALS" xml:space="preserve">
  <value>Invalid credentials</value>
</data>
<data name="SUCCESS_LOGIN_SUCCESSFUL" xml:space="preserve">
  <value>Login successful</value>
</data>
<data name="VALIDATION_REQUIRED_FIELD" xml:space="preserve">
  <value>This field is required</value>
</data>
```

#### Messages.es.resx (Spanish)

```xml
<data name="ERROR_USER_NOT_FOUND" xml:space="preserve">
  <value>Usuario no encontrado</value>
</data>
<data name="ERROR_INVALID_CREDENTIALS" xml:space="preserve">
  <value>Credenciales inválidas</value>
</data>
<data name="SUCCESS_LOGIN_SUCCESSFUL" xml:space="preserve">
  <value>Inicio de sesión exitoso</value>
</data>
<data name="VALIDATION_REQUIRED_FIELD" xml:space="preserve">
  <value>Este campo es obligatorio</value>
</data>
```

### Naming Convention

The system uses **UPPER_SNAKE_CASE** convention for all translation keys:

- **ERROR\_** - Error messages: `ERROR_USER_NOT_FOUND`, `ERROR_INVALID_CREDENTIALS`
- **SUCCESS\_** - Success messages: `SUCCESS_LOGIN_SUCCESSFUL`, `SUCCESS_USER_CREATED`
- **VALIDATION\_** - Validation messages: `VALIDATION_REQUIRED_FIELD`, `VALIDATION_INVALID_EMAIL_FORMAT`
- **EMAIL\_** - Email subjects: `EMAIL_WELCOME_SUBJECT`, `EMAIL_PASSWORD_RESET_SUBJECT`

### Separation of Concerns

The system implements a clean separation between exception error codes and localization keys:

#### Exception Error Codes (Clean Format)

```csharp
// Exceptions use clean error codes without prefixes
throw new InvalidCredentialsError(); // ErrorCode = "INVALID_CREDENTIALS"
throw new UserNotFoundError("admin"); // ErrorCode = "USER_NOT_FOUND"
```

#### LocalizationService (Automatic Prefixes)

```csharp
// LocalizationService automatically adds appropriate prefixes
localizationService.GetErrorMessage("INVALID_CREDENTIALS");
// → Looks for "ERROR_INVALID_CREDENTIALS" in .resx files

localizationService.GetSuccessMessage("LOGIN_SUCCESSFUL");
// → Looks for "SUCCESS_LOGIN_SUCCESSFUL" in .resx files

localizationService.GetValidationMessage("REQUIRED_FIELD");
// → Looks for "VALIDATION_REQUIRED_FIELD" in .resx files
```

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

**Invalid credentials (Spanish):**

```json
{
  "success": false,
  "message": "Credenciales inválidas",
  "data": null,
  "errors": null,
  "errorCode": "INVALID_CREDENTIALS",
  "timestamp": "2025-09-13T14:48:20.161813Z",
  "requestId": null
}
```

#### Error Localization Flow

1. **Exception thrown** in the application layer with clean error code (e.g., `INVALID_CREDENTIALS`)
2. **Middleware captures** the exception
3. **Detects language** from request (query param or header)
4. **LocalizationService** automatically adds appropriate prefix (`ERROR_`) and looks up translation
5. **Returns localized** standardized response with translated message

#### Example Flow:

```csharp
// 1. Exception thrown with clean error code
throw new InvalidCredentialsError(); // ErrorCode = "INVALID_CREDENTIALS"

// 2. Middleware calls LocalizationService
localizationService.GetErrorMessage("INVALID_CREDENTIALS");

// 3. LocalizationService adds prefix and looks up translation
// Searches for "ERROR_INVALID_CREDENTIALS" in .resx files

// 4. Returns localized message based on culture
// Spanish: "Credenciales inválidas"
// English: "Invalid credentials"
```

### Best Practices

#### 1. Exception Error Codes

- Use **clean, descriptive error codes** without prefixes
- Follow **UPPER_SNAKE_CASE** convention
- Examples: `INVALID_CREDENTIALS`, `USER_NOT_FOUND`, `PASSWORD_TOO_WEAK`

#### 2. Adding New Translations

When adding new error messages:

1. **Add to exception class:**

```csharp
public class NewError : ApplicationException
{
    public NewError()
        : base("NEW_ERROR_CODE", "Default message")
    {
    }
}
```

2. **Add to resource files:**

```xml
<!-- Messages.resx (English) -->
<data name="ERROR_NEW_ERROR_CODE" xml:space="preserve">
  <value>New error message</value>
</data>

<!-- Messages.es.resx (Spanish) -->
<data name="ERROR_NEW_ERROR_CODE" xml:space="preserve">
  <value>Mensaje de nuevo error</value>
</data>
```

3. **Use in code:**

```csharp
throw new NewError(); // Clean error code
// LocalizationService automatically handles prefix and translation
```

#### 3. Benefits of This Approach

- **Separation of Concerns**: Exceptions focus on business logic, localization handles presentation
- **Consistency**: All translation keys follow the same UPPER_SNAKE_CASE convention
- **Maintainability**: Easy to add new translations following the established pattern
- **Flexibility**: LocalizationService can handle different message types (error, success, validation)
- **Clean Code**: Exception error codes are readable and don't include implementation details

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
