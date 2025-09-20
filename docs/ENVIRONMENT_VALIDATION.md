# Environment Validation & Configuration

This document explains the environment validation system and environment variables configuration implemented in the Clean Architecture API.

## Overview

The application includes comprehensive environment validation to ensure only approved environments are used. This prevents configuration errors and ensures consistent behavior across different deployment scenarios.

## Allowed Environments

The following environments are currently allowed:

- **Development** - Local development environment
- **Staging** - Pre-production testing environment  
- **Production** - Live production environment

## Environment Variables

### Core Environment Variables

#### `ASPNETCORE_ENVIRONMENT`
- **Purpose**: Defines the current application environment
- **Required**: Yes
- **Allowed Values**: `Development`, `Staging`, `Production`
- **Default**: `Development` (when not set)
- **Usage**: Controls which configuration files are loaded and application behavior

```bash
# Examples
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_ENVIRONMENT=Staging
export ASPNETCORE_ENVIRONMENT=Production
```

#### `ASPNETCORE_URLS`
- **Purpose**: Specifies the URLs that the server should listen on
- **Required**: No (has defaults)
- **Default**: `http://localhost:5000` and `https://localhost:5001`
- **Usage**: Customize server binding addresses

```bash
# Examples
export ASPNETCORE_URLS="http://localhost:8080;https://localhost:8443"
export ASPNETCORE_URLS="http://0.0.0.0:80"
```

#### `ASPNETCORE_Kestrel__Certificates__Default__Password`
- **Purpose**: Password for SSL certificate
- **Required**: No
- **Usage**: For HTTPS development with custom certificates

```bash
export ASPNETCORE_Kestrel__Certificates__Default__Password="your-cert-password"
```

### Database Environment Variables

#### `ConnectionStrings__DefaultConnection`
- **Purpose**: PostgreSQL database connection string
- **Required**: Yes
- **Format**: `Host=hostname;Database=dbname;Username=user;Password=pass`
- **Usage**: Override database connection settings

```bash
# Examples
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB;Username=postgres;Password=postgres"
export ConnectionStrings__DefaultConnection="Host=prod-db.example.com;Database=CleanArchitectureDB_Prod;Username=prod_user;Password=secure_password"
```

### JWT Configuration Variables

#### `JwtSettings__SecretKey`
- **Purpose**: Secret key for JWT token signing
- **Required**: Yes
- **Usage**: Override default JWT secret key

```bash
export JwtSettings__SecretKey="YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
```

#### `JwtSettings__Issuer`
- **Purpose**: JWT token issuer
- **Required**: No
- **Default**: `CleanArchitecture`
- **Usage**: Customize JWT issuer

```bash
export JwtSettings__Issuer="YourCompanyName"
```

#### `JwtSettings__Audience`
- **Purpose**: JWT token audience
- **Required**: No
- **Default**: `CleanArchitectureUsers`
- **Usage**: Customize JWT audience

```bash
export JwtSettings__Audience="YourAppUsers"
```

### Email Configuration Variables

#### `EmailSettings__SmtpHost`
- **Purpose**: SMTP server hostname
- **Required**: Yes
- **Usage**: Configure email server

```bash
export EmailSettings__SmtpHost="smtp.gmail.com"
```

#### `EmailSettings__SmtpPort`
- **Purpose**: SMTP server port
- **Required**: No
- **Default**: `587`
- **Usage**: Configure SMTP port

```bash
export EmailSettings__SmtpPort="465"
```

#### `EmailSettings__SmtpUsername`
- **Purpose**: SMTP authentication username
- **Required**: Yes
- **Usage**: SMTP login credentials

```bash
export EmailSettings__SmtpUsername="your-email@gmail.com"
```

#### `EmailSettings__SmtpPassword`
- **Purpose**: SMTP authentication password
- **Required**: Yes
- **Usage**: SMTP login credentials

```bash
export EmailSettings__SmtpPassword="your-app-password"
```

#### `EmailSettings__FromEmail`
- **Purpose**: Default sender email address
- **Required**: Yes
- **Usage**: Set default from email

```bash
export EmailSettings__FromEmail="noreply@yourdomain.com"
```

