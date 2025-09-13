# Clean Architecture API Makefile
# Alternative to npm scripts for Unix-like systems

.PHONY: help dev build clean restore test setup reset logs docker-up docker-down

# Default target
help: ## Show this help message
	@echo "Clean Architecture API - Available Commands:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

# Development
dev: ## Start development server
	@echo "🔧 Starting development environment..."
	@docker compose up -d
	@sleep 10
	@dotnet run --project src/CleanArchitecture.API

build: ## Build the solution
	@echo "🔨 Building solution..."
	@dotnet build

clean: ## Clean build artifacts
	@echo "🧹 Cleaning build artifacts..."
	@dotnet clean

restore: ## Restore NuGet packages
	@echo "📦 Restoring packages..."
	@dotnet restore

test: ## Run tests
	@echo "🧪 Running tests..."
	@dotnet test

# Database
migrate: ## Run database migrations
	@echo "🗄️ Running database migrations..."
	@dotnet ef database update --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API

migrate-add: ## Add a new migration (usage: make migrate-add NAME=migration_name)
	@echo "➕ Adding migration: $(NAME)"
	@dotnet ef migrations add $(NAME) --project src/CleanArchitecture.Infrastructure

migrate-remove: ## Remove the last migration
	@echo "➖ Removing last migration..."
	@dotnet ef migrations remove --project src/CleanArchitecture.Infrastructure

db-drop: ## Drop the database (WARNING: This will delete all data)
	@echo "⚠️ Dropping database..."
	@dotnet ef database drop --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API --force

db-reset: ## Reset the database (drop and recreate)
	@echo "🔄 Resetting database..."
	@make db-drop
	@make migrate

# Docker
docker-up: ## Start Docker containers
	@echo "🐳 Starting Docker containers..."
	@docker compose up -d

docker-down: ## Stop Docker containers
	@echo "🛑 Stopping Docker containers..."
	@docker compose down

docker-logs: ## View Docker container logs
	@echo "📋 Viewing container logs..."
	@docker compose logs -f

docker-restart: ## Restart Docker containers
	@echo "🔄 Restarting Docker containers..."
	@docker compose restart

# Setup
setup: ## Complete setup (restore, build, docker, migrate)
	@echo "🚀 Setting up Clean Architecture API..."
	@make restore
	@make build
	@make docker-up
	@sleep 15
	@make migrate
	@echo "✅ Setup completed successfully!"

setup-dev: ## Setup for development environment
	@echo "🔧 Setting up development environment..."
	@make setup

reset: ## Reset everything (docker down, up, migrate)
	@echo "🔄 Resetting environment..."
	@make docker-down
	@make docker-up
	@sleep 15
	@make migrate

# Code Quality
format: ## Format code
	@echo "🎨 Formatting code..."
	@dotnet format

lint: ## Check code formatting
	@echo "🔍 Checking code formatting..."
	@dotnet format --verify-no-changes

# Utilities
logs: ## View application logs
	@echo "📋 Viewing logs..."
	@docker compose logs -f cleanarch-postgres cleanarch-pgadmin

swagger: ## Open Swagger UI
	@echo "📚 Opening Swagger UI..."
	@open https://localhost:7000/swagger || xdg-open https://localhost:7000/swagger || start https://localhost:7000/swagger

pgadmin: ## Open pgAdmin
	@echo "🗄️ Opening pgAdmin..."
	@open http://localhost:5050 || xdg-open http://localhost:5050 || start http://localhost:5050

health: ## Check API health
	@echo "🏥 Checking API health..."
	@curl -k https://localhost:7000/health || echo "API not running"

# Publishing
publish: ## Publish the application
	@echo "📦 Publishing application..."
	@dotnet publish src/CleanArchitecture.API -c Release -o ./publish

# Aliases
start: dev ## Alias for dev
stop: docker-down ## Alias for docker-down
up: docker-up ## Alias for docker-up
down: docker-down ## Alias for docker-down
