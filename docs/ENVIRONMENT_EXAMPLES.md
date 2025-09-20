# Environment Configuration Examples

This document provides practical examples of environment configurations for different scenarios.

## Quick Start Examples

### Development Setup
```bash
# Basic development setup
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres"

# Run the application
npm run setup:dev
npm run db:seed
```

### Staging Setup
```bash
# Staging environment setup
export ASPNETCORE_ENVIRONMENT=Staging
export ConnectionStrings__DefaultConnection="Host=staging-db.example.com;Database=CleanArchitectureDB_Staging;Username=staging_user;Password=staging_password"
export FrontendSettings__BaseUrl="https://staging.yourdomain.com"

# Run the application
npm run setup:staging
npm run db:seed:staging
```

### Production Setup
```bash
# Production environment setup
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Host=prod-db.example.com;Database=CleanArchitectureDB_Prod;Username=prod_user;Password=secure_production_password"
export FrontendSettings__BaseUrl="https://yourdomain.com"

# Run the application
npm run setup:prod
npm run db:seed:prod
```

## Complete Environment Configurations

### Development Environment
```bash
#!/bin/bash
# Development environment configuration

# Core settings
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="http://localhost:5103;https://localhost:7267"

# Database
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres"

# JWT Configuration
export JwtSettings__SecretKey="DevelopmentSecretKeyThatIsAtLeast32CharactersLong!"
export JwtSettings__Issuer="CleanArchitecture-Dev"
export JwtSettings__Audience="CleanArchitectureUsers-Dev"

# Email Configuration (using Mailtrap for testing)
export EmailSettings__SmtpHost="sandbox.smtp.mailtrap.io"
export EmailSettings__SmtpPort="587"
export EmailSettings__SmtpUsername="your_mailtrap_username"
export EmailSettings__SmtpPassword="your_mailtrap_password"
export EmailSettings__FromEmail="noreply-dev@cleanarchitecture.com"
export EmailSettings__FromName="Clean Architecture (Dev)"

# Frontend
export FrontendSettings__BaseUrl="http://localhost:4200"

# Logging
export Logging__LogLevel__Default="Debug"
export Logging__LogLevel__Microsoft.AspNetCore="Information"

echo "✅ Development environment configured"
```

### Staging Environment
```bash
#!/bin/bash
# Staging environment configuration

# Core settings
export ASPNETCORE_ENVIRONMENT=Staging
export ASPNETCORE_URLS="http://0.0.0.0:80;https://0.0.0.0:443"

# Database
export ConnectionStrings__DefaultConnection="Host=staging-db.example.com;Database=CleanArchitectureDB_Staging;Username=staging_user;Password=staging_password"

# JWT Configuration
export JwtSettings__SecretKey="StagingSecretKeyThatIsAtLeast32CharactersLong!"
export JwtSettings__Issuer="CleanArchitecture-Staging"
export JwtSettings__Audience="CleanArchitectureUsers-Staging"

# Email Configuration
export EmailSettings__SmtpHost="smtp.staging.example.com"
export EmailSettings__SmtpPort="587"
export EmailSettings__SmtpUsername="staging_smtp_user"
export EmailSettings__SmtpPassword="staging_smtp_password"
export EmailSettings__FromEmail="noreply-staging@yourdomain.com"
export EmailSettings__FromName="Your Company (Staging)"

# Frontend
export FrontendSettings__BaseUrl="https://staging.yourdomain.com"

# CORS
export AllowedOrigins__0="https://staging.yourdomain.com"
export AllowedOrigins__1="https://staging-admin.yourdomain.com"

# Logging
export Logging__LogLevel__Default="Information"
export Logging__LogLevel__Microsoft.AspNetCore="Warning"

echo "✅ Staging environment configured"
```

