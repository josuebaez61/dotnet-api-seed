# 📚 Documentación del Proyecto Clean Architecture

Bienvenido a la documentación completa del proyecto Clean Architecture ASP.NET Core. Esta documentación te guiará a través de todas las funcionalidades implementadas y cómo utilizarlas.

## 📋 Documentos Disponibles

### 🔐 [AUTHENTICATION.md](AUTHENTICATION.md)
**Sistema de Autenticación Completo**
- ✅ Autenticación JWT con tokens de acceso y renovación
- ✅ Login con email o username
- ✅ Registro de usuarios con validaciones
- ✅ Cambio de contraseña
- ✅ Endpoints protegidos con autorización
- ✅ Validaciones robustas con FluentValidation

### 🔄 [PASSWORD_RECOVERY.md](PASSWORD_RECOVERY.md)
**Sistema de Recuperación de Contraseña**
- ✅ Solicitud de reset por email
- ✅ Códigos de 6 dígitos seguros con expiración
- ✅ Correos HTML con diseño profesional
- ✅ Validaciones robustas de códigos y contraseñas
- ✅ Confirmación por email del cambio exitoso

### 🛡️ [PERMISSIONS_AND_ROLES.md](PERMISSIONS_AND_ROLES.md)
**Sistema de Permisos y Roles**
- ✅ Gestión completa de permisos (CRUD)
- ✅ Gestión de roles con asignación de permisos
- ✅ Autorización granular por endpoint
- ✅ Tokens JWT con permisos incluidos
- ✅ Políticas de autorización configuradas
- ✅ Roles predefinidos (Admin, User) con permisos

### 🌍 [LOCALIZATION_AND_EMAIL.md](LOCALIZATION_AND_EMAIL.md)
**Localización y Servicio de Correos**
- ✅ Soporte para Español e Inglés
- ✅ Mensajes localizados en todas las respuestas
- ✅ Configuración automática de idiomas
- ✅ Cambio dinámico de idioma por header/query
- ✅ Templates HTML con CSS moderno
- ✅ Correos de bienvenida, recuperación y confirmación

### 🚨 [ERROR_HANDLING.md](ERROR_HANDLING.md)
**Sistema de Manejo de Errores**
- ✅ Excepciones específicas con códigos únicos
- ✅ Localización automática de mensajes de error
- ✅ Middleware centralizado para manejo de excepciones
- ✅ Códigos HTTP apropiados mapeados automáticamente
- ✅ Respuestas estructuradas con metadatos consistentes
- ✅ Logging estructurado para debugging

### 🔍 [ERROR_HANDLING_EXAMPLES.md](ERROR_HANDLING_EXAMPLES.md)
**Ejemplos Prácticos de Manejo de Errores**
- ✅ Ejemplos de API con requests y responses
- ✅ Ejemplos de código en diferentes capas
- ✅ Casos de uso comunes paso a paso
- ✅ Testing de errores con unit tests e integration tests
- ✅ Configuración de testing y mocking

### 🛠️ [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md)
**Scripts y Automatización**
- ✅ Scripts npm para tareas comunes
- ✅ Scripts de Bash y PowerShell
- ✅ Makefile con comandos útiles
- ✅ Configuración de VS Code tasks
- ✅ Automatización de Docker y base de datos

## 🚀 Inicio Rápido

### 1. Configuración Inicial
```bash
# Clonar y configurar
git clone <repository>
cd CleanArchitecture

# Configuración completa
npm run setup

# Iniciar base de datos
npm run docker:up

# Aplicar migraciones
npm run db:migrate

# Iniciar aplicación
npm run dev
```

### 2. Usuario Admin por Defecto
```
Username: admin
Email: admin@example.com
Password: Admin123!
```

### 3. Endpoints Principales
- **Swagger UI**: `https://localhost:7000/swagger`
- **API Base**: `https://localhost:7000/api`
- **pgAdmin**: `http://localhost:5050` (admin@admin.com / admin)

## 🏗️ Arquitectura del Proyecto

```
CleanArchitecture/
├── docs/                    # 📚 Documentación completa
├── src/                     # 💻 Código fuente
│   ├── Domain/             # 🎯 Capa de Dominio
│   ├── Application/        # 🔄 Capa de Aplicación
│   ├── Infrastructure/     # 🔧 Capa de Infraestructura
│   └── API/               # 🌐 Capa de Presentación
├── scripts/               # 🤖 Scripts de automatización
└── .vscode/              # ⚙️ Configuración de VS Code
```

## 🎯 Características Principales

### ✅ **Autenticación y Autorización**
- JWT con refresh tokens
- Login con email/username
- Sistema de permisos granular
- Roles predefinidos (Admin, User)

### ✅ **Validación y Manejo de Errores**
- FluentValidation para validaciones
- Sistema de errores controlados
- Localización de mensajes
- Respuestas API consistentes

### ✅ **Base de Datos**
- PostgreSQL con Entity Framework Core
- Migraciones automáticas
- Seeding de datos iniciales
- Configuración Docker

### ✅ **Desarrollo y Testing**
- Scripts de automatización
- Configuración VS Code
- Swagger/OpenAPI
- Logging estructurado

## 🔧 Tecnologías Utilizadas

- **.NET 9.0** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Base de datos
- **ASP.NET Core Identity** - Autenticación
- **FluentValidation** - Validaciones
- **MediatR** - CQRS Pattern
- **Swagger/OpenAPI** - Documentación API

## 📖 Guías de Uso

### Para Desarrolladores
1. Lee [AUTHENTICATION.md](AUTHENTICATION.md) para entender el sistema de auth
2. Revisa [ERROR_HANDLING.md](ERROR_HANDLING.md) para manejo de errores
3. Consulta [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md) para desarrollo

### Para DevOps
1. Usa [SCRIPTS_AND_AUTOMATION.md](SCRIPTS_AND_AUTOMATION.md) para deployment
2. Configura Docker con `docker-compose.yml`
3. Aplica migraciones con scripts automatizados

### Para Testing
1. Revisa [ERROR_HANDLING_EXAMPLES.md](ERROR_HANDLING_EXAMPLES.md) para ejemplos
2. Usa los scripts de testing en `scripts/`
3. Configura VS Code tasks para debugging

## 🆘 Soporte y Contribución

### Reportar Issues
- Usa los templates de GitHub Issues
- Incluye logs y pasos para reproducir
- Especifica versión y entorno

### Contribuir
1. Fork el proyecto
2. Crea una rama para tu feature
3. Sigue las convenciones de código
4. Agrega tests si es necesario
5. Crea un Pull Request

### Contacto
- **Email**: [tu-email@example.com]
- **Issues**: [GitHub Issues](https://github.com/tu-usuario/tu-repo/issues)

## 📝 Changelog

### v1.0.0 (13 Enero, 2025)
- ✅ Sistema de autenticación completo
- ✅ Recuperación de contraseña
- ✅ Sistema de permisos y roles
- ✅ Localización (es/en)
- ✅ Manejo de errores robusto
- ✅ Scripts de automatización
- ✅ Documentación completa

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver [LICENSE](LICENSE) para más detalles.

---

**Última actualización:** 13 de Enero, 2025  
**Versión:** 1.0.0  
**Mantenido por:** [Tu Nombre]
