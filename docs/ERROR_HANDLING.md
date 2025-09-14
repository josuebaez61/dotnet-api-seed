# 🚨 Error Handling System

This document describes the complete error handling system implemented in the Clean Architecture application, providing consistent, localizable, and structured error handling throughout the application.

## 📋 Table of Contents

- [System Architecture](#system-architecture)
- [Error Classes](#error-classes)
- [Exception Handling Middleware](#exception-handling-middleware)
- [Error Localization](#error-localization)
- [Standard Responses](#standard-responses)
- [Usage Examples](#usage-examples)
- [HTTP Codes](#http-codes)
- [Best Practices](#best-practices)

## 🏗️ System Architecture

The error handling system follows Clean Architecture principles and is structured in the following layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   AuthController│  │  UsersController│  │ Other Controllers │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
├─────────────────────────────────────────────────────────────┤
│              ExceptionHandlingMiddleware                    │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  • Captures exceptions                                  │ │
│  │  • Maps HTTP codes                                      │ │
│  │  │  • Localizes messages                                │ │
│  │  │  • Structures responses                              │ │
│  │  │  • Logs errors                                       │ │
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

### Error Handling Flow

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

**Step-by-step process:**

1. **Controller** receives request and delegates to service
2. **Service** validates and throws specific exception with error code
3. **ExceptionHandlingMiddleware** automatically captures the exception
4. **LocalizationService** translates the message according to user's language
5. **Middleware** structures consistent JSON response
6. **Client** receives localized response with appropriate HTTP code

## 🔧 Error Classes

### Base Class: `ApplicationException`

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

**Features:**
- ✅ Unique error code for identification
- ✅ Parameters for message customization
- ✅ Base for all application exceptions

### Authentication Errors (`AuthExceptions.cs`)

| Error | Code | Description | HTTP Status |
|-------|------|-------------|-------------|
| `UserNotFoundError` | `USER_NOT_FOUND` | User not found | 404 |
| `InvalidCredentialsError` | `INVALID_CREDENTIALS` | Invalid credentials | 401 |
| `AccountDeactivatedError` | `ACCOUNT_DEACTIVATED` | Account deactivated | 403 |
| `UserAlreadyExistsError` | `USER_ALREADY_EXISTS` | User already exists | 409 |
| `InvalidPasswordError` | `INVALID_PASSWORD` | Invalid password | 400 |
| `InvalidRefreshTokenError` | `INVALID_REFRESH_TOKEN` | Invalid refresh token | 401 |
| `PasswordResetCodeInvalidError` | `PASSWORD_RESET_CODE_INVALID` | Invalid reset code | 400 |
| `PasswordResetCodeExpiredError` | `PASSWORD_RESET_CODE_EXPIRED` | Expired reset code | 400 |
| `PasswordResetCodeAlreadyUsedError` | `PASSWORD_RESET_CODE_ALREADY_USED` | Code already used | 400 |
| `CurrentPasswordIncorrectError` | `CURRENT_PASSWORD_INCORRECT` | Current password incorrect | 400 |
| `PasswordChangeFailedError` | `PASSWORD_CHANGE_FAILED` | Password change failed | 400 |

### Validation Errors (`ValidationExceptions.cs`)

| Error | Code | Description | HTTP Status |
|-------|------|-------------|-------------|
| `ValidationError` | `VALIDATION_ERROR` | Generic validation error | 400 |
| `RequiredFieldError` | `REQUIRED_FIELD` | Required field | 400 |
| `InvalidEmailFormatError` | `INVALID_EMAIL_FORMAT` | Invalid email format | 400 |
| `PasswordTooWeakError` | `PASSWORD_TOO_WEAK` | Weak password | 400 |
| `InvalidDateOfBirthError` | `INVALID_DATE_OF_BIRTH` | Invalid date of birth | 400 |
| `InvalidAgeError` | `INVALID_AGE` | Invalid age | 400 |
| `UsernameTooShortError` | `USERNAME_TOO_SHORT` | Username too short | 400 |
| `UsernameTooLongError` | `USERNAME_TOO_LONG` | Username too long | 400 |
| `InvalidUsernameFormatError` | `INVALID_USERNAME_FORMAT` | Invalid username format | 400 |

### Permission Errors (`PermissionExceptions.cs`)

| Error | Code | Description | HTTP Status |
|-------|------|-------------|-------------|
| `InsufficientPermissionsError` | `INSUFFICIENT_PERMISSIONS` | Insufficient permissions | 403 |
| `RoleNotFoundError` | `ROLE_NOT_FOUND` | Role not found | 404 |
| `RoleNotFoundByIdError` | `ROLE_NOT_FOUND_BY_ID` | Role not found by ID | 404 |
| `RoleAlreadyExistsError` | `ROLE_ALREADY_EXISTS` | Role already exists | 409 |
| `PermissionNotFoundError` | `PERMISSION_NOT_FOUND` | Permission not found | 404 |
| `PermissionNotFoundByIdError` | `PERMISSION_NOT_FOUND_BY_ID` | Permission not found by ID | 404 |
| `PermissionAlreadyExistsError` | `PERMISSION_ALREADY_EXISTS` | Permission already exists | 409 |
| `UserNotInRoleError` | `USER_NOT_IN_ROLE` | User not in role | 400 |
| `RolePermissionNotFoundError` | `ROLE_PERMISSION_NOT_FOUND` | Role doesn't have permission | 404 |

## 🔄 Exception Handling Middleware

The `ExceptionHandlingMiddleware` runs automatically and handles all unhandled exceptions:

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

### Configuration in Program.cs

```csharp
// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

**Features:**
- ✅ Automatic capture of all exceptions
- ✅ Error code mapping to appropriate HTTP codes
- ✅ Automatic message localization
- ✅ Structured error logging
- ✅ Consistent JSON responses

## 🌍 Error Localization

### Localization Files

Error messages are localized using JSON files:

#### Spanish (`es.json`)
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

#### English (`en.json`)
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

### Language Configuration

```csharp
var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
```

**Supported languages:**
- 🇺🇸 **English (en)** - Default language
- 🇪🇸 **Spanish (es)** - Alternative language

## 📊 Standard Responses

### Error Response Structure

```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-01-13T12:00:00.000Z",
  "requestId": "req_123456789"
}
```

### Success Response Structure

```json
{
  "success": true,
  "message": "Usuario creado exitosamente",
  "data": {
    "id": "guid",
    "firstName": "John",
    "lastName": "Doe"
  },
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

### ApiResponse Class

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

## 💡 Usage Examples

### 1. In Command Handlers

```csharp
public async Task<ApiResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
{
    var existingUser = await _userManager.FindByEmailAsync(request.Email);
    if (existingUser != null)
    {
        throw new UserAlreadyExistsError("email", request.Email);
    }
    
    // ... rest of the code
}
```

### 2. In Services

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
    
    // ... rest of the code
}
```

### 3. In Controllers (Optional)

Controllers can handle specific exceptions if needed:

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
        // Middleware will automatically handle these exceptions
        throw;
    }
}
```

## 🔢 HTTP Codes

### Automatic Code Mapping

| Error Type | HTTP Code | Description |
|------------|-----------|-------------|
| `UserNotFoundError` | 404 | Not Found |
| `InvalidCredentialsError` | 401 | Unauthorized |
| `AccountDeactivatedError` | 403 | Forbidden |
| `UserAlreadyExistsError` | 409 | Conflict |
| `InvalidPasswordError` | 400 | Bad Request |
| `InsufficientPermissionsError` | 403 | Forbidden |
| `ValidationError` | 400 | Bad Request |
| `UnauthorizedAccessException` | 401 | Unauthorized |
| `ArgumentException` | 400 | Bad Request |
| Other errors | 500 | Internal Server Error |

### Mapping Function

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

## ✅ Best Practices

### 1. **Use Specific Exceptions**

❌ **Bad:**
```csharp
throw new Exception("User not found");
```

✅ **Good:**
```csharp
throw new UserNotFoundError(request.Email);
```

### 2. **Include Context in Exceptions**

❌ **Bad:**
```csharp
throw new UserNotFoundError("User not found");
```

✅ **Good:**
```csharp
throw new UserNotFoundError(request.Email);
```

### 3. **Don't Handle Exceptions in Controllers**

❌ **Bad:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    try
    {
        // ... code
    }
    catch (UserNotFoundError ex)
    {
        return NotFound(ApiResponse.ErrorResponse("User not found"));
    }
}
```

✅ **Good:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    var command = new LoginCommand { Request = request };
    var result = await _mediator.Send(command);
    return Ok(ApiResponse.SuccessResponse(result));
}
```

### 4. **Use Consistent Error Codes**

❌ **Bad:**
```csharp
throw new ApplicationException("USER_NOT_FOUND", "User not found");
throw new ApplicationException("user-not-found", "User not found");
```

✅ **Good:**
```csharp
throw new UserNotFoundError(request.Email); // Always use "USER_NOT_FOUND"
```

### 5. **Localize Error Messages**

❌ **Bad:**
```csharp
throw new ApplicationException("USER_NOT_FOUND", "Usuario no encontrado");
```

✅ **Good:**
```csharp
throw new UserNotFoundError(request.Email); // Middleware automatically localizes
```

### 6. **Error Logging**

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

## 🔍 Debugging and Testing

### Exception Testing

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

### Structured Logging

```csharp
_logger.LogError("User not found with email {Email}", request.Email);
_logger.LogWarning("Invalid password attempt for user {UserId}", user.Id);
_logger.LogInformation("Password reset code generated for user {UserId}", user.Id);
```

## 🚀 Future Extensions

### Possible Improvements

- [ ] **Rate Limiting**: Login attempt limits
- [ ] **Error Analytics**: Metrics for most frequent errors
- [ ] **Error Recovery**: Recovery suggestions
- [ ] **Error Context**: More context in error responses
- [ ] **Error Correlation**: Correlation IDs for debugging
- [ ] **Error Notifications**: Automatic notifications for critical errors

### Integration with External Tools

- [ ] **Sentry**: For production error tracking
- [ ] **Application Insights**: For metrics and analysis
- [ ] **Elasticsearch**: For error log search
- [ ] **Grafana**: For error dashboards

## 📚 References

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Exception Handling](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)

---

**Last updated:** January 13, 2025  
**Version:** 1.0.0