# Scripts and Automation

This document describes all the scripts and automation tools available for the Clean Architecture API project.

## 🚀 Automation Options

The project includes multiple options for automating common tasks:

1. **npm scripts** (Recommended for development)
2. **Bash scripts** (Unix/Linux/macOS)
3. **PowerShell scripts** (Windows)
4. **Makefile** (Unix/Linux/macOS)
5. **VS Code Tasks** (Integrated editor)

## 📦 npm Scripts (Recommended)

### **Installation**
```bash
npm install
```

### **Available Scripts**

#### **Development**
```bash
npm run dev              # Start development server
npm start               # Alias for dev
npm run build           # Compile the solution
npm run clean           # Clean compilation artifacts
npm run restore         # Restore NuGet packages
```

#### **Database**
```bash
npm run db:migrate      # Run migrations
npm run db:migrate:dev  # Migrations in development environment
npm run db:migrate:prod # Migrations in production environment
npm run db:add-migration # Add new migration
npm run db:remove-migration # Remove last migration
npm run db:drop         # Drop database
npm run db:seed         # Seed initial data
```

#### **Docker**
```bash
npm run docker:up       # Start Docker containers
npm run docker:down     # Stop Docker containers
npm run docker:logs     # View container logs
npm run docker:restart  # Restart containers
```

#### **Configuration and Utilities**
```bash
npm run setup           # Complete project setup
npm run setup:dev       # Development setup
npm run reset           # Reset environment (Docker + DB)
npm run logs            # View application logs
npm run pgadmin         # Open pgAdmin
npm run swagger         # Open Swagger UI
npm run health          # Check API health
```

#### **Code Quality**
```bash
npm run format          # Format code
npm run lint            # Check code format
npm run test            # Run tests
npm run publish         # Publish application
```

## 🐚 Bash Scripts (Unix/Linux/macOS)

### **Available Scripts**

#### **setup.sh** - Initial setup
```bash
./scripts/setup.sh
```
- Restores NuGet packages
- Compiles the solution
- Starts Docker containers
- Waits for PostgreSQL to be ready
- Runs database migrations

#### **dev.sh** - Development environment
```bash
./scripts/dev.sh
```
- Verifies Docker is running
- Starts the API in development mode
- Shows useful information about endpoints

#### **db.sh** - Database management
```bash
./scripts/db.sh migrate          # Run migrations
./scripts/db.sh add-migration    # Add migration
./scripts/db.sh remove-migration # Remove migration
./scripts/db.sh drop             # Drop database
./scripts/db.sh reset            # Reset database
./scripts/db.sh status           # Migration status
./scripts/db.sh backup           # Create backup
./scripts/db.sh restore          # Restore from backup
```

#### **test.sh** - Testing and quality
```bash
./scripts/test.sh test           # Run tests
./scripts/test.sh build          # Compile solution
./scripts/test.sh format         # Format code
./scripts/test.sh lint           # Check format
./scripts/test.sh clean          # Clean artifacts
./scripts/test.sh restore        # Restore packages
./scripts/test.sh all            # Run all checks
```

### **Make Scripts Executable**
```bash
chmod +x scripts/*.sh
```

## 💻 PowerShell Scripts (Windows)

### **setup.ps1** - Initial setup
```powershell
.\scripts\setup.ps1
```
- Verifies Docker is running
- Verifies .NET is installed
- Restores NuGet packages
- Compiles the solution
- Starts Docker containers
- Waits for PostgreSQL to be ready
- Runs migrations

### **Run in PowerShell**
```powershell
# Enable script execution (if necessary)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Run script
.\scripts\setup.ps1
```

## 🔧 Makefile (Unix/Linux/macOS)

### **Available Commands**

#### **Development**
```bash
make dev              # Start development server
make build            # Compile the solution
make clean            # Clean artifacts
make restore          # Restore packages
make test             # Run tests
```

#### **Database**
```bash
make migrate          # Run migrations
make migrate-add NAME=migration_name  # Add migration
make migrate-remove   # Remove last migration
make db-drop          # Drop database
make db-reset         # Reset database
```

#### **Docker**
```bash
make docker-up        # Start containers
make docker-down      # Stop containers
make docker-logs      # View logs
make docker-restart   # Restart containers
```

#### **Configuration**
```bash
make setup            # Complete setup
make setup-dev        # Development setup
make reset            # Reset environment
```

