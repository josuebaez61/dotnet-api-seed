#!/bin/bash

# Test Script
# Runs tests and code quality checks

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
    echo "Test and Quality Script"
    echo ""
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  test             - Run all tests"
    echo "  build            - Build the solution"
    echo "  format           - Format code"
    echo "  lint             - Check code formatting"
    echo "  clean            - Clean build artifacts"
    echo "  restore          - Restore NuGet packages"
    echo "  all              - Run all quality checks"
    echo ""
    echo "Examples:"
    echo "  $0 test"
    echo "  $0 all"
}

# Run tests
run_tests() {
    print_status "Running tests..."
    if dotnet test; then
        print_success "All tests passed!"
    else
        print_error "Some tests failed!"
        exit 1
    fi
}

# Build solution
build_solution() {
    print_status "Building solution..."
    if dotnet build; then
        print_success "Build completed successfully!"
    else
        print_error "Build failed!"
        exit 1
    fi
}

# Format code
format_code() {
    print_status "Formatting code..."
    if dotnet format; then
        print_success "Code formatted successfully!"
    else
        print_warning "Code formatting completed with warnings"
    fi
}

# Check code formatting
lint_code() {
    print_status "Checking code formatting..."
    if dotnet format --verify-no-changes; then
        print_success "Code formatting is correct!"
    else
        print_error "Code formatting issues found!"
        exit 1
    fi
}

# Clean build artifacts
clean_solution() {
    print_status "Cleaning build artifacts..."
    dotnet clean
    print_success "Clean completed!"
}

# Restore packages
restore_packages() {
    print_status "Restoring NuGet packages..."
    dotnet restore
    print_success "Packages restored successfully!"
}

# Run all quality checks
run_all() {
    print_status "Running all quality checks..."
    echo ""
    
    print_status "1. Restoring packages..."
    restore_packages
    echo ""
    
    print_status "2. Building solution..."
    build_solution
    echo ""
    
    print_status "3. Checking code formatting..."
    lint_code
    echo ""
    
    print_status "4. Running tests..."
    run_tests
    echo ""
    
    print_success "All quality checks completed successfully!"
}

# Main script logic
case "$1" in
    "test")
        run_tests
        ;;
    "build")
        build_solution
        ;;
    "format")
        format_code
        ;;
    "lint")
        lint_code
        ;;
    "clean")
        clean_solution
        ;;
    "restore")
        restore_packages
        ;;
    "all")
        run_all
        ;;
    *)
        show_usage
        ;;
esac
