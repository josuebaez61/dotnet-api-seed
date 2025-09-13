# Scripts y Automatización

Este documento describe todos los scripts y herramientas de automatización disponibles para el proyecto Clean Architecture API.

## 🚀 Opciones de Automatización

El proyecto incluye múltiples opciones para automatizar tareas comunes:

1. **npm scripts** (Recomendado para desarrollo)
2. **Scripts de Bash** (Unix/Linux/macOS)
3. **Scripts de PowerShell** (Windows)
4. **Makefile** (Unix/Linux/macOS)
5. **VS Code Tasks** (Editor integrado)

## 📦 npm Scripts (Recomendado)

### **Instalación**
```bash
npm install
```

### **Scripts Disponibles**

#### **Desarrollo**
```bash
npm run dev              # Iniciar servidor de desarrollo
npm start               # Alias para dev
npm run build           # Compilar la solución
npm run clean           # Limpiar artefactos de compilación
npm run restore         # Restaurar paquetes NuGet
```

#### **Base de Datos**
```bash
npm run db:migrate      # Ejecutar migraciones
npm run db:migrate:dev  # Migraciones en entorno de desarrollo
npm run db:migrate:prod # Migraciones en entorno de producción
npm run db:add-migration # Agregar nueva migración
npm run db:remove-migration # Remover última migración
npm run db:drop         # Eliminar base de datos
npm run db:seed         # Sembrar datos iniciales
```

#### **Docker**
```bash
npm run docker:up       # Iniciar contenedores Docker
npm run docker:down     # Detener contenedores Docker
npm run docker:logs     # Ver logs de contenedores
npm run docker:restart  # Reiniciar contenedores
```

#### **Configuración y Utilidades**
```bash
npm run setup           # Configuración completa del proyecto
npm run setup:dev       # Configuración para desarrollo
npm run reset           # Resetear entorno (Docker + DB)
npm run logs            # Ver logs de la aplicación
npm run pgadmin         # Abrir pgAdmin
npm run swagger         # Abrir Swagger UI
npm run health          # Verificar salud de la API
```

#### **Calidad de Código**
```bash
npm run format          # Formatear código
npm run lint            # Verificar formato de código
npm run test            # Ejecutar pruebas
npm run publish         # Publicar aplicación
```

## 🐚 Scripts de Bash (Unix/Linux/macOS)

### **Scripts Disponibles**

#### **setup.sh** - Configuración inicial
```bash
./scripts/setup.sh
```
- Restaura paquetes NuGet
- Compila la solución
- Inicia contenedores Docker
- Espera a que PostgreSQL esté listo
- Ejecuta migraciones de base de datos

#### **dev.sh** - Entorno de desarrollo
```bash
./scripts/dev.sh
```
- Verifica que Docker esté ejecutándose
- Inicia la API en modo desarrollo
- Muestra información útil sobre endpoints

#### **db.sh** - Gestión de base de datos
```bash
./scripts/db.sh migrate          # Ejecutar migraciones
./scripts/db.sh add-migration    # Agregar migración
./scripts/db.sh remove-migration # Remover migración
./scripts/db.sh drop             # Eliminar base de datos
./scripts/db.sh reset            # Resetear base de datos
./scripts/db.sh status           # Estado de migraciones
./scripts/db.sh backup           # Crear respaldo
./scripts/db.sh restore          # Restaurar desde respaldo
```

#### **test.sh** - Pruebas y calidad
```bash
./scripts/test.sh test           # Ejecutar pruebas
./scripts/test.sh build          # Compilar solución
./scripts/test.sh format         # Formatear código
./scripts/test.sh lint           # Verificar formato
./scripts/test.sh clean          # Limpiar artefactos
./scripts/test.sh restore        # Restaurar paquetes
./scripts/test.sh all            # Ejecutar todas las verificaciones
```

### **Hacer Scripts Ejecutables**
```bash
chmod +x scripts/*.sh
```

## 💻 Scripts de PowerShell (Windows)

### **setup.ps1** - Configuración inicial
```powershell
.\scripts\setup.ps1
```
- Verifica que Docker esté ejecutándose
- Verifica que .NET esté instalado
- Restaura paquetes NuGet
- Compila la solución
- Inicia contenedores Docker
- Espera a que PostgreSQL esté listo
- Ejecuta migraciones

### **Ejecutar en PowerShell**
```powershell
# Habilitar ejecución de scripts (si es necesario)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ejecutar script
.\scripts\setup.ps1
```

## 🔧 Makefile (Unix/Linux/macOS)

### **Comandos Disponibles**

#### **Desarrollo**
```bash
make dev              # Iniciar servidor de desarrollo
make build            # Compilar la solución
make clean            # Limpiar artefactos
make restore          # Restaurar paquetes
make test             # Ejecutar pruebas
```

#### **Base de Datos**
```bash
make migrate          # Ejecutar migraciones
make migrate-add NAME=migration_name  # Agregar migración
make migrate-remove   # Remover última migración
make db-drop          # Eliminar base de datos
make db-reset         # Resetear base de datos
```

