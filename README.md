# Clean Architecture .NET Core Project

A Clean Architecture implementation in .NET Core with Entity Framework Core, ASP.NET Core Identity, and PostgreSQL.

## 📁 Project Structure

```
src/
├── CleanArchitecture.API/          # Presentation layer (Web API)
├── CleanArchitecture.Application/  # Application layer (CQRS, DTOs, Services)
├── CleanArchitecture.Domain/       # Domain layer (Entities, Interfaces)
└── CleanArchitecture.Infrastructure/ # Infrastructure layer (EF Core, Repositories)

docs/                               # Project documentation
├── AUTHENTICATION.md               # Authentication system
├── PERMISSIONS_AND_ROLES.md        # Permissions and roles system
├── ERROR_HANDLING.md               # Error handling system
├── LOCALIZATION_AND_EMAIL.md       # Localization and email system
├── UTC_DATETIME_SYSTEM.md          # UTC DateTime system
└── ...                             # More documentation
```

## 🚀 Key Features

- **Clean Architecture**: Clear separation of concerns
- **CQRS**: Command Query Responsibility Segregation pattern
- **Entity Framework Core**: ORM with PostgreSQL
- **ASP.NET Core Identity**: Authentication and authorization system
- **AutoMapper**: Automatic object mapping
- **MediatR**: Mediator pattern for decoupling
- **UTC DateTime**: Automatic UTC date handling
- **Snake Case**: Database naming convention
- **Soft Delete**: Soft delete support for entities
- **Localization**: Multi-language support (English/Spanish)
- **Email Service**: SMTP email service with templates
- **JWT Authentication**: Secure token-based authentication

## 📚 Documentation

Complete project documentation is available in the `docs/` folder. For API endpoint documentation, see the Swagger UI when running the application.

### **Core Features:**

- [🔐 Authentication](docs/AUTHENTICATION.md) - JWT authentication system
- [👥 Permissions and Roles](docs/PERMISSIONS_AND_ROLES.md) - Authorization system

### **Infrastructure:**

- [📅 UTC DateTime System](docs/UTC_DATETIME_SYSTEM.md) - UTC date handling
- [⚠️ Error Handling](docs/ERROR_HANDLING.md) - Custom exception system
- [📝 Error Handling Examples](docs/ERROR_HANDLING_EXAMPLES.md) - Practical examples

### **Features:**

- [🌐 Localization and Email](docs/LOCALIZATION_AND_EMAIL.md) - Email and localization system
- [🔑 Password Recovery](docs/PASSWORD_RECOVERY.md) - Password recovery flow
- [🎯 Custom Authorization Attributes](docs/CUSTOM_AUTHORIZATION_ATTRIBUTES.md) - Custom authorization attributes

### **Development:**

- [🤖 Scripts and Automation](docs/SCRIPTS_AND_AUTOMATION.md) - Development scripts
- [🌍 Environment Validation](docs/ENVIRONMENT_VALIDATION.md) - Environment validation system
- [📋 Environment Examples](docs/ENVIRONMENT_EXAMPLES.md) - Configuration examples

## 🛠️ Quick Setup

### **Prerequisites:**

- .NET 9.0 SDK
- PostgreSQL
- Node.js (for development scripts)

### **Installation:**

```bash
# Clone the repository
git clone <repository-url>
cd CleanArchitecture

# Restore dependencies
dotnet restore

# Configure environment variables (required)
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres"

# Configure database
# Edit connection string in appsettings.json or use environment variables

# Run migrations
dotnet ef database update --project src/CleanArchitecture.Infrastructure

# Run the application
dotnet run --project src/CleanArchitecture.API
```

### **Required Environment Variables:**

- `ASPNETCORE_ENVIRONMENT` - Application environment (Development/Staging/Production)
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `JwtSettings__SecretKey` - JWT secret key (minimum 32 characters)

📖 **See complete documentation:** [Environment Validation](docs/ENVIRONMENT_VALIDATION.md)

## 🔧 Useful Commands

```bash
# Create new migration
dotnet ef migrations add MigrationName --project src/CleanArchitecture.Infrastructure

# Apply migrations
dotnet ef database update --project src/CleanArchitecture.Infrastructure

# Run tests
dotnet test

# Build project
dotnet build

# Run with hot reload
dotnet watch run --project src/CleanArchitecture.API
```

## 📋 API Documentation

Complete API documentation with request/response examples, authentication requirements, and detailed endpoint descriptions is available through **Swagger UI** when running the application.

Access Swagger UI at: `https://localhost:7000/swagger` (or your configured URL)

The API includes endpoints for:

- **Authentication** - Login, registration, password recovery, email change
- **Users** - User management, roles, and permissions
- **Roles** - Role management and permissions assignment
- **Permissions** - Permission management and resource grouping
- **Geographic Data** - Countries, states, and cities

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'feat: add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

**Note:** All commit messages must follow the [Conventional Commits](https://www.conventionalcommits.org/) specification in English.

## 📄 License

This project is licensed under the MIT License. See the `LICENSE` file for details.

## 📞 Support

If you have questions or need help, please:

- Review the documentation in `docs/`
- Open an issue in the repository
- Contact the development team

---

**Thank you for using Clean Architecture .NET Core!** 🎉