### Production Environment
```bash
#!/bin/bash
# Production environment configuration

# Core settings
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:80;https://0.0.0.0:443"

# Database (use secrets management in real scenarios)
export ConnectionStrings__DefaultConnection="Host=prod-db.example.com;Database=CleanArchitectureDB_Prod;Username=prod_user;Password=secure_production_password"

# JWT Configuration
export JwtSettings__SecretKey="ProductionSecretKeyThatIsAtLeast32CharactersLong!"
export JwtSettings__Issuer="CleanArchitecture-Production"
export JwtSettings__Audience="CleanArchitectureUsers-Production"

# Email Configuration
export EmailSettings__SmtpHost="smtp.yourdomain.com"
export EmailSettings__SmtpPort="587"
export EmailSettings__SmtpUsername="noreply@yourdomain.com"
export EmailSettings__SmtpPassword="production_email_password"
export EmailSettings__FromEmail="noreply@yourdomain.com"
export EmailSettings__FromName="Your Company"

# Frontend
export FrontendSettings__BaseUrl="https://yourdomain.com"

# CORS
export AllowedOrigins__0="https://yourdomain.com"
export AllowedOrigins__1="https://admin.yourdomain.com"

# Logging
export Logging__LogLevel__Default="Warning"
export Logging__LogLevel__Microsoft.AspNetCore="Error"

echo "✅ Production environment configured"
```

## Docker Examples

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CleanArchitecture.API/CleanArchitecture.API.csproj", "src/CleanArchitecture.API/"]
COPY ["src/CleanArchitecture.Application/CleanArchitecture.Application.csproj", "src/CleanArchitecture.Application/"]
COPY ["src/CleanArchitecture.Infrastructure/CleanArchitecture.Infrastructure.csproj", "src/CleanArchitecture.Infrastructure/"]
COPY ["src/CleanArchitecture.Domain/CleanArchitecture.Domain.csproj", "src/CleanArchitecture.Domain/"]

RUN dotnet restore "src/CleanArchitecture.API/CleanArchitecture.API.csproj"
COPY . .
WORKDIR "/src/src/CleanArchitecture.API"
RUN dotnet build "CleanArchitecture.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CleanArchitecture.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set default environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://+:80;https://+:443"

ENTRYPOINT ["dotnet", "CleanArchitecture.API.dll"]
```

### docker-compose.yml
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: CleanArchitectureDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: .
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=CleanArchitectureDB;Username=postgres;Password=postgres
      - JwtSettings__SecretKey=DevelopmentSecretKeyThatIsAtLeast32CharactersLong!
      - EmailSettings__SmtpHost=sandbox.smtp.mailtrap.io
      - EmailSettings__SmtpPort=587
      - EmailSettings__SmtpUsername=your_mailtrap_username
      - EmailSettings__SmtpPassword=your_mailtrap_password
    ports:
      - "8080:80"
    depends_on:
      - postgres

volumes:
  postgres_data:
```

### docker-compose.prod.yml
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: CleanArchitectureDB_Prod
      POSTGRES_USER: ${DB_USER}
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - app-network

  api:
    build: .
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=CleanArchitectureDB_Prod;Username=${DB_USER};Password=${DB_PASSWORD}
      - JwtSettings__SecretKey=${JWT_SECRET_KEY}
      - EmailSettings__SmtpHost=${SMTP_HOST}
      - EmailSettings__SmtpUsername=${SMTP_USERNAME}
      - EmailSettings__SmtpPassword=${SMTP_PASSWORD}
      - FrontendSettings__BaseUrl=${FRONTEND_URL}
    ports:
      - "80:80"
    depends_on:
      - postgres
    networks:
      - app-network

networks:
  app-network:
    driver: bridge

volumes:
  postgres_data:
```

## Kubernetes Examples

### ConfigMap
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: cleanarchitecture-config
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  ASPNETCORE_URLS: "http://+:80"
  FrontendSettings__BaseUrl: "https://yourdomain.com"
  Logging__LogLevel__Default: "Warning"
  Logging__LogLevel__Microsoft.AspNetCore: "Error"
```

### Secret
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: cleanarchitecture-secrets
type: Opaque
data:
  ConnectionStrings__DefaultConnection: SG9zdD1wb3N0Z3JlcztEYXRhYmFzZT1DbGVhbkFyY2hpdGVjdHVyZURCX1Byb2Q7VXNlcm5hbWU9cHJvZF91c2VyO1Bhc3N3b3JkPXNlY3VyZV9wcm9kdWN0aW9uX3Bhc3N3b3Jk
  JwtSettings__SecretKey: UHJvZHVjdGlvblNlY3JldEtleVRoYXRJc0F0TGVhc3QzMkNoYXJhY3RlcnNMb25nIQ==
  EmailSettings__SmtpPassword: cHJvZHVjdGlvbl9lbWFpbF9wYXNzd29yZA==
```

### Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: cleanarchitecture-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: cleanarchitecture-api
  template:
    metadata:
      labels:
        app: cleanarchitecture-api
    spec:
      containers:
      - name: api
        image: yourregistry/cleanarchitecture-api:latest
        ports:
        - containerPort: 80
        envFrom:
        - configMapRef:
            name: cleanarchitecture-config
        - secretRef:
            name: cleanarchitecture-secrets
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
```

## CI/CD Examples

### GitHub Actions
```yaml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Build application
      run: dotnet build --configuration Release
    
    - name: Deploy to production
      run: |
        # Set production environment variables
        export ASPNETCORE_ENVIRONMENT=Production
        export ConnectionStrings__DefaultConnection="${{ secrets.PROD_DB_CONNECTION }}"
        export JwtSettings__SecretKey="${{ secrets.JWT_SECRET_KEY }}"
        export EmailSettings__SmtpPassword="${{ secrets.EMAIL_PASSWORD }}"
        
        # Deploy application
        ./deploy.sh
      env:
        DEPLOYMENT_TOKEN: ${{ secrets.DEPLOYMENT_TOKEN }}
```

### Azure DevOps Pipeline
```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'

stages:
- stage: Build
  jobs:
  - job: BuildJob
    steps:
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build application'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration)'

- stage: Deploy
  dependsOn: Build
  jobs:
  - deployment: DeployJob
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            inputs:
              azureSubscription: 'Azure Service Connection'
              appName: 'cleanarchitecture-api'
              package: '$(Pipeline.Workspace)/drop'
            env:
              ASPNETCORE_ENVIRONMENT: 'Production'
              ConnectionStrings__DefaultConnection: '$(PROD_DB_CONNECTION)'
              JwtSettings__SecretKey: '$(JWT_SECRET_KEY)'
              EmailSettings__SmtpPassword: '$(EMAIL_PASSWORD)'
```

## Local Development Scripts

### setup-dev.sh
```bash
#!/bin/bash
# Development setup script

echo "🚀 Setting up development environment..."

# Set environment variables
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Database=CleanArchitectureDB_Dev;Username=postgres;Password=postgres"
export JwtSettings__SecretKey="DevelopmentSecretKeyThatIsAtLeast32CharactersLong!"

# Start Docker services
echo "🐳 Starting Docker services..."
docker-compose up -d postgres

# Wait for database
echo "⏳ Waiting for database..."
sleep 10

# Run migrations
echo "📊 Running database migrations..."
npm run db:migrate:dev

# Seed database
echo "🌱 Seeding database..."
npm run db:seed

echo "✅ Development environment setup complete!"
echo "🌐 API: https://localhost:7267"
echo "📚 Swagger: https://localhost:7267"
echo "🗄️ Database: localhost:5432"
```

### setup-staging.sh
```bash
#!/bin/bash
# Staging setup script

echo "🚀 Setting up staging environment..."

# Set environment variables
export ASPNETCORE_ENVIRONMENT=Staging
export ConnectionStrings__DefaultConnection="Host=staging-db.example.com;Database=CleanArchitectureDB_Staging;Username=staging_user;Password=staging_password"
export JwtSettings__SecretKey="StagingSecretKeyThatIsAtLeast32CharactersLong!"
export FrontendSettings__BaseUrl="https://staging.yourdomain.com"

# Run migrations
echo "📊 Running database migrations..."
npm run db:migrate:staging

# Seed database
echo "🌱 Seeding database..."
npm run db:seed:staging

echo "✅ Staging environment setup complete!"
echo "🌐 API: https://staging-api.yourdomain.com"
```

## Environment Variable Validation Script

### validate-env.sh
```bash
#!/bin/bash
# Environment validation script

echo "🔍 Validating environment configuration..."

# Check required variables
required_vars=(
    "ASPNETCORE_ENVIRONMENT"
    "ConnectionStrings__DefaultConnection"
    "JwtSettings__SecretKey"
)

missing_vars=()

for var in "${required_vars[@]}"; do
    if [ -z "${!var}" ]; then
        missing_vars+=("$var")
    fi
done

if [ ${#missing_vars[@]} -ne 0 ]; then
    echo "❌ Missing required environment variables:"
    printf '   %s\n' "${missing_vars[@]}"
    exit 1
fi

# Validate environment value
case "$ASPNETCORE_ENVIRONMENT" in
    "Development"|"Staging"|"Production")
        echo "✅ Environment '$ASPNETCORE_ENVIRONMENT' is valid"
        ;;
    *)
        echo "❌ Invalid environment: '$ASPNETCORE_ENVIRONMENT'"
        echo "   Allowed values: Development, Staging, Production"
        exit 1
        ;;
esac

# Validate JWT secret key length
jwt_key_length=${#JwtSettings__SecretKey}
if [ $jwt_key_length -lt 32 ]; then
    echo "❌ JWT secret key must be at least 32 characters long"
    echo "   Current length: $jwt_key_length"
    exit 1
fi

echo "✅ All environment validations passed!"
```

## Troubleshooting Scripts

### debug-env.sh
```bash
#!/bin/bash
# Environment debugging script

echo "🔍 Environment Debug Information"
echo "================================"

echo "Current Environment: $ASPNETCORE_ENVIRONMENT"
echo "Current Directory: $(pwd)"
echo "User: $(whoami)"
echo ""

echo "Environment Variables:"
echo "---------------------"
env | grep -E "(ASPNETCORE|ConnectionStrings|JwtSettings|EmailSettings|FrontendSettings|Logging)" | sort
echo ""

echo "Configuration Files:"
echo "-------------------"
ls -la appsettings*.json 2>/dev/null || echo "No appsettings files found"
echo ""

echo "Launch Profiles:"
echo "---------------"
if [ -f "src/CleanArchitecture.API/Properties/launchSettings.json" ]; then
    cat src/CleanArchitecture.API/Properties/launchSettings.json | jq '.profiles | keys[]' 2>/dev/null || echo "Could not parse launchSettings.json"
else
    echo "launchSettings.json not found"
fi
```

## Usage Examples

### Running with specific environment
```bash
# Development
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/CleanArchitecture.API

# Staging
ASPNETCORE_ENVIRONMENT=Staging dotnet run --project src/CleanArchitecture.API

# Production
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/CleanArchitecture.API
```

### Running with custom configuration
```bash
# Custom database
ConnectionStrings__DefaultConnection="Host=custom-db;Database=MyDB;Username=user;Password=pass" \
ASPNETCORE_ENVIRONMENT=Development \
dotnet run --project src/CleanArchitecture.API

# Custom JWT settings
JwtSettings__SecretKey="MyCustomSecretKeyThatIsAtLeast32CharactersLong!" \
JwtSettings__Issuer="MyCompany" \
JwtSettings__Audience="MyAppUsers" \
ASPNETCORE_ENVIRONMENT=Development \
dotnet run --project src/CleanArchitecture.API
```

### Running CLI commands with environment
```bash
# List seeders in development
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/CleanArchitecture.API list

# Run seeders in staging
ASPNETCORE_ENVIRONMENT=Staging dotnet run --project src/CleanArchitecture.API --seed all

# Run seeders in production (requires confirmation)
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/CleanArchitecture.API --seed all
```
