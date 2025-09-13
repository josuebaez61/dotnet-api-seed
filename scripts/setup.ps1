# Clean Architecture API Setup Script (PowerShell)
# This script sets up the development environment on Windows

param(
    [switch]$Force
)

# Function to print colored output
function Write-Status {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

Write-Host "🚀 Setting up Clean Architecture API..." -ForegroundColor Cyan

# Check if Docker is running
try {
    docker info | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Docker is not running"
    }
} catch {
    Write-Error "Docker is not running. Please start Docker Desktop and try again."
    exit 1
}

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Status "Found .NET version: $dotnetVersion"
} catch {
    Write-Error ".NET is not installed. Please install .NET 9.0 and try again."
    exit 1
}

Write-Status "Restoring NuGet packages..."
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to restore packages"
    exit 1
}

Write-Status "Building the solution..."
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Status "Starting Docker containers..."
docker compose up -d
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to start Docker containers"
    exit 1
}

Write-Status "Waiting for PostgreSQL to be ready..."
Start-Sleep -Seconds 15

# Check if PostgreSQL is ready
$maxAttempts = 30
$attempt = 1
do {
    try {
        docker exec cleanarch-postgres pg_isready -U postgres | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Success "PostgreSQL is ready!"
            break
        }
    } catch {
        # Continue waiting
    }
    
    if ($attempt -eq $maxAttempts) {
        Write-Error "PostgreSQL failed to start after $maxAttempts attempts"
        exit 1
    }
    
    Write-Status "Waiting for PostgreSQL... (attempt $attempt/$maxAttempts)"
    Start-Sleep -Seconds 2
    $attempt++
} while ($attempt -le $maxAttempts)

Write-Status "Running database migrations..."
dotnet ef database update --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API
if ($LASTEXITCODE -ne 0) {
    Write-Error "Migration failed"
    exit 1
}

Write-Success "Setup completed successfully!"
Write-Status "You can now run:"
Write-Host "  • npm run dev          - Start the API" -ForegroundColor White
Write-Host "  • npm run swagger      - Open Swagger UI" -ForegroundColor White
Write-Host "  • npm run pgadmin      - Open pgAdmin" -ForegroundColor White
Write-Host "  • npm run logs         - View container logs" -ForegroundColor White
