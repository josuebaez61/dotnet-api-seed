# 🔍 Practical Error Handling System Examples

This document contains practical examples of how the error handling system works in different scenarios.

## 📋 Table of Contents

- [API Examples](#api-examples)
- [Code Examples](#code-examples)
- [Common Use Cases](#common-use-cases)
- [Error Testing](#error-testing)

## 🌐 API Examples

### 1. Login with Non-existent User

**Request:**

```bash
POST /api/v1/auth/login
Content-Type: application/json

{
  "emailOrUsername": "nonexistent.user@example.com",
  "password": "Password123!"
}
```

**Response (Spanish):**

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

### 2. Registration with Duplicate Email

**Request:**

```bash
POST /api/v1/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "admin@example.com",
  "userName": "jdoe",
  "password": "Password123!",
  "dateOfBirth": "1990-01-01T00:00:00Z"
}
```

**Response:**

```json
{
  "success": false,
  "message": "User already exists",
  "errorCode": "USER_ALREADY_EXISTS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `409 Conflict`

### 3. Incorrect Password

**Request:**

```bash
POST /api/v1/auth/login
Content-Type: application/json

{
  "emailOrUsername": "admin",
  "password": "wrong_password"
}
```

**Response:**

```json
{
  "success": false,
  "message": "Invalid credentials",
  "errorCode": "INVALID_CREDENTIALS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `401 Unauthorized`

### 4. Invalid Refresh Token

**Request:**

```bash
POST /api/v1/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "invalid_or_expired_token"
}
```

**Response:**

```json
{
  "success": false,
  "message": "Invalid refresh token",
  "errorCode": "INVALID_REFRESH_TOKEN",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `401 Unauthorized`

### 5. Invalid Reset Code

**Request:**

```bash
POST /api/v1/auth/reset-password
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
  "message": "Invalid reset code",
  "errorCode": "PASSWORD_RESET_CODE_INVALID",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `400 Bad Request`

### 6. Insufficient Permissions

**Request:**

```bash
GET /api/v1/users
Authorization: Bearer token_with_insufficient_permissions
```

**Response:**

```json
{
  "success": false,
  "message": "Insufficient permissions. Required: Users.Read",
  "errorCode": "INSUFFICIENT_PERMISSIONS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `403 Forbidden`

### 7. Data Validation

**Request:**

```bash
POST /api/v1/auth/register
Content-Type: application/json

{
  "firstName": "",
  "lastName": "Doe",
  "email": "invalid_email",
  "userName": "a",
  "password": "123",
  "dateOfBirth": "2030-01-01T00:00:00Z"
}
```

**Response:**

```json
{
  "success": false,
  "message": "Validation error",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "FirstName": ["The 'FirstName' field is required"],
    "Email": ["Invalid email format"],
    "UserName": ["Username must be at least 3 characters long"],
    "Password": ["Password must be at least 8 characters long"]
  },
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

**HTTP Status:** `400 Bad Request`

## 💻 Code Examples

### 1. Command Handler with Error Handling

```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly ILocalizationService _localizationService;

    public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
        {
            throw new UserAlreadyExistsError("email", request.Email);
        }

        // Check if username already exists
        var existingUserByUsername = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserByUsername != null)
        {
            throw new UserAlreadyExistsError("username", request.UserName);
        }

        // Validate date of birth
        var age = DateTime.UtcNow.Year - request.DateOfBirth.Year;
        if (age < 13 || age > 120)
        {
            throw new InvalidAgeError(age);
        }

        // Create user
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
            _localizationService.GetSuccessMessage("USER_CREATED")
        );
    }
}
```

### 2. Service with Validations

```csharp
public class AuthService : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.EmailOrUsername))
        {
            throw new RequiredFieldError("EmailOrUsername");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new RequiredFieldError("Password");
        }

        // Find user
        var user = await GetUserByEmailOrUsernameAsync(request.EmailOrUsername);
        if (user == null)
        {
            throw new UserNotFoundError(request.EmailOrUsername);
        }

        // Check account status
        if (!user.IsActive)
        {
            throw new AccountDeactivatedError(user.Id.ToString());
        }

        // Verify password
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new InvalidCredentialsError();
        }

        // Generate tokens
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

### 3. Simplified Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        // No try-catch needed, middleware handles everything
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

## 🎯 Common Use Cases

### 1. Complete Login Flow

```csharp
// 1. User attempts login with incorrect credentials
var loginRequest = new LoginRequestDto
{
    EmailOrUsername = "admin@example.com",
    Password = "wrong_password"
};

// 2. AuthService validates and throws exception
try
{
    var result = await _authService.LoginAsync(loginRequest);
}
catch (InvalidCredentialsError ex)
{
    // 3. Middleware captures the exception
    // 4. Localizes the message according to user's language
    // 5. Returns structured response
}
```

**Result:**

```json
{
  "success": false,
  "message": "Invalid credentials",
  "errorCode": "INVALID_CREDENTIALS",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

### 2. Registration Flow with Validations

```csharp
// 1. User attempts registration with invalid data
var registerRequest = new RegisterRequestDto
{
    FirstName = "", // Invalid
    Email = "invalid_email", // Invalid
    Password = "123" // Invalid
};

// 2. FluentValidation validates and throws exception
// 3. Middleware captures and localizes
// 4. Returns response with detailed errors
```

**Result:**

```json
{
  "success": false,
  "message": "Validation error",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "FirstName": ["The 'FirstName' field is required"],
    "Email": ["Invalid email format"],
    "Password": ["Password must be at least 8 characters long"]
  },
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

### 3. Password Recovery Flow

```csharp
// 1. User requests reset with non-existent email
var resetRequest = new RequestPasswordResetDto
{
    Email = "nonexistent.user@example.com"
};

// 2. AuthService searches for user
var user = await _userManager.FindByEmailAsync(resetRequest.Email);
if (user == null)
{
    throw new UserNotFoundError(resetRequest.Email);
}

// 3. Middleware handles and localizes
// 4. Returns 404 error with localized message
```

**Result:**

```json
{
  "success": false,
  "message": "User not found",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2025-01-13T12:00:00.000Z"
}
```

## 🧪 Error Testing

### 1. Unit Test for Exceptions

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

### 2. API Integration Test

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
    var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);

    Assert.False(apiResponse.Success);
    Assert.Equal("INVALID_CREDENTIALS", apiResponse.ErrorCode);
    Assert.Equal("Invalid credentials", apiResponse.Message);
}

[Test]
public async Task Register_WithInvalidData_ReturnsBadRequest()
{
    // Arrange
    var request = new RegisterRequestDto
    {
        FirstName = "", // Invalid
        Email = "invalid_email", // Invalid
        Password = "123" // Invalid
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);

    Assert.False(apiResponse.Success);
    Assert.Equal("VALIDATION_ERROR", apiResponse.ErrorCode);
}
```

### 3. Localization Test

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
    var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

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
    var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

    // Assert
    var content = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);

    Assert.Equal("User not found", apiResponse.Message);
}
```

## 🔧 Testing Configuration

### 1. Test Base Setup

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

### 2. Service Mocking

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

## 📊 Monitoring and Logging

### 1. Structured Logging

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
            // Structured logging with context
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

### 2. Error Metrics

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

**Last updated:** January 13, 2025  
**Version:** 1.0.0
