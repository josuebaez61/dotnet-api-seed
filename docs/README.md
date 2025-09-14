# 📚 Clean Architecture Project Documentation

Welcome to the complete documentation of the Clean Architecture ASP.NET Core project. This documentation will guide you through all the implemented features and how to use them.

## 📋 Available Documents

### 🔐 [AUTHENTICATION.md](AUTHENTICATION.md)
**Complete Authentication System**
- ✅ JWT authentication with access and renewal tokens
- ✅ Login with email or username
- ✅ User registration with validations
- ✅ Password change
- ✅ Protected endpoints with authorization
- ✅ Robust validations with FluentValidation

### 🔄 [PASSWORD_RECOVERY.md](PASSWORD_RECOVERY.md)
**Password Recovery System**
- ✅ Reset request via email
- ✅ Secure 6-digit codes with expiration
- ✅ Professional HTML emails
- ✅ Robust code and password validations
- ✅ Email confirmation of successful change

### 🛡️ [PERMISSIONS_AND_ROLES.md](PERMISSIONS_AND_ROLES.md)
**Permissions and Roles System**
- ✅ Complete permission management (CRUD)
- ✅ Role management with permission assignment
- ✅ Granular authorization per endpoint
- ✅ JWT tokens with included permissions
- ✅ Configured authorization policies
- ✅ Predefined roles (Admin, User) with permissions

### 🌍 [LOCALIZATION_AND_EMAIL.md](LOCALIZATION_AND_EMAIL.md)
**Localization and Email Service**
- ✅ Spanish and English support
- ✅ Localized messages in all responses
- ✅ Automatic language configuration
- ✅ Dynamic language change via header/query
- ✅ HTML templates with modern CSS
- ✅ Welcome, recovery and confirmation emails

### 🚨 [ERROR_HANDLING.md](ERROR_HANDLING.md)
**Error Handling System**
- ✅ Specific exceptions with unique codes
- ✅ Automatic error message localization
- ✅ Centralized middleware for exception handling
- ✅ Appropriate HTTP codes automatically mapped
- ✅ Structured responses with consistent metadata
- ✅ Structured logging for debugging

### 🔍 [ERROR_HANDLING_EXAMPLES.md](ERROR_HANDLING_EXAMPLES.md)
**Practical Error Handling Examples**
- ✅ API examples with requests and responses
- ✅ Code examples in different layers
- ✅ Common use cases step by step
- ✅ Error testing with unit tests and integration tests
- ✅ Testing configuration and mocking

### 🛠️ [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md)
**Scripts and Automation**
- ✅ npm scripts for common tasks
- ✅ Bash and PowerShell scripts
- ✅ Makefile with useful commands
- ✅ VS Code tasks configuration
- ✅ Docker and database automation

## 🚀 Quick Start

### 1. Initial Setup
```bash
# Clone and configure
git clone <repository>
cd CleanArchitecture

# Complete setup
npm run setup

# Start database
npm run docker:up

# Apply migrations
npm run db:migrate

# Start application
npm run dev
```

### 2. Default Admin User
```
Username: admin
Email: admin@example.com
Password: Admin123!
```

### 3. Main Endpoints
- **Swagger UI**: `https://localhost:7000/swagger`
- **API Base**: `https://localhost:7000/api/v1`
- **pgAdmin**: `http://localhost:5050` (admin@admin.com / admin)

## 🏗️ Project Architecture

```
CleanArchitecture/
├── docs/                    # 📚 Complete documentation
├── src/                     # 💻 Source code
│   ├── Domain/             # 🎯 Domain Layer
│   ├── Application/        # 🔄 Application Layer
│   ├── Infrastructure/     # 🔧 Infrastructure Layer
│   └── API/               # 🌐 Presentation Layer
├── scripts/               # 🤖 Automation scripts
└── .vscode/              # ⚙️ VS Code configuration
```

## 🎯 Main Features

### ✅ **Authentication and Authorization**
- JWT with refresh tokens
- Login with email/username
- Granular permission system
- Predefined roles (Admin, User)

### ✅ **Validation and Error Handling**
- FluentValidation for validations
- Controlled error system
- Message localization
- Consistent API responses

### ✅ **Database**
- PostgreSQL with Entity Framework Core
- Automatic migrations
- Initial data seeding
- Docker configuration

### ✅ **Development and Testing**
- Automation scripts
- VS Code configuration
- Swagger/OpenAPI
- Structured logging

## 🔧 Technologies Used

- **.NET 9.0** - Main framework
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Database
- **ASP.NET Core Identity** - Authentication
- **FluentValidation** - Validations
- **MediatR** - CQRS Pattern
- **Swagger/OpenAPI** - API documentation

## 📖 Usage Guides

### For Developers
1. Read [AUTHENTICATION.md](AUTHENTICATION.md) to understand the auth system
2. Review [ERROR_HANDLING.md](ERROR_HANDLING.md) for error handling
3. Consult [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md) for development

### For DevOps
1. Use [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md) for deployment
2. Configure Docker with `docker-compose.yml`
3. Apply migrations with automated scripts

### For Testing
1. Review [ERROR_HANDLING_EXAMPLES.md](ERROR_HANDLING_EXAMPLES.md) for examples
2. Use testing scripts in `scripts/`
3. Configure VS Code tasks for debugging

## 🆘 Support and Contribution

### Reporting Issues
- Use GitHub Issues templates
- Include logs and reproduction steps
- Specify version and environment

### Contributing
1. Fork the project
2. Create a branch for your feature
3. Follow code conventions
4. Add tests if necessary
5. Create a Pull Request

### Contact
- **Email**: [your-email@example.com]
- **Issues**: [GitHub Issues](https://github.com/your-username/your-repo/issues)

## 📝 Changelog

### v1.0.0 (January 13, 2025)
- ✅ Complete authentication system
- ✅ Password recovery
- ✅ Permissions and roles system
- ✅ Localization (es/en)
- ✅ Robust error handling
- ✅ Automation scripts
- ✅ Complete documentation

## 📄 License

This project is under the MIT License. See [LICENSE](LICENSE) for more details.

---

**Last updated:** January 13, 2025  
**Version:** 1.0.0  
**Maintained by:** [Your Name]