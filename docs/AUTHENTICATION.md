# Authentication System

This document describes the authentication system implemented in the Clean Architecture application.

## Features

- **JWT Authentication**: Secure tokens with configurable expiration
- **Refresh Tokens**: Automatic token renewal
- **Login with Email or Username**: Flexibility in authentication method
- **User Registration**: New account creation
- **Password Change**: Secure credential updates
- **Email Change**: Email address change with verification
- **Robust Validation**: FluentValidation for all inputs

## Authentication Endpoints

### 1. User Registration

```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "userName": "jdoe",
  "password": "Password123!",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "profilePicture": "https://example.com/photo.jpg"
}
```

**Response:**

```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiresAt": "2024-01-01T12:00:00Z",
    "user": {
      "id": "guid",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@example.com",
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

### 2. Login

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "emailOrUsername": "john.doe@example.com",
  "password": "Password123!"
}
```

**Response:** Same structure as registration.

### 3. Refresh Token

```http
POST /api/v1/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response:** Same structure as login.

### 4. Change Password

```http
POST /api/v1/auth/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "Password123!",
  "newPassword": "NewPassword123!"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Password changed successfully",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 5. Current User Information

```http
GET /api/v1/auth/me
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "message": "",
  "data": {
    "id": "guid",
    "userName": "jdoe",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isActive": true
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 6. Request Email Change

```http
POST /api/v1/auth/request-email-change
Authorization: Bearer {token}
Content-Type: application/json

{
  "newEmail": "new.email@example.com"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Email change verification sent",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 7. Verify Email Change

```http
POST /api/v1/auth/verify-email-change
Content-Type: application/json

{
  "verificationCode": "verification-code-from-email"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Email changed successfully",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 8. Request Password Reset

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
    "expiresAt": "2024-01-01T12:15:00Z"
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 9. Reset Password

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

## JWT Configuration

JWT configuration is found in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "CleanArchitecture",
    "Audience": "CleanArchitectureUsers",
    "ExpiryMinutes": 15
  }
}
```

### Parameters:

- **SecretKey**: Secret key for signing tokens (minimum 32 characters)
- **Issuer**: Token issuer
- **Audience**: Token audience
- **ExpiryHours**: Expiration time in hours

## Validations

### Registration:

- **FirstName**: Required, maximum 100 characters
- **LastName**: Required, maximum 100 characters
- **Email**: Required, valid format, maximum 256 characters, unique
- **UserName**: Required, 3-50 characters, only letters, numbers, hyphens and underscores, unique
- **Password**: Required, minimum 8 characters, must include:
  - At least one lowercase letter
  - At least one uppercase letter
  - At least one digit
  - At least one special character (@$!%\*?&)
- **DateOfBirth**: Required, must be in the past, maximum 120 years
- **ProfilePicture**: Optional, maximum 500 characters

### Login:

- **EmailOrUsername**: Required, maximum 256 characters
- **Password**: Required

### Change Password:

- **CurrentPassword**: Required
- **NewPassword**: Required, same format as registration

### Email Change:

- **NewEmail**: Required, valid format, maximum 256 characters, unique

### Password Reset:

- **Email**: Required, valid format
- **Code**: Required, exactly 6 digits
- **NewPassword**: Required, same format as registration

## Usage in Protected Endpoints

To access protected endpoints, include the JWT token in the header:

```http
Authorization: Bearer {token}
```

### Example:

```http
GET /api/v1/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Security

### Security Features:

1. **Signed JWT tokens**: Impossible to forge without the secret key
2. **Token expiration**: Tokens expire automatically
3. **Refresh tokens**: Secure renewal without re-authentication
4. **Input validation**: FluentValidation prevents injection attacks
5. **Hashed passwords**: ASP.NET Core Identity handles hashing
6. **HTTPS**: Encrypted communication (in production)

### Best Practices:

1. **Change the secret key** in production
2. **Use HTTPS** in production
3. **Implement rate limiting** to prevent brute force attacks
4. **Audit logs** for authentication attempts
5. **Store refresh tokens** in database in production

## Authentication Flow

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant AuthService
    participant Database

    Client->>API: POST /api/v1/auth/register
    API->>AuthService: RegisterAsync()
    AuthService->>Database: Create user
    AuthService->>AuthService: Generate JWT + Refresh Token
    AuthService->>API: AuthResponseDto
    API->>Client: Token + User Info

    Client->>API: POST /api/v1/auth/login
    API->>AuthService: LoginAsync()
    AuthService->>Database: Validate credentials
    AuthService->>AuthService: Generate JWT + Refresh Token
    AuthService->>API: AuthResponseDto
    API->>Client: Token + User Info

    Client->>API: GET /api/v1/users (with Bearer token)
    API->>API: Validate JWT
    API->>Client: Protected data

    Client->>API: POST /api/v1/auth/refresh-token
    API->>AuthService: RefreshTokenAsync()
    AuthService->>AuthService: Validate refresh token
    AuthService->>AuthService: Generate new tokens
    AuthService->>API: AuthResponseDto
    API->>Client: New tokens
```

## Error Handling

### Common Errors:

**401 Unauthorized:**

- Invalid credentials
- Expired token
- Invalid token

**400 Bad Request:**

- Invalid input data
- User already exists (registration)
- Incorrect current password (change)

**422 Unprocessable Entity:**

- FluentValidation errors

## Testing

### Test Examples:

1. **Successful registration:**

```bash
curl -X POST "https://localhost:7000/api/v1/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "email": "test@example.com",
    "userName": "testuser",
    "password": "TestPassword123!",
    "dateOfBirth": "1990-01-01T00:00:00Z"
  }'
```

2. **Successful login:**

```bash
curl -X POST "https://localhost:7000/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "test@example.com",
    "password": "TestPassword123!"
  }'
```

3. **Access protected endpoint:**

```bash
curl -X GET "https://localhost:7000/api/v1/users" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

4. **Request password reset:**

```bash
curl -X POST "https://localhost:7000/api/v1/auth/request-password-reset" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com"}'
```

5. **Reset password:**

```bash
curl -X POST "https://localhost:7000/api/v1/auth/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "code": "123456",
    "newPassword": "NewPassword123!"
  }'
```