#### **Docker**
```bash
make docker-up        # Iniciar contenedores
make docker-down      # Detener contenedores
make docker-logs      # Ver logs
make docker-restart   # Reiniciar contenedores
```

#### **Configuración**
```bash
make setup            # Configuración completa
make setup-dev        # Configuración para desarrollo
make reset            # Resetear entorno
```

#### **Utilidades**
```bash
make logs             # Ver logs de aplicación
make swagger          # Abrir Swagger UI
make pgadmin          # Abrir pgAdmin
make health           # Verificar salud de API
make format           # Formatear código
make lint             # Verificar formato
make publish          # Publicar aplicación
```

#### **Ayuda**
```bash
make help             # Mostrar todos los comandos disponibles
```

## 🎯 VS Code Tasks

### **Tareas Disponibles**

Accede a las tareas desde VS Code:
- `Ctrl+Shift+P` → "Tasks: Run Task"
- O desde el menú Terminal → Run Task

#### **Tareas Principales**
- **Build** - Compilar la solución
- **Run** - Ejecutar la API
- **Test** - Ejecutar pruebas
- **Clean** - Limpiar artefactos
- **Restore** - Restaurar paquetes

#### **Tareas de Docker**
- **Docker Up** - Iniciar contenedores
- **Docker Down** - Detener contenedores

#### **Tareas de Base de Datos**
- **Database Migrate** - Ejecutar migraciones
- **Add Migration** - Agregar nueva migración

#### **Tareas de Calidad**
- **Format Code** - Formatear código

### **Configuración de Debug**

El proyecto incluye configuraciones de debug para VS Code:
- **Launch API** - Ejecutar en modo desarrollo
- **Launch API (Production)** - Ejecutar en modo producción

## 🔧 Configuración de Entorno

### **Variables de Entorno**

Copia `env.example` a `.env` y configura las variables:

```bash
cp env.example .env
```

#### **Variables Principales**
```env
# Base de Datos
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

## 🚀 Flujos de Trabajo Recomendados

### **Configuración Inicial**
```bash
# Opción 1: npm (Recomendado)
npm run setup

# Opción 2: Script de Bash
./scripts/setup.sh

# Opción 3: Makefile
make setup

# Opción 4: PowerShell (Windows)
.\scripts\setup.ps1
```

### **Desarrollo Diario**
```bash
# Iniciar desarrollo
npm run dev

# En otra terminal: ver logs
npm run logs

# Abrir Swagger
npm run swagger
```

### **Gestión de Base de Datos**
```bash
# Agregar migración
npm run db:add-migration AddNewFeature

# Ejecutar migraciones
npm run db:migrate

# Resetear base de datos
npm run reset
```

### **Antes de Commit**
```bash
# Verificar calidad de código
npm run lint

# Ejecutar pruebas
npm run test

# Formatear código
npm run format
```

## 🐳 Comandos Docker Útiles

### **Gestión de Contenedores**
```bash
# Ver estado de contenedores
docker compose ps

# Ver logs en tiempo real
docker compose logs -f

# Reiniciar un contenedor específico
docker compose restart cleanarch-postgres

# Ejecutar comandos en contenedor
docker exec -it cleanarch-postgres psql -U postgres -d CleanArchitectureDB
```

### **Limpieza de Docker**
```bash
# Limpiar contenedores parados
docker container prune

# Limpiar imágenes no utilizadas
docker image prune

# Limpiar volúmenes no utilizados
docker volume prune

# Limpieza completa
docker system prune -a
```

## 🔍 Troubleshooting

### **Problemas Comunes**

#### **Docker no está ejecutándose**
```bash
# Verificar estado
docker info

# Iniciar Docker Desktop (macOS/Windows)
# O iniciar servicio (Linux)
sudo systemctl start docker
```

#### **PostgreSQL no responde**
```bash
# Verificar logs
docker compose logs cleanarch-postgres

# Reiniciar contenedor
docker compose restart cleanarch-postgres

# Verificar conectividad
docker exec cleanarch-postgres pg_isready -U postgres
```

#### **Migraciones fallan**
```bash
# Verificar conexión a base de datos
npm run health

# Resetear base de datos
npm run reset

# Verificar migraciones pendientes
dotnet ef migrations list --project CleanArchitecture.Infrastructure
```

#### **Puertos ocupados**
```bash
# Verificar puertos en uso
lsof -i :7000  # API
lsof -i :5432  # PostgreSQL
lsof -i :5050  # pgAdmin

# Cambiar puertos en docker-compose.yml si es necesario
```

## 📚 Recursos Adicionales

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Entity Framework Core CLI](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- [ASP.NET Core Development](https://docs.microsoft.com/en-us/aspnet/core/)
- [VS Code Tasks](https://code.visualstudio.com/docs/editor/tasks)

## 🎉 Conclusión

Con estos scripts y herramientas, puedes:

- ✅ **Configurar** el proyecto en segundos
- ✅ **Desarrollar** con comandos simples
- ✅ **Gestionar** la base de datos fácilmente
- ✅ **Automatizar** tareas repetitivas
- ✅ **Mantener** la calidad del código
- ✅ **Desplegar** la aplicación

**¡Elige la opción que más te guste y comienza a desarrollar!** 🚀
