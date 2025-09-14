# Clean Architecture ASP.NET Core Project

This project implements Clean Architecture with ASP.NET Core, Entity Framework Core, PostgreSQL, Identity, FluentValidation, and MediatR.

## Project Structure

```
CleanArchitecture/
├── docs/                             # Project documentation
│   ├── AUTHENTICATION.md             # Authentication system
│   ├── PASSWORD_RECOVERY.md          # Password recovery
│   ├── PERMISSIONS_AND_ROLES.md      # Permissions and roles system
│   ├── LOCALIZATION_AND_EMAIL.md     # Localization and emails
│   ├── ERROR_HANDLING.md             # Error handling system
│   └── SCRIPTS_AND_AUTOMATION.md     # Scripts and automation
├── src/                              # Application source code
│   ├── CleanArchitecture.Domain/     # Domain Layer
│   │   ├── Entities/                 # Domain entities
│   │   └── Common/                   # Common interfaces
│   ├── CleanArchitecture.Application/ # Application Layer
│   │   ├── DTOs/                     # Data transfer objects
│   │   ├── Features/                 # Use cases (CQRS with MediatR)
│   │   ├── Validators/               # Validators with FluentValidation
│   │   └── DependencyInjection.cs    # DI configuration
│   ├── CleanArchitecture.Infrastructure/ # Infrastructure Layer
│   │   ├── Data/                     # DbContext and database configuration
│   │   ├── Repositories/             # Repository implementations
│   │   └── DependencyInjection.cs    # DI configuration
│   └── CleanArchitecture.API/        # Presentation Layer
│       ├── Controllers/              # API controllers
│       ├── Program.cs                # Application entry point
│       └── appsettings.json         # Application configuration
├── scripts/                          # Automation scripts
├── .vscode/                          # VS Code configuration
├── docker-compose.yml               # Docker configuration
├── package.json                     # npm scripts
├── Makefile                         # make commands
└── CleanArchitecture.sln            # Solution file
```

## Technologies Used

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0**
- **PostgreSQL** (with Npgsql)
- **ASP.NET Core Identity**
- **FluentValidation**
- **MediatR** (CQRS Pattern)
- **Swagger/OpenAPI**

## Database Configuration

### Prerequisites
- PostgreSQL installed and running
- User `postgres` with password `postgres` (or modify the connection string)

### Connection String
The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=CleanArchitectureDB;Username=postgres;Password=postgres"
  }
}
```

### Apply Migrations
```bash
# Apply migrations to the database
dotnet ef database update --project CleanArchitecture.Infrastructure --startup-project CleanArchitecture.API
```

## Running the Project

1. **Clone the repository**
2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```
3. **Build the project**
   ```bash
   dotnet build
   ```
4. **Run the application**
   ```bash
   dotnet run --project CleanArchitecture.API
   ```
5. **Open Swagger UI**
   - Navigate to `https://localhost:7000/swagger` (or the configured port)

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - Login with email/username and password
- `POST /api/v1/auth/refresh-token` - Renew access token
- `POST /api/v1/auth/change-password` - Change password (requires authentication)
- `GET /api/v1/auth/me` - Get current user information
- `POST /api/v1/auth/request-password-reset` - Request password reset
- `POST /api/v1/auth/reset-password` - Reset password with code
- `POST /api/v1/auth/request-email-change` - Request email change
- `POST /api/v1/auth/verify-email-change` - Verify email change

### Users (Require authentication)
- `GET /api/v1/users` - Get all users
- `GET /api/v1/users/{id}` - Get user by ID
- `POST /api/v1/users` - Create new user
- `GET /api/v1/users/paginated` - Get paginated users

### Roles (Require authentication)
- `GET /api/v1/roles` - Get all roles
- `POST /api/v1/roles` - Create new role
- `PATCH /api/v1/roles/{id}/permissions` - Update role permissions

### Permissions (Require authentication)
- `GET /api/v1/permissions` - Get all permissions
- `POST /api/v1/permissions` - Create new permission

