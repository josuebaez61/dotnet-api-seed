# Refactorización de Configuración de Identity

Se ha refactorizado exitosamente toda la configuración de Entity Framework Identity del método `OnModelCreating` a archivos de configuración separados, siguiendo las mejores prácticas de Clean Architecture.

## Archivos Creados

### Configuraciones de Identity

1. **`UserConfiguration.cs`** - Configuración de la entidad User
2. **`RoleConfiguration.cs`** - Configuración de la entidad Role
3. **`UserRoleConfiguration.cs`** - Configuración de la entidad UserRole
4. **`UserClaimConfiguration.cs`** - Configuración de la entidad UserClaim
5. **`UserLoginConfiguration.cs`** - Configuración de la entidad UserLogin
6. **`UserTokenConfiguration.cs`** - Configuración de la entidad UserToken
7. **`RoleClaimConfiguration.cs`** - Configuración de la entidad RoleClaim

## Beneficios de la Refactorización

### 1. **Separación de Responsabilidades**

- Cada entidad tiene su propia configuración
- Código más organizado y mantenible
- Fácil de encontrar y modificar configuraciones específicas

### 2. **Consistencia**

- Todas las configuraciones siguen el mismo patrón
- Nombres de tablas en snake_case automáticamente
- Manejo UTC de fechas automático

### 3. **Mantenibilidad**

- Cambios a una entidad no afectan otras configuraciones
- Fácil de agregar nuevas propiedades o relaciones
- Código más legible y documentado

### 4. **Testabilidad**

- Cada configuración puede ser probada independientemente
- Fácil de mockear para pruebas unitarias

## Antes vs Después

### Antes (ApplicationDbContext.cs)

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // ... muchas líneas de configuración manual ...

    // Configure User entity properties
    builder.Entity<User>(entity =>
    {
        entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        // ... más configuraciones ...
    });

    // Configure Role entity properties
    builder.Entity<Role>(entity =>
    {
        entity.Property(e => e.Description).HasMaxLength(500);
        // ... más configuraciones ...
    });

    // ... muchas más configuraciones ...
}
```

### Después (ApplicationDbContext.cs)

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Apply snake_case naming convention and UTC DateTime handling
    ApplySnakeCaseNamingAndUtcDateTimeHandling(builder);

    // Apply entity configurations
    builder.ApplyConfiguration(new UserConfiguration());
    builder.ApplyConfiguration(new RoleConfiguration());
    builder.ApplyConfiguration(new UserRoleConfiguration());
    builder.ApplyConfiguration(new UserClaimConfiguration());
    builder.ApplyConfiguration(new UserLoginConfiguration());
    builder.ApplyConfiguration(new UserTokenConfiguration());
    builder.ApplyConfiguration(new RoleClaimConfiguration());
    // ... otras configuraciones existentes ...
}
```

## Estructura de Archivos

```
src/CleanArchitecture.Infrastructure/Data/Configurations/
├── UserConfiguration.cs
├── RoleConfiguration.cs
├── UserRoleConfiguration.cs
├── UserClaimConfiguration.cs
├── UserLoginConfiguration.cs
├── UserTokenConfiguration.cs
├── RoleClaimConfiguration.cs
├── CountryConfiguration.cs
├── StateConfiguration.cs
├── CityConfiguration.cs
├── PermissionConfiguration.cs
├── RolePermissionConfiguration.cs
├── PasswordResetCodeConfiguration.cs
└── EmailVerificationCodeConfiguration.cs
```

## Características de Cada Configuración

### UserConfiguration.cs

- Configura propiedades personalizadas del User
- Define longitudes máximas para campos de texto
- Configura campos requeridos y opcionales
- Nombres de tabla en snake_case automático

### RoleConfiguration.cs

- Configura propiedades del Role
- Define longitud máxima para Description
- Campos de fecha UTC automáticos

### UserRoleConfiguration.cs

- Configura clave compuesta (UserId, RoleId)
- Define relaciones con User y Role
- Comportamiento de eliminación en cascada

### UserClaimConfiguration.cs

- Configura relación con User
- Eliminación en cascada

### UserLoginConfiguration.cs

- Configura clave compuesta (LoginProvider, ProviderKey)
- Relación con User
- Eliminación en cascada

### UserTokenConfiguration.cs

- Configura clave compuesta (UserId, LoginProvider, Name)
- Relación con User
- Eliminación en cascada

### RoleClaimConfiguration.cs

- Configura relación con Role
- Eliminación en cascada

## Ventajas Adicionales

1. **Reutilización**: Las configuraciones pueden ser reutilizadas en diferentes contextos
2. **Escalabilidad**: Fácil agregar nuevas entidades y configuraciones
3. **Debugging**: Más fácil identificar problemas en configuraciones específicas
4. **Documentación**: Cada configuración está bien documentada y es autoexplicativa

## Próximos Pasos

1. **Migración**: Generar nueva migración para aplicar cambios
2. **Testing**: Probar que todas las configuraciones funcionan correctamente
3. **Documentación**: Actualizar documentación del proyecto
4. **Code Review**: Revisar todas las configuraciones para asegurar consistencia

## Comando para Generar Migración

```bash
dotnet ef migrations add RefactorIdentityConfigurations --project src/CleanArchitecture.Infrastructure
```

Esta refactorización mejora significativamente la organización del código y sigue las mejores prácticas de Clean Architecture y Entity Framework Core.
