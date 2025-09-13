#!/bin/bash

# Database Management Script
# Handles database operations

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

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

# Function to show usage
show_usage() {
    echo "Database Management Script"
    echo ""
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  migrate          - Run database migrations"
    echo "  add-migration    - Add a new migration (requires name)"
    echo "  remove-migration - Remove the last migration"
    echo "  drop             - Drop the database (WARNING: This will delete all data)"
    echo "  reset            - Reset the database (drop and recreate)"
    echo "  seed             - Seed the database with initial data"
    echo "  status           - Show migration status"
    echo "  backup           - Create a database backup"
    echo "  restore          - Restore from backup (requires backup file)"
    echo ""
    echo "Examples:"
    echo "  $0 migrate"
    echo "  $0 add-migration AddUserTable"
    echo "  $0 reset"
}

# Check if Docker containers are running
check_containers() {
    if ! docker compose ps | grep -q "Up"; then
        print_status "Starting Docker containers..."
        docker compose up -d
        sleep 10
    fi
}

# Run migrations
migrate() {
    print_status "Running database migrations..."
    check_containers
    dotnet ef database update --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API
    print_success "Migrations completed successfully!"
}

# Add migration
add_migration() {
    if [ -z "$2" ]; then
        print_error "Migration name is required"
        echo "Usage: $0 add-migration <MigrationName>"
        exit 1
    fi
    
    print_status "Adding migration: $2"
    dotnet ef migrations add "$2" --project src/CleanArchitecture.Infrastructure
    print_success "Migration '$2' added successfully!"
}

# Remove migration
remove_migration() {
    print_warning "This will remove the last migration. Are you sure? (y/N)"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        print_status "Removing last migration..."
        dotnet ef migrations remove --project src/CleanArchitecture.Infrastructure
        print_success "Migration removed successfully!"
    else
        print_status "Operation cancelled."
    fi
}

# Drop database
drop_database() {
    print_warning "This will delete ALL data in the database. Are you sure? (y/N)"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        print_status "Dropping database..."
        dotnet ef database drop --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API --force
        print_success "Database dropped successfully!"
    else
        print_status "Operation cancelled."
    fi
}

# Reset database
reset_database() {
    print_warning "This will reset the database (drop and recreate). Are you sure? (y/N)"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        print_status "Resetting database..."
        drop_database
        migrate
        print_success "Database reset completed successfully!"
    else
        print_status "Operation cancelled."
    fi
}

# Show migration status
show_status() {
    print_status "Migration status:"
    dotnet ef migrations list --project src/CleanArchitecture.Infrastructure
}

# Create backup
backup_database() {
    print_status "Creating database backup..."
    check_containers
    
    timestamp=$(date +"%Y%m%d_%H%M%S")
    backup_file="backup_${timestamp}.sql"
    
    docker exec cleanarch-postgres pg_dump -U postgres -d CleanArchitectureDB > "$backup_file"
    print_success "Backup created: $backup_file"
}

# Restore from backup
restore_database() {
    if [ -z "$2" ]; then
        print_error "Backup file is required"
        echo "Usage: $0 restore <backup_file>"
        exit 1
    fi
    
    if [ ! -f "$2" ]; then
        print_error "Backup file not found: $2"
        exit 1
    fi
    
    print_warning "This will replace ALL data in the database. Are you sure? (y/N)"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        print_status "Restoring database from: $2"
        check_containers
        docker exec -i cleanarch-postgres psql -U postgres -d CleanArchitectureDB < "$2"
        print_success "Database restored successfully!"
    else
        print_status "Operation cancelled."
    fi
}

# Main script logic
case "$1" in
    "migrate")
        migrate
        ;;
    "add-migration")
        add_migration "$@"
        ;;
    "remove-migration")
        remove_migration
        ;;
    "drop")
        drop_database
        ;;
    "reset")
        reset_database
        ;;
    "status")
        show_status
        ;;
    "backup")
        backup_database
        ;;
    "restore")
        restore_database "$@"
        ;;
    *)
        show_usage
        ;;
esac
