# Clean Architecture API Documentation

This directory contains comprehensive documentation for the Clean Architecture API project.

## 📚 Documentation Files

### [Environment Validation & Configuration](./ENVIRONMENT_VALIDATION.md)
Complete guide to environment validation system and environment variables configuration.

**Topics covered:**
- Environment validation system
- Allowed environments (Development, Staging, Production)
- Environment variables reference
- Configuration files
- Security best practices
- Troubleshooting guide

### [Environment Configuration Examples](./ENVIRONMENT_EXAMPLES.md)
Practical examples and scripts for different deployment scenarios.

**Topics covered:**
- Quick start examples
- Complete environment configurations
- Docker and Kubernetes examples
- CI/CD pipeline examples
- Local development scripts
- Troubleshooting scripts

## 🚀 Quick Start

### Development Setup
```bash
# Set environment variables
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres"

# Setup and run
npm run setup:dev
npm run db:seed
```

### Production Setup
```bash
# Set environment variables
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Host=prod-db.example.com;Database=CleanArchitectureDB_Prod;Username=prod_user;Password=secure_password"

# Setup and run
npm run setup:prod
npm run db:seed:prod
```

## 🔧 Environment Variables

### Required Variables
- `ASPNETCORE_ENVIRONMENT` - Application environment (Development/Staging/Production)
- `ConnectionStrings__DefaultConnection` - Database connection string
- `JwtSettings__SecretKey` - JWT signing key (minimum 32 characters)

### Optional Variables
- `ASPNETCORE_URLS` - Server URLs
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience
- `EmailSettings__*` - Email configuration
- `FrontendSettings__BaseUrl` - Frontend URL
- `AllowedOrigins__*` - CORS origins
- `Logging__LogLevel__*` - Logging levels

## 📋 NPM Scripts

### Development
```bash
npm run setup:dev      # Setup development environment
npm run db:migrate:dev # Run database migrations
npm run db:seed        # Seed development database
npm run db:truncate    # Truncate all tables (dev only)
```

### Staging
```bash
npm run setup:staging      # Setup staging environment
npm run db:migrate:staging # Run database migrations
npm run db:seed:staging    # Seed staging database
```

### Production
```bash
npm run setup:prod      # Setup production environment
npm run db:migrate:prod # Run database migrations
npm run db:seed:prod    # Seed production database (requires confirmation)
```

## 🛠️ Launch Profiles

VS Code launch profiles are configured for each environment:

- **Launch API (Development)** - `ASPNETCORE_ENVIRONMENT=Development`
- **Launch API (Staging)** - `ASPNETCORE_ENVIRONMENT=Staging`
- **Launch API (Production)** - `ASPNETCORE_ENVIRONMENT=Production`

## 🔍 Troubleshooting

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

### Debug Commands
```bash
# Check current environment
echo $ASPNETCORE_ENVIRONMENT

# List all environment variables
env | grep ASPNETCORE

# Run with verbose logging
dotnet run --verbosity detailed
```

## 🔒 Security Considerations

- Environment validation prevents accidental use of wrong configurations
- Production environment requires explicit confirmation for destructive operations
- CLI commands validate environment before executing database operations
- Sensitive data should never be stored in configuration files
- Use environment variables or secrets management for sensitive data

## 📖 Best Practices

1. **Always set environment variables explicitly** - Don't rely on defaults
2. **Use NPM scripts** - They handle environment setup automatically
3. **Test in Staging first** - Validate changes before production deployment
4. **Monitor environment validation logs** - Check for configuration issues
5. **Use launch profiles** - Consistent environment setup across team members
6. **Document environment variables** - Keep documentation updated
7. **Use secrets management** - Never hardcode sensitive data
8. **Validate configuration** - Check that all required variables are set
9. **Use different secrets per environment** - Never reuse production secrets
10. **Rotate secrets regularly** - Implement a secret rotation policy

## 📞 Support

For questions or issues related to environment configuration:

1. Check the troubleshooting section in the documentation
2. Review the examples in `ENVIRONMENT_EXAMPLES.md`
3. Verify your environment variables are set correctly
4. Check the application logs for validation errors

## 🔄 Updates

This documentation is maintained alongside the codebase. When adding new environment variables or changing configuration:

1. Update `ENVIRONMENT_VALIDATION.md` with new variables
2. Add examples to `ENVIRONMENT_EXAMPLES.md`
3. Update this README if needed
4. Test the changes in all environments