#!/bin/bash

# Development Script
# Starts the development environment

set -e

echo "🔧 Starting development environment..."

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

# Check if Docker containers are running
if ! docker compose ps | grep -q "Up"; then
    print_status "Starting Docker containers..."
    docker compose up -d
    sleep 10
fi

# Check if PostgreSQL is ready
if ! docker exec cleanarch-postgres pg_isready -U postgres > /dev/null 2>&1; then
    print_status "Waiting for PostgreSQL to be ready..."
    sleep 5
fi

print_status "Starting the API..."
print_success "Development environment is ready!"
echo ""
echo "🌐 Available endpoints:"
echo "  • API: https://localhost:7000"
echo "  • Swagger: https://localhost:7000/swagger"
echo "  • pgAdmin: http://localhost:5050"
echo ""
echo "📝 Useful commands:"
echo "  • Ctrl+C to stop the API"
echo "  • npm run logs to view container logs"
echo "  • npm run reset to reset the database"
echo ""

dotnet run --project src/CleanArchitecture.API
