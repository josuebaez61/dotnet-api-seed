# Clean Architecture .NET Core Project

Este es un proyecto de Clean Architecture implementado en .NET Core con Entity Framework Core, Identity, y PostgreSQL.

## 📁 Estructura del Proyecto

```
src/
├── CleanArchitecture.API/          # Capa de presentación (Web API)
├── CleanArchitecture.Application/  # Capa de aplicación (CQRS, DTOs, Servicios)
├── CleanArchitecture.Domain/       # Capa de dominio (Entidades, Interfaces)
└── CleanArchitecture.Infrastructure/ # Capa de infraestructura (EF Core, Repositorios)

docs/                               # Documentación del proyecto
├── README.md                       # Documentación principal
├── AUTHENTICATION.md               # Sistema de autenticación
├── PERMISSIONS_AND_ROLES.md        # Sistema de permisos y roles
├── HIERARCHICAL_PERMISSIONS.md     # Sistema de permisos jerárquicos
├── NEW_PERMISSIONS_MIGRATION.md    # Migración al nuevo sistema de permisos
├── ERROR_HANDLING.md               # Manejo de errores
├── LOCALIZATION_AND_EMAIL.md       # Localización y emails
├── UTC_DATETIME_SYSTEM.md          # Sistema de fechas UTC
└── ...                             # Más documentación
```

## 🚀 Características Principales

- **Clean Architecture**: Separación clara de responsabilidades
- **CQRS**: Patrón Command Query Responsibility Segregation
- **Entity Framework Core**: ORM con PostgreSQL
- **ASP.NET Core Identity**: Sistema de autenticación y autorización
- **AutoMapper**: Mapeo automático de objetos
- **MediatR**: Mediator pattern para desacoplamiento
- **UTC DateTime**: Manejo automático de fechas UTC
- **Snake Case**: Convención de nomenclatura para base de datos
- **Lazy Loading**: Carga diferida de entidades relacionadas

## 📚 Documentación

La documentación completa del proyecto se encuentra en la carpeta `docs/`:

### **Documentación Principal:**

- [📖 README Principal](docs/README.md) - Documentación completa del proyecto
- [🔐 Autenticación](docs/AUTHENTICATION.md) - Sistema de autenticación JWT
- [👥 Permisos y Roles](docs/PERMISSIONS_AND_ROLES.md) - Sistema de autorización

### **APIs y Endpoints:**

- [👤 Usuario Roles - GET](docs/USER_ROLES_ENDPOINT.md) - Endpoint para obtener roles de usuario
- [✏️ Usuario Roles - PUT](docs/UPDATE_USER_ROLES_ENDPOINT.md) - Endpoint para actualizar roles de usuario
- [🔐 Usuario Permisos - GET](docs/USER_PERMISSIONS_ENDPOINT.md) - Endpoint para obtener permisos de usuario

### **Infraestructura:**

- [🗄️ Configuraciones EF Core](docs/AUTOMATIC_CONFIGURATION_APPLICATION.md) - Configuraciones automáticas
- [🔧 Identity Refactor](docs/IDENTITY_CONFIGURATION_REFACTOR.md) - Refactorización de configuraciones Identity
- [📅 Sistema UTC DateTime](docs/UTC_DATETIME_SYSTEM.md) - Manejo de fechas UTC
- [⚠️ Manejo de Errores](docs/ERROR_HANDLING.md) - Sistema de excepciones custom

### **Funcionalidades:**

- [🌐 Localización y Email](docs/LOCALIZATION_AND_EMAIL.md) - Sistema de emails y localización
- [🔑 Recuperación de Contraseña](docs/PASSWORD_RECOVERY.md) - Flujo de recuperación
- [🏗️ Permisos Jerárquicos](docs/HIERARCHICAL_PERMISSIONS.md) - Sistema de permisos avanzado

### **Automatización:**

- [🤖 Scripts y Automatización](docs/SCRIPTS_AND_AUTOMATION.md) - Scripts de desarrollo
- [📝 Ejemplos de Manejo de Errores](docs/ERROR_HANDLING_EXAMPLES.md) - Ejemplos prácticos

## 🛠️ Configuración Rápida

### **Prerrequisitos:**

- .NET 9.0 SDK
- PostgreSQL
- Node.js (para scripts de desarrollo)

### **Instalación:**

```bash
# Clonar el repositorio
git clone <repository-url>
cd CleanArchitecture

# Restaurar dependencias
dotnet restore

# Configurar base de datos
# Editar connection string en appsettings.json

# Ejecutar migraciones
dotnet ef database update --project src/CleanArchitecture.Infrastructure

# Ejecutar la aplicación
dotnet run --project src/CleanArchitecture.API
```

## 🔧 Comandos Útiles

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion --project src/CleanArchitecture.Infrastructure

# Aplicar migraciones
dotnet ef database update --project src/CleanArchitecture.Infrastructure

# Ejecutar tests
dotnet test

# Compilar proyecto
dotnet build

# Ejecutar con hot reload
dotnet watch run --project src/CleanArchitecture.API
```

## 📋 Endpoints Principales

### **Autenticación:**

- `POST /api/v1/auth/login` - Iniciar sesión
- `POST /api/v1/auth/register` - Registrar usuario
- `POST /api/v1/auth/refresh-token` - Renovar token

### **Usuarios:**

- `GET /api/v1/users` - Listar usuarios (paginado)
- `GET /api/v1/users/id/{id}` - Obtener usuario por ID
- `GET /api/v1/users/id/{id}/roles` - Obtener roles de usuario
- `PUT /api/v1/users/id/{id}/roles` - Actualizar roles de usuario

### **Roles:**

- `GET /api/v1/roles` - Listar roles
- `GET /api/v1/roles/id/{id}` - Obtener rol por ID

### **Países y Estados:**

- `GET /api/v1/countries` - Listar países
- `GET /api/v1/countries/{countryId}/states` - Estados por país
- `GET /api/v1/cities` - Listar ciudades

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 📞 Soporte

Si tienes preguntas o necesitas ayuda, por favor:

- Revisa la documentación en `docs/`
- Abre un issue en el repositorio
- Contacta al equipo de desarrollo

---

**¡Gracias por usar Clean Architecture .NET Core!** 🎉