#### **Utilities**
```bash
make logs             # View application logs
make swagger          # Open Swagger UI
make pgadmin          # Open pgAdmin
make health           # Check API health
make format           # Format code
make lint             # Check format
make publish          # Publish application
```

#### **Help**
```bash
make help             # Show all available commands
```

## 🎯 VS Code Tasks

### **Available Tasks**

Access tasks from VS Code:
- `Ctrl+Shift+P` → "Tasks: Run Task"
- Or from Terminal menu → Run Task

#### **Main Tasks**
- **Build** - Compile the solution
- **Run** - Run the API
- **Test** - Run tests
- **Clean** - Clean artifacts
- **Restore** - Restore packages

#### **Docker Tasks**
- **Docker Up** - Start containers
- **Docker Down** - Stop containers

#### **Database Tasks**
- **Database Migrate** - Run migrations
- **Add Migration** - Add new migration

#### **Quality Tasks**
- **Format Code** - Format code

### **Debug Configuration**

The project includes VS Code debug configurations:
- **Launch API** - Run in development mode
- **Launch API (Production)** - Run in production mode

## 🔧 Environment Configuration

### **Environment Variables**

Copy `env.example` to `.env` and configure the variables:

```bash
cp env.example .env
```

#### **Main Variables**
```env
# Database
POSTGRES_DB=CleanArchitectureDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# JWT
JWT_SECRET_KEY=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
JWT_ISSUER=CleanArchitecture
JWT_AUDIENCE=CleanArchitectureUsers

# Email
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

## 🚀 Recommended Workflows

### **Initial Setup**
```bash
# Option 1: npm (Recommended)
npm run setup

# Option 2: Bash script
./scripts/setup.sh

# Option 3: Makefile
make setup

# Option 4: PowerShell (Windows)
.\scripts\setup.ps1
```

### **Daily Development**
```bash
# Start development
npm run dev

# In another terminal: view logs
npm run logs

# Open Swagger
npm run swagger
```

### **Database Management**
```bash
# Add migration
npm run db:add-migration AddNewFeature

# Run migrations
npm run db:migrate

# Reset database
npm run reset
```

### **Before Commit**
```bash
# Check code quality
npm run lint

# Run tests
npm run test

# Format code
npm run format
```

## 🐳 Useful Docker Commands

### **Container Management**
```bash
# View container status
docker compose ps

# View logs in real time
docker compose logs -f

# Restart specific container
docker compose restart cleanarch-postgres

# Execute commands in container
docker exec -it cleanarch-postgres psql -U postgres -d CleanArchitectureDB
```

### **Docker Cleanup**
```bash
# Clean stopped containers
docker container prune

# Clean unused images
docker image prune

# Clean unused volumes
docker volume prune

# Complete cleanup
docker system prune -a
```

## 🔍 Troubleshooting

### **Common Problems**

#### **Docker is not running**
```bash
# Check status
docker info

# Start Docker Desktop (macOS/Windows)
# Or start service (Linux)
sudo systemctl start docker
```

#### **PostgreSQL not responding**
```bash
# Check logs
docker compose logs cleanarch-postgres

# Restart container
docker compose restart cleanarch-postgres

# Check connectivity
docker exec cleanarch-postgres pg_isready -U postgres
```

#### **Migrations failing**
```bash
# Check database connection
npm run health

# Reset database
npm run reset

# Check pending migrations
dotnet ef migrations list --project CleanArchitecture.Infrastructure
```

#### **Ports occupied**
```bash
# Check ports in use
lsof -i :7000  # API
lsof -i :5432  # PostgreSQL
lsof -i :5050  # pgAdmin

# Change ports in docker-compose.yml if necessary
```

## 📚 Additional Resources

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Entity Framework Core CLI](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- [ASP.NET Core Development](https://docs.microsoft.com/en-us/aspnet/core/)
- [VS Code Tasks](https://code.visualstudio.com/docs/editor/tasks)

## 🎉 Conclusion

With these scripts and tools, you can:

- ✅ **Configure** the project in seconds
- ✅ **Develop** with simple commands
- ✅ **Manage** the database easily
- ✅ **Automate** repetitive tasks
- ✅ **Maintain** code quality
- ✅ **Deploy** the application

**Choose the option you like best and start developing!** 🚀