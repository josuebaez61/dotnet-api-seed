# Sistema de Autenticación

Este documento describe el sistema de autenticación implementado en la aplicación Clean Architecture.

## Características

- **Autenticación JWT**: Tokens seguros con expiración configurable
- **Refresh Tokens**: Renovación automática de tokens
- **Login con Email o Username**: Flexibilidad en el método de autenticación
- **Registro de usuarios**: Creación de nuevas cuentas
- **Cambio de contraseña**: Actualización segura de credenciales
- **Validación robusta**: FluentValidation para todos los inputs

## Endpoints de Autenticación

### 1. Registro de Usuario
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan.perez@example.com",
  "userName": "jperez",
  "password": "Password123!",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "profilePicture": "https://example.com/photo.jpg"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "guid",
    "firstName": "Juan",
    "lastName": "Pérez",
    "email": "juan.perez@example.com",
    "userName": "jperez",
    "dateOfBirth": "1990-01-01T00:00:00Z",
    "profilePicture": "https://example.com/photo.jpg",
    "createdAt": "2024-01-01T10:00:00Z",
    "updatedAt": null,
    "isActive": true,
    "emailConfirmed": true
  }
}
```

### 2. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "juan.perez@example.com",
  "password": "Password123!"
}
```

**Respuesta:** Misma estructura que el registro.

### 3. Refresh Token
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Respuesta:** Misma estructura que el login.

### 4. Cambio de Contraseña
```http
POST /api/auth/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "Password123!",
  "newPassword": "NewPassword123!"
}
```

**Respuesta:**
```json
{
  "message": "Password changed successfully"
}
```

### 5. Información del Usuario Actual
```http
GET /api/auth/me
Authorization: Bearer {token}
```

**Respuesta:**
```json
{
  "id": "guid",
  "userName": "jperez",
  "email": "juan.perez@example.com",
  "firstName": "Juan",
  "lastName": "Pérez",
  "isActive": "true"
}
```

## Configuración JWT

La configuración JWT se encuentra en `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "CleanArchitecture",
    "Audience": "CleanArchitectureUsers",
    "ExpiryHours": 1
  }
}
```

### Parámetros:
- **SecretKey**: Clave secreta para firmar los tokens (mínimo 32 caracteres)
- **Issuer**: Emisor del token
- **Audience**: Audiencia del token
- **ExpiryHours**: Tiempo de expiración en horas

## Validaciones

### Registro:
- **FirstName**: Requerido, máximo 100 caracteres
- **LastName**: Requerido, máximo 100 caracteres
- **Email**: Requerido, formato válido, máximo 256 caracteres, único
- **UserName**: Requerido, 3-50 caracteres, solo letras, números, guiones y guiones bajos, único
- **Password**: Requerido, mínimo 8 caracteres, debe incluir:
  - Al menos una letra minúscula
  - Al menos una letra mayúscula
  - Al menos un dígito
  - Al menos un carácter especial (@$!%*?&)
- **DateOfBirth**: Requerido, debe ser en el pasado, máximo 120 años
- **ProfilePicture**: Opcional, máximo 500 caracteres

### Login:
- **EmailOrUsername**: Requerido, máximo 256 caracteres
- **Password**: Requerido

### Cambio de Contraseña:
- **CurrentPassword**: Requerido
- **NewPassword**: Requerido, mismo formato que en registro

## Uso en Endpoints Protegidos

Para acceder a endpoints protegidos, incluir el token JWT en el header:

```http
Authorization: Bearer {token}
```

### Ejemplo:
```http
GET /api/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Seguridad

### Características de Seguridad:
1. **Tokens JWT firmados**: Imposible falsificar sin la clave secreta
2. **Expiración de tokens**: Tokens expiran automáticamente
3. **Refresh tokens**: Renovación segura sin re-autenticación
4. **Validación de entrada**: FluentValidation previene ataques de inyección
5. **Contraseñas hasheadas**: ASP.NET Core Identity maneja el hashing
6. **HTTPS**: Comunicación encriptada (en producción)

### Mejores Prácticas:
1. **Cambiar la clave secreta** en producción
2. **Usar HTTPS** en producción
3. **Implementar rate limiting** para prevenir ataques de fuerza bruta
4. **Logs de auditoría** para intentos de autenticación
5. **Almacenar refresh tokens** en base de datos en producción

## Flujo de Autenticación

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant AuthService
    participant Database

    Client->>API: POST /api/auth/register
    API->>AuthService: RegisterAsync()
    AuthService->>Database: Create user
    AuthService->>AuthService: Generate JWT + Refresh Token
    AuthService->>API: AuthResponseDto
    API->>Client: Token + User Info

    Client->>API: POST /api/auth/login
    API->>AuthService: LoginAsync()
    AuthService->>Database: Validate credentials
    AuthService->>AuthService: Generate JWT + Refresh Token
    AuthService->>API: AuthResponseDto
    API->>Client: Token + User Info

    Client->>API: GET /api/users (with Bearer token)
    API->>API: Validate JWT
    API->>Client: Protected data

    Client->>API: POST /api/auth/refresh-token
    API->>AuthService: RefreshTokenAsync()
    AuthService->>AuthService: Validate refresh token
    AuthService->>AuthService: Generate new tokens
    AuthService->>API: AuthResponseDto
    API->>Client: New tokens
```

## Manejo de Errores

### Errores Comunes:

**401 Unauthorized:**
- Credenciales inválidas
- Token expirado
- Token inválido

**400 Bad Request:**
- Datos de entrada inválidos
- Usuario ya existe (registro)
- Contraseña actual incorrecta (cambio)

**422 Unprocessable Entity:**
- Errores de validación de FluentValidation

## Testing

### Ejemplos de Pruebas:

1. **Registro exitoso:**
```bash
curl -X POST "https://localhost:7000/api/auth/register" \
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

2. **Login exitoso:**
```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "test@example.com",
    "password": "TestPassword123!"
  }'
```

3. **Acceso a endpoint protegido:**
```bash
curl -X GET "https://localhost:7000/api/users" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```