#### `EmailSettings__FromName`
- **Purpose**: Default sender name
- **Required**: No
- **Default**: `Clean Architecture`
- **Usage**: Set default from name

```bash
export EmailSettings__FromName="Your Company Name"
```

### Frontend Configuration Variables

#### `FrontendSettings__BaseUrl`
- **Purpose**: Base URL for frontend application
- **Required**: No
- **Default**: `http://localhost:4200`
- **Usage**: Configure frontend URL for CORS and redirects

```bash
export FrontendSettings__BaseUrl="https://yourdomain.com"
```

### CORS Configuration Variables

#### `AllowedOrigins__0`, `AllowedOrigins__1`, etc.
- **Purpose**: Configure allowed origins for CORS
- **Required**: No (has defaults for Development)
- **Usage**: Specify allowed frontend domains

```bash
export AllowedOrigins__0="https://yourdomain.com"
export AllowedOrigins__1="https://admin.yourdomain.com"
export AllowedOrigins__2="https://staging.yourdomain.com"
```

### Logging Configuration Variables

#### `Logging__LogLevel__Default`
- **Purpose**: Set default log level
- **Required**: No
- **Default**: `Information`
- **Allowed Values**: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`
- **Usage**: Control logging verbosity

```bash
export Logging__LogLevel__Default="Debug"
```

#### `Logging__LogLevel__Microsoft.AspNetCore`
- **Purpose**: Set log level for ASP.NET Core
- **Required**: No
- **Default**: `Warning`
- **Usage**: Control ASP.NET Core logging

```bash
export Logging__LogLevel__Microsoft.AspNetCore="Information"
```

## Environment-Specific Configuration

### Development Environment
```bash
# Development settings
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="http://localhost:5103;https://localhost:7267"
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres"
export Logging__LogLevel__Default="Debug"
export FrontendSettings__BaseUrl="http://localhost:4200"
```

### Staging Environment
```bash
# Staging settings
export ASPNETCORE_ENVIRONMENT=Staging
export ASPNETCORE_URLS="http://0.0.0.0:80;https://0.0.0.0:443"
export ConnectionStrings__DefaultConnection="Host=staging-db.example.com;Database=CleanArchitectureDB_Staging;Username=staging_user;Password=staging_password"
export Logging__LogLevel__Default="Information"
export FrontendSettings__BaseUrl="https://staging.yourdomain.com"
export AllowedOrigins__0="https://staging.yourdomain.com"
export AllowedOrigins__1="https://staging-admin.yourdomain.com"
```

### Production Environment
```bash
# Production settings
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:80;https://0.0.0.0:443"
export ConnectionStrings__DefaultConnection="Host=prod-db.example.com;Database=CleanArchitectureDB_Prod;Username=prod_user;Password=secure_production_password"
export Logging__LogLevel__Default="Warning"
export Logging__LogLevel__Microsoft.AspNetCore="Error"
export FrontendSettings__BaseUrl="https://yourdomain.com"
export AllowedOrigins__0="https://yourdomain.com"
export AllowedOrigins__1="https://admin.yourdomain.com"
export JwtSettings__SecretKey="YourProductionSecretKeyThatIsAtLeast32CharactersLong!"
export EmailSettings__SmtpHost="smtp.yourdomain.com"
export EmailSettings__SmtpUsername="noreply@yourdomain.com"
export EmailSettings__SmtpPassword="your_production_email_password"
```

## Validation Points

### 1. Application Startup

Environment validation occurs at application startup in `Program.cs`:

```csharp
// Validate environment before proceeding
try
{
    EnvironmentConstants.ValidateEnvironment(builder.Environment.EnvironmentName);
    Console.WriteLine($"✅ Environment validation passed: {builder.Environment.EnvironmentName}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine($"🔧 Current environment: '{builder.Environment.EnvironmentName}'");
    Console.WriteLine($"💡 Please set ASPNETCORE_ENVIRONMENT to one of the allowed values.");
    Environment.Exit(1);
}
```

### 2. CLI Commands

Seeder commands validate the environment before execution:

```bash
# These commands will validate the environment
npm run db:seed
npm run db:seed:staging
npm run db:seed:prod
npm run db:truncate
```

### 3. Runtime Validation (Optional)

A middleware is available for runtime validation (currently commented out):

```csharp
// Add environment validation middleware (optional, for runtime validation)
app.UseEnvironmentValidation(); // Uncomment if you want runtime validation
```

## Error Handling

When an invalid environment is detected:

1. **Startup**: Application exits with error code 1
2. **CLI Commands**: Command fails with detailed error message
3. **Runtime**: Returns HTTP 500 with JSON error response (if middleware enabled)

## Setting Environment Variables

### Linux/macOS
```bash
# Temporary (current session only)
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Database=MyDB;Username=user;Password=pass"

# Permanent (add to ~/.bashrc, ~/.zshrc, or ~/.profile)
echo 'export ASPNETCORE_ENVIRONMENT=Development' >> ~/.bashrc
echo 'export ConnectionStrings__DefaultConnection="Host=localhost;Database=MyDB;Username=user;Password=pass"' >> ~/.bashrc
```

### Windows (Command Prompt)
```cmd
REM Temporary (current session only)
set ASPNETCORE_ENVIRONMENT=Development
set ConnectionStrings__DefaultConnection=Host=localhost;Database=MyDB;Username=user;Password=pass

REM Permanent (setx command)
setx ASPNETCORE_ENVIRONMENT "Development"
setx ConnectionStrings__DefaultConnection "Host=localhost;Database=MyDB;Username=user;Password=pass"
```

### Windows (PowerShell)
```powershell
# Temporary (current session only)
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=MyDB;Username=user;Password=pass"

# Permanent (setx command)
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "User")
[Environment]::SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Host=localhost;Database=MyDB;Username=user;Password=pass", "User")
```

### Docker
```dockerfile
# Dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Host=db;Database=CleanArchitectureDB;Username=postgres;Password=postgres"
```

```yaml
# docker-compose.yml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres
```

### .env Files (for local development)
Create a `.env` file in the project root:
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres
JwtSettings__SecretKey=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
EmailSettings__SmtpHost=sandbox.smtp.mailtrap.io
EmailSettings__SmtpUsername=your_username
EmailSettings__SmtpPassword=your_password
FrontendSettings__BaseUrl=http://localhost:4200
```

