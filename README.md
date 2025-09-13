# Clean Architecture ASP.NET Core Project

Este proyecto implementa una arquitectura limpia (Clean Architecture) con ASP.NET Core, Entity Framework Core, PostgreSQL, Identity, FluentValidation y MediatR.

## Estructura del Proyecto

```
CleanArchitecture/
├── docs/                             # Documentación del proyecto
│   ├── AUTHENTICATION.md             # Sistema de autenticación
│   ├── PASSWORD_RECOVERY.md          # Recuperación de contraseña
│   ├── PERMISSIONS_AND_ROLES.md      # Sistema de permisos y roles
│   ├── LOCALIZATION_AND_EMAIL.md     # Localización y correos
│   └── SCRIPTS_AND_AUTOMATION.md     # Scripts y automatización
├── src/                              # Código fuente de la aplicación
│   ├── CleanArchitecture.Domain/     # Capa de Dominio
│   │   ├── Entities/                 # Entidades del dominio
│   │   └── Common/                   # Interfaces comunes
│   ├── CleanArchitecture.Application/ # Capa de Aplicación
│   │   ├── DTOs/                     # Objetos de transferencia de datos
│   │   ├── Features/                 # Casos de uso (CQRS con MediatR)
│   │   ├── Validators/               # Validadores con FluentValidation
│   │   └── DependencyInjection.cs    # Configuración de DI
│   ├── CleanArchitecture.Infrastructure/ # Capa de Infraestructura
│   │   ├── Data/                     # DbContext y configuración de BD
│   │   ├── Repositories/             # Implementación de repositorios
│   │   └── DependencyInjection.cs    # Configuración de DI
│   └── CleanArchitecture.API/        # Capa de Presentación
│       ├── Controllers/              # Controladores de la API
│       ├── Program.cs                # Punto de entrada de la aplicación
│       └── appsettings.json         # Configuración de la aplicación
├── scripts/                          # Scripts de automatización
├── .vscode/                          # Configuración de VS Code
├── docker-compose.yml               # Configuración de Docker
├── package.json                     # Scripts npm
├── Makefile                         # Comandos make
└── CleanArchitecture.sln            # Archivo de solución
```

## Tecnologías Utilizadas

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0**
- **PostgreSQL** (con Npgsql)
- **ASP.NET Core Identity**
- **FluentValidation**
- **MediatR** (CQRS Pattern)
- **Swagger/OpenAPI**

## Configuración de la Base de Datos

### Prerequisitos
- PostgreSQL instalado y ejecutándose
- Usuario `postgres` con contraseña `postgres` (o modificar la cadena de conexión)

### Cadena de Conexión
La cadena de conexión está configurada en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=CleanArchitectureDB;Username=postgres;Password=postgres"
  }
}
```

### Aplicar Migraciones
```bash
# Aplicar migraciones a la base de datos
dotnet ef database update --project CleanArchitecture.Infrastructure --startup-project CleanArchitecture.API
```

## Ejecutar el Proyecto

1. **Clonar el repositorio**
2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```
3. **Compilar el proyecto**
   ```bash
   dotnet build
   ```
4. **Ejecutar la aplicación**
   ```bash
   dotnet run --project CleanArchitecture.API
   ```
5. **Abrir Swagger UI**
   - Navegar a `https://localhost:7000/swagger` (o el puerto configurado)

## API Endpoints

### Autenticación
- `POST /api/auth/register` - Registro de usuario
- `POST /api/auth/login` - Login con email/username y password
- `POST /api/auth/refresh-token` - Renovar token de acceso
- `POST /api/auth/change-password` - Cambiar contraseña (requiere autenticación)
- `GET /api/auth/me` - Obtener información del usuario actual

### Usuarios (Requieren autenticación)
- `GET /api/users` - Obtener todos los usuarios
- `GET /api/users/{id}` - Obtener usuario por ID
- `POST /api/users` - Crear nuevo usuario

### Ejemplo de Registro
```json
POST /api/auth/register
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

### Ejemplo de Login
```json
POST /api/auth/login
{
  "emailOrUsername": "juan.perez@example.com",
  "password": "Password123!"
}
```

### Respuesta de Autenticación
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

## Arquitectura

### Principios de Clean Architecture
1. **Independencia de frameworks**: El dominio no depende de frameworks externos
2. **Testabilidad**: La lógica de negocio puede ser probada sin dependencias externas
3. **Independencia de UI**: La UI puede cambiar sin afectar el resto del sistema
4. **Independencia de base de datos**: Se puede cambiar de PostgreSQL a otro motor sin afectar el dominio
5. **Independencia de agentes externos**: Las reglas de negocio no conocen nada del mundo exterior

### Patrones Implementados
- **Repository Pattern**: Para el acceso a datos
- **Unit of Work**: Para transacciones
- **CQRS**: Comandos y consultas separados con MediatR
- **Dependency Injection**: Para la inversión de dependencias
- **FluentValidation**: Para validación de entrada

## Desarrollo

### Agregar Nuevas Funcionalidades
1. **Entidad**: Crear en `CleanArchitecture.Domain/Entities/`
2. **DTO**: Crear en `CleanArchitecture.Application/DTOs/`
3. **Validadores**: Crear en `CleanArchitecture.Application/Validators/`
4. **Comandos/Consultas**: Crear en `CleanArchitecture.Application/Features/`
5. **Controlador**: Crear en `CleanArchitecture.API/Controllers/`

### Migraciones
```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion --project CleanArchitecture.Infrastructure --startup-project CleanArchitecture.API

# Aplicar migraciones
dotnet ef database update --project CleanArchitecture.Infrastructure --startup-project CleanArchitecture.API
```

## Validaciones

El proyecto incluye validaciones robustas para usuarios:
- **Nombre y Apellido**: Requeridos, máximo 100 caracteres
- **Email**: Formato válido, requerido, máximo 256 caracteres
- **Contraseña**: Mínimo 8 caracteres, debe incluir mayúsculas, minúsculas, números y caracteres especiales
- **Fecha de Nacimiento**: Requerida, debe ser en el pasado, máximo 120 años

## Seguridad

- **Identity Framework**: Para autenticación y autorización
- **Validación de entrada**: Con FluentValidation
- **CORS**: Configurado para desarrollo
- **HTTPS**: Habilitado por defecto

## Autenticación

El proyecto incluye un sistema completo de autenticación con JWT:

- ✅ **Autenticación JWT** implementada
- ✅ **Login con email o username**
- ✅ **Registro de usuarios**
- ✅ **Refresh tokens** para renovación automática
- ✅ **Cambio de contraseña**
- ✅ **Recuperación de contraseña** con códigos por email
- ✅ **Validaciones robustas** con FluentValidation
- ✅ **Endpoints protegidos** con autorización

Para más detalles, consulta [AUTHENTICATION.md](AUTHENTICATION.md).

## 🌍 Localización y Correos

### Sistema de Idiomas
- ✅ **Español (es)** e **Inglés (en)** soportados
- ✅ **Mensajes localizados** en todas las respuestas
- ✅ **Configuración automática** de idiomas
- ✅ **Cambio dinámico** de idioma por header/query

### Servicio de Correos
- ✅ **Templates HTML** con CSS moderno
- ✅ **Correos de bienvenida** automáticos
- ✅ **Recuperación de contraseña** con códigos de 6 dígitos
- ✅ **Confirmaciones** de cambio de contraseña
- ✅ **Configuración SMTP** flexible

### Respuestas Estandarizadas
- ✅ **Estructura consistente** en todas las respuestas
- ✅ **Mensajes localizados** automáticamente
- ✅ **Manejo de errores** uniforme
- ✅ **Timestamps** y metadatos incluidos

Para más detalles, consulta [LOCALIZATION_AND_EMAIL.md](LOCALIZATION_AND_EMAIL.md).

## 🔐 Recuperación de Contraseña

El sistema incluye un sistema completo de recuperación de contraseña:

- ✅ **Solicitud de reset** por email
- ✅ **Códigos de 6 dígitos** seguros con expiración
- ✅ **Correos HTML** con diseño profesional
- ✅ **Validaciones robustas** de códigos y contraseñas
- ✅ **Confirmación por email** del cambio exitoso
- ✅ **Seguridad mejorada** con códigos de un solo uso

Para más detalles, consulta [PASSWORD_RECOVERY.md](PASSWORD_RECOVERY.md).

## 🔐 Sistema de Permisos y Roles

El sistema incluye un sistema completo de permisos y roles:

- ✅ **Gestión de permisos** con CRUD completo
- ✅ **Gestión de roles** con asignación de permisos
- ✅ **Autorización granular** por endpoint
- ✅ **Tokens JWT** con permisos incluidos
- ✅ **Políticas de autorización** configuradas
- ✅ **Permisos predefinidos** para usuarios, roles y permisos
- ✅ **Roles predefinidos** (Admin, User) con permisos asignados

### Permisos Disponibles
- **Users.Read/Write/Delete** - Gestión de usuarios
- **Roles.Read/Write** - Gestión de roles
- **Permissions.Read/Write** - Gestión de permisos

Para más detalles, consulta [PERMISSIONS_AND_ROLES.md](PERMISSIONS_AND_ROLES.md).

## 🛠️ Scripts y Automatización

El proyecto incluye múltiples opciones para automatizar tareas comunes:

### **npm Scripts (Recomendado)**
```bash
npm run setup          # Configuración completa del proyecto
npm run dev            # Iniciar servidor de desarrollo
npm run db:migrate     # Ejecutar migraciones
npm run docker:up      # Iniciar contenedores Docker
npm run swagger        # Abrir Swagger UI
npm run pgadmin        # Abrir pgAdmin
```

### **Scripts de Bash/PowerShell**
```bash
./scripts/setup.sh     # Configuración inicial (Unix/Linux/macOS)
.\scripts\setup.ps1    # Configuración inicial (Windows)
./scripts/dev.sh       # Entorno de desarrollo
./scripts/db.sh        # Gestión de base de datos
```

### **Makefile**
```bash
make setup             # Configuración completa
make dev               # Iniciar desarrollo
make migrate           # Ejecutar migraciones
make help              # Ver todos los comandos
```

### **VS Code Tasks**
- `Ctrl+Shift+P` → "Tasks: Run Task"
- Tareas preconfiguradas para Build, Run, Test, etc.

Para más detalles, consulta [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md).

## Próximos Pasos

- [ ] Agregar logging estructurado
- [ ] Implementar cache con Redis
- [ ] Agregar tests unitarios e integración
- [ ] Implementar paginación
- [ ] Agregar filtros y búsqueda
- [ ] Implementar soft delete
- [ ] Agregar auditoría de entidades
- [ ] Implementar roles y permisos
- [ ] Agregar confirmación de email
- [ ] Implementar recuperación de contraseña