### Admin (Require authentication)
- `GET /api/v1/admin/test` - Test admin endpoint
- `POST /api/v1/admin/cleanup-expired-codes` - Cleanup expired codes

### Registration Example
```json
POST /api/v1/auth/register
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

### Login Example
```json
POST /api/v1/auth/login
{
  "emailOrUsername": "john.doe@example.com",
  "password": "Password123!"
}
```

### Authentication Response
```json
{
  "success": true,
  "message": "Login successful",
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

## Architecture

### Clean Architecture Principles
1. **Framework independence**: The domain does not depend on external frameworks
2. **Testability**: Business logic can be tested without external dependencies
3. **UI independence**: The UI can change without affecting the rest of the system
4. **Database independence**: You can change from PostgreSQL to another engine without affecting the domain
5. **External agency independence**: Business rules know nothing about the outside world

### Implemented Patterns
- **Repository Pattern**: For data access
- **Unit of Work**: For transactions
- **CQRS**: Commands and queries separated with MediatR
- **Dependency Injection**: For dependency inversion
- **FluentValidation**: For input validation

## Development

### Adding New Features
1. **Entity**: Create in `CleanArchitecture.Domain/Entities/`
2. **DTO**: Create in `CleanArchitecture.Application/DTOs/`
3. **Validators**: Create in `CleanArchitecture.Application/Validators/`
4. **Commands/Queries**: Create in `CleanArchitecture.Application/Features/`
5. **Controller**: Create in `CleanArchitecture.API/Controllers/`

### Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName --project CleanArchitecture.Infrastructure --startup-project CleanArchitecture.API

# Apply migrations
dotnet ef database update --project CleanArchitecture.Infrastructure --startup-project CleanArchitecture.API
```

## Validations

The project includes robust validations for users:
- **First Name and Last Name**: Required, maximum 100 characters
- **Email**: Valid format, required, maximum 256 characters
- **Password**: Minimum 8 characters, must include uppercase, lowercase, numbers and special characters
- **Date of Birth**: Required, must be in the past, maximum 120 years

## Security

- **Identity Framework**: For authentication and authorization
- **Input validation**: With FluentValidation
- **CORS**: Configured for development
- **HTTPS**: Enabled by default

## Authentication

The project includes a complete JWT authentication system:

- ✅ **JWT Authentication** implemented
- ✅ **Login with email or username**
- ✅ **User registration**
- ✅ **Refresh tokens** for automatic renewal
- ✅ **Password change**
- ✅ **Password recovery** with email codes
- ✅ **Email change** with verification
- ✅ **Robust validations** with FluentValidation
- ✅ **Protected endpoints** with authorization

For more details, see [AUTHENTICATION.md](docs/AUTHENTICATION.md).

## 🌍 Localization and Emails

### Language System
- ✅ **Spanish (es)** and **English (en)** supported
- ✅ **Localized messages** in all responses
- ✅ **Automatic language** configuration
- ✅ **Dynamic language** change via header/query

### Email Service
- ✅ **HTML templates** with modern CSS
- ✅ **Automatic welcome** emails
- ✅ **Password recovery** with 6-digit codes
- ✅ **Email change verification** with links
- ✅ **Password change confirmations**
- ✅ **Flexible SMTP** configuration

### Standardized Responses
- ✅ **Consistent structure** in all responses
- ✅ **Automatically localized** messages
- ✅ **Uniform error** handling
- ✅ **Timestamps** and metadata included

For more details, see [LOCALIZATION_AND_EMAIL.md](docs/LOCALIZATION_AND_EMAIL.md).

## 🔐 Password Recovery

The system includes a complete password recovery system:

- ✅ **Reset request** via email
- ✅ **6-digit secure codes** with expiration
- ✅ **Professional HTML** emails
- ✅ **Robust validations** of codes and passwords
- ✅ **Email confirmation** of successful change
- ✅ **Enhanced security** with single-use codes

For more details, see [PASSWORD_RECOVERY.md](docs/PASSWORD_RECOVERY.md).

## 🔐 Permissions and Roles System

The system includes a complete permissions and roles system:

- ✅ **Permission management** with full CRUD
- ✅ **Role management** with permission assignment
- ✅ **Granular authorization** per endpoint
- ✅ **JWT tokens** with included permissions
- ✅ **Configured authorization** policies
- ✅ **Predefined permissions** for users, roles and permissions
- ✅ **Predefined roles** (Admin, User) with assigned permissions
- ✅ **Permission constants** for type safety

### Available Permissions
- **Users.Read/Write/Delete** - User management
- **Roles.Read/Write** - Role management
- **Permissions.Read/Write** - Permission management
- **System.Admin** - System administration

For more details, see [PERMISSIONS_AND_ROLES.md](docs/PERMISSIONS_AND_ROLES.md).

## 🚨 Error Handling System

The system includes robust and consistent error handling:

- ✅ **Specific exceptions** with unique error codes
- ✅ **Automatic localization** of error messages
- ✅ **Centralized middleware** for exception handling
- ✅ **Appropriate HTTP codes** automatically mapped
- ✅ **Structured responses** with consistent metadata
- ✅ **Structured logging** for debugging and monitoring
- ✅ **Simplified testing** with specific exceptions

### Supported Error Types
- **Authentication**: `UserNotFoundError`, `InvalidCredentialsError`, etc.
- **Validation**: `RequiredFieldError`, `InvalidEmailFormatError`, etc.
- **Permissions**: `InsufficientPermissionsError`, `RoleNotFoundError`, etc.

For more details, see [ERROR_HANDLING.md](docs/ERROR_HANDLING.md) and [ERROR_HANDLING_EXAMPLES.md](docs/ERROR_HANDLING_EXAMPLES.md).

## 🛠️ Scripts and Automation

The project includes multiple options for automating common tasks:

### **npm Scripts (Recommended)**
```bash
npm run setup          # Complete project setup
npm run dev            # Start development server
npm run db:migrate     # Run migrations
npm run docker:up      # Start Docker containers
npm run swagger        # Open Swagger UI
npm run pgadmin        # Open pgAdmin
```

### **Bash/PowerShell Scripts**
```bash
./scripts/setup.sh     # Initial setup (Unix/Linux/macOS)
.\scripts\setup.ps1    # Initial setup (Windows)
./scripts/dev.sh       # Development environment
./scripts/db.sh        # Database management
```

### **Makefile**
```bash
make setup             # Complete setup
make dev               # Start development
make migrate           # Run migrations
make help              # See all commands
```

### **VS Code Tasks**
- `Ctrl+Shift+P` → "Tasks: Run Task"
- Preconfigured tasks for Build, Run, Test, etc.

For more details, see [SCRIPTS_AND_AUTOMATION.md](docs/SCRIPTS_AND_AUTOMATION.md).

## Features Implemented

- ✅ **Clean Architecture** implementation
- ✅ **JWT Authentication** with refresh tokens
- ✅ **User Registration** and management
- ✅ **Password Recovery** with email codes
- ✅ **Email Change** with verification
- ✅ **Role-based Authorization** with permissions
- ✅ **Localization** (Spanish/English) with .resx files
- ✅ **Email Service** with HTML templates
- ✅ **Error Handling** with centralized middleware
- ✅ **Pagination** for database entities
- ✅ **Soft Delete** for sensitive data
- ✅ **Background Services** for cleanup tasks
- ✅ **Repository Pattern** implementation
- ✅ **Permission Constants** for type safety
- ✅ **Standardized API** responses
- ✅ **Global API prefix** (/api/v1)
- ✅ **Comprehensive Documentation**

## Next Steps

- ✅ Add structured logging
- ✅ Implement cache with Redis
- ✅ Add unit and integration tests
- ✅ Implement pagination
- ✅ Add filters and search
- ✅ Implement soft delete
- ✅ Add entity auditing
- ✅ Implement roles and permissions
- ✅ Add email confirmation
- ✅ Implement password recovery
- ✅ Add email change functionality
- ✅ Implement background cleanup services
- ✅ Add permission constants