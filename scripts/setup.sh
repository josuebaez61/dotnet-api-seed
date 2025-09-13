#!/bin/bash

# Clean Architecture API Setup Script
# This script sets up the development environment

set -e

echo "🚀 Setting up Clean Architecture API..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker and try again."
    exit 1
fi

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    print_error ".NET is not installed. Please install .NET 9.0 and try again."
    exit 1
fi

print_status "Restoring NuGet packages..."
dotnet restore

print_status "Building the solution..."
dotnet build

print_status "Starting Docker containers..."
docker compose up -d

print_status "Waiting for PostgreSQL to be ready..."
sleep 15

# Check if PostgreSQL is ready
max_attempts=30
attempt=1
while [ $attempt -le $max_attempts ]; do
    if docker exec cleanarch-postgres pg_isready -U postgres > /dev/null 2>&1; then
        print_success "PostgreSQL is ready!"
        break
    fi
    
    if [ $attempt -eq $max_attempts ]; then
        print_error "PostgreSQL failed to start after $max_attempts attempts"
        exit 1
    fi
    
    print_status "Waiting for PostgreSQL... (attempt $attempt/$max_attempts)"
    sleep 2
    attempt=$((attempt + 1))
done

print_status "Running database migrations..."
dotnet ef database update --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API

print_success "Setup completed successfully!"
print_status "You can now run:"
echo "  • npm run dev          - Start the API"
echo "  • npm run swagger      - Open Swagger UI"
echo "  • npm run pgadmin      - Open pgAdmin"
echo "  • npm run logs         - View container logs"