## NPM Scripts

The following NPM scripts automatically set the correct environment:

```bash
# Development (default)
npm run db:seed
npm run db:migrate:dev
npm run setup:dev

# Staging
npm run db:seed:staging
npm run db:migrate:staging
npm run setup:staging

# Production
npm run db:seed:prod
npm run db:migrate:prod
npm run setup:prod
```

## Launch Profiles

VS Code launch profiles are configured for each environment:

- **Launch API (Development)** - `ASPNETCORE_ENVIRONMENT=Development`
- **Launch API (Staging)** - `ASPNETCORE_ENVIRONMENT=Staging`
- **Launch API (Production)** - `ASPNETCORE_ENVIRONMENT=Production`

## Configuration Files

Each environment has its own configuration file:

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development-specific settings
- `appsettings.Staging.json` - Staging-specific settings
- `appsettings.Production.json` - Production-specific settings

## Environment Variable Priority

Configuration values are loaded in the following order (later values override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Environment variables (highest priority)

## Security Best Practices

### Environment Variables Security
- **Never commit sensitive data** to version control
- **Use secrets management** for production (Azure Key Vault, AWS Secrets Manager, etc.)
- **Rotate secrets regularly** (JWT keys, database passwords, etc.)
- **Use different secrets** for each environment
- **Validate environment variables** at startup

### Example Secure Configuration
```bash
# Production - Use secrets management
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Host=${DB_HOST};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
export JwtSettings__SecretKey="${JWT_SECRET_KEY}"
export EmailSettings__SmtpPassword="${EMAIL_PASSWORD}"
```

## Adding New Environments

To add a new environment:

1. Add the environment name to `EnvironmentConstants.AllowedEnvironments`
2. Create a new `appsettings.{Environment}.json` file
3. Add launch profiles to `launchSettings.json`
4. Add NPM scripts to `package.json`
5. Update VS Code launch configurations
6. Document environment-specific variables

## Troubleshooting

### Common Issues

1. **"Invalid environment" error**
   - Check that `ASPNETCORE_ENVIRONMENT` is set correctly
   - Ensure the environment name matches exactly (case-sensitive)

2. **Environment not being detected**
   - Restart the application after changing environment variables
   - Check that the environment variable is set in the correct scope

3. **Configuration not loading**
   - Ensure the corresponding `appsettings.{Environment}.json` file exists
   - Check file permissions and syntax

4. **Environment variables not overriding config**
   - Check the variable name format (use double underscores for nested properties)
   - Ensure the variable is set in the correct scope

5. **Database connection issues**
   - Verify connection string format
   - Check database server accessibility
   - Validate credentials

### Debugging

Enable detailed logging to see environment validation:

```bash
# Check current environment
echo $ASPNETCORE_ENVIRONMENT

# List all environment variables
env | grep ASPNETCORE

# Run with verbose logging
dotnet run --verbosity detailed

# Check configuration values
dotnet run --launch-profile cli --seed list
```

### Configuration Debugging

Add this to your `Program.cs` for debugging configuration:

```csharp
// Debug configuration (remove in production)
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("🔍 Configuration Debug:");
    Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
    Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("DefaultConnection")}");
    Console.WriteLine($"JWT Issuer: {builder.Configuration["JwtSettings:Issuer"]}");
}
```

## Security Considerations

- Environment validation prevents accidental use of wrong configurations
- Production environment requires explicit confirmation for destructive operations
- CLI commands validate environment before executing database operations
- Runtime validation (if enabled) provides additional safety for web requests
- Sensitive data should never be stored in configuration files
- Use environment variables or secrets management for sensitive data

## Best Practices

1. **Always set environment variables explicitly** - Don't rely on defaults
2. **Use NPM scripts** - They handle environment setup automatically
3. **Test in Staging first** - Validate changes before production deployment
4. **Monitor environment validation logs** - Check for configuration issues
5. **Use launch profiles** - Consistent environment setup across team members
6. **Document environment variables** - Keep this documentation updated
7. **Use secrets management** - Never hardcode sensitive data
8. **Validate configuration** - Check that all required variables are set
9. **Use different secrets per environment** - Never reuse production secrets
10. **Rotate secrets regularly** - Implement a secret rotation policy

## Environment Variable Reference

| Variable | Purpose | Required | Default | Example |
|----------|---------|----------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | Application environment | Yes | `Development` | `Production` |
| `ASPNETCORE_URLS` | Server URLs | No | `http://localhost:5000;https://localhost:5001` | `http://0.0.0.0:80` |
| `ConnectionStrings__DefaultConnection` | Database connection | Yes | - | `Host=localhost;Database=MyDB;Username=user;Password=pass` |
| `JwtSettings__SecretKey` | JWT signing key | Yes | - | `YourSecretKey32CharsLong!` |
| `JwtSettings__Issuer` | JWT issuer | No | `CleanArchitecture` | `YourCompany` |
| `JwtSettings__Audience` | JWT audience | No | `CleanArchitectureUsers` | `YourAppUsers` |
| `EmailSettings__SmtpHost` | SMTP server | Yes | - | `smtp.gmail.com` |
| `EmailSettings__SmtpPort` | SMTP port | No | `587` | `465` |
| `EmailSettings__SmtpUsername` | SMTP username | Yes | - | `your-email@gmail.com` |
| `EmailSettings__SmtpPassword` | SMTP password | Yes | - | `your-app-password` |
| `EmailSettings__FromEmail` | Default from email | Yes | - | `noreply@yourdomain.com` |
| `EmailSettings__FromName` | Default from name | No | `Clean Architecture` | `Your Company` |
| `FrontendSettings__BaseUrl` | Frontend URL | No | `http://localhost:4200` | `https://yourdomain.com` |
| `AllowedOrigins__0` | CORS origin 1 | No | - | `https://yourdomain.com` |
| `AllowedOrigins__1` | CORS origin 2 | No | - | `https://admin.yourdomain.com` |
| `Logging__LogLevel__Default` | Default log level | No | `Information` | `Debug` |
| `Logging__LogLevel__Microsoft.AspNetCore` | ASP.NET log level | No | `Warning` | `Information` |