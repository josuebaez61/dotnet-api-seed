# Sistema de Manejo de Fechas UTC

Este sistema maneja las fechas de manera consistente en toda la aplicación, almacenando todas las fechas en UTC en la base de datos y proporcionando herramientas para convertir a la zona horaria del usuario.

## Características

- ✅ **Almacenamiento automático en UTC**: Entity Framework Core convierte automáticamente todas las fechas a UTC
- ✅ **Conversión transparente**: Las fechas se convierten automáticamente al guardar y recuperar
- ✅ **Header X-Timezone**: El cliente puede enviar su zona horaria para conversiones a nivel de aplicación
- ✅ **Value Converters**: EF Core maneja las conversiones automáticamente
- ✅ **Snake_case**: Nombres de tablas y columnas en formato snake_case
- ✅ **UpdatedAt automático**: Las fechas UpdatedAt se actualizan automáticamente

## Arquitectura

### 1. Entity Framework Level (Automático)

- **`UtcDateTimeValueConverter`**: Convierte automáticamente DateTime a UTC al guardar
- **`ApplicationDbContext`**: Configura automáticamente todas las propiedades DateTime
- **Value Converters**: Manejan DateTime y DateTime? automáticamente

### 2. Application Level (Manual)

- **`IUserTimezoneService`**: Para conversiones de zona horaria del usuario
- **`UserTimezoneMiddleware`**: Extrae la zona horaria del header X-Timezone
- **`UserTimezoneHelper`**: Helper para conversiones en controladores

## Cómo Funciona

### Almacenamiento en Base de Datos

```csharp
// Entity Framework convierte automáticamente a UTC
var user = new User
{
    CreatedAt = DateTime.Now, // Se convierte automáticamente a UTC
    UpdatedAt = DateTime.Now  // Se convierte automáticamente a UTC
};

await context.Users.AddAsync(user);
await context.SaveChangesAsync(); // Se guarda en UTC
```

### Recuperación de Base de Datos

```csharp
// Entity Framework devuelve fechas en UTC
var user = await context.Users.FirstAsync();
// user.CreatedAt está en UTC (DateTimeKind.Utc)
```

### Conversión a Zona Horaria del Usuario

```csharp
// Para mostrar al usuario en su zona horaria
var userTimezone = _timezoneService.GetCurrentUserTimezone();
var userLocalTime = _timezoneService.ConvertFromUtc(utcDateTime, userTimezone);
```

## Configuración

### 1. ApplicationDbContext

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Aplica snake_case y conversión UTC automáticamente
    ApplySnakeCaseNamingAndUtcDateTimeHandling(builder);
}

private void ApplySnakeCaseNamingAndUtcDateTimeHandling(ModelBuilder builder)
{
    foreach (var entity in builder.Model.GetEntityTypes())
    {
        foreach (var property in entity.GetProperties())
        {
            // Conversión automática a UTC
            if (property.ClrType == typeof(DateTime))
            {
                property.SetValueConverter(new UtcDateTimeValueConverter());
            }

            if (property.ClrType == typeof(DateTime?))
            {
                property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                    v => v.HasValue ? v.Value.ToUniversalTime() : v,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
                ));
            }

            // UpdatedAt se actualiza automáticamente
            if (property.Name == "UpdatedAt")
            {
                property.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            }
        }
    }
}
```

### 2. Value Converter

```csharp
public class UtcDateTimeValueConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeValueConverter() : base(
        // Convertir a UTC al guardar
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        // Especificar que es UTC al recuperar
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}
```

### 3. Middleware

```csharp
public class UserTimezoneMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var timezone = context.Request.Headers["X-Timezone"].FirstOrDefault() ?? "UTC";
        context.Items["UserTimezone"] = timezone;
        await _next(context);
    }
}
```

## Uso Práctico

### 1. En Controladores (Para mostrar al usuario)

```csharp
[HttpGet("users")]
public async Task<ActionResult<List<UserDto>>> GetUsers()
{
    var users = await _context.Users.ToListAsync();

    // Convertir fechas a la zona horaria del usuario
    var userTimezone = _timezoneService.GetCurrentUserTimezone();
    var userDtos = users.Select(u => new UserDto
    {
        Id = u.Id,
        Name = u.Name,
        CreatedAt = _timezoneService.ConvertFromUtc(u.CreatedAt, userTimezone),
        UpdatedAt = u.UpdatedAt.HasValue ?
            _timezoneService.ConvertFromUtc(u.UpdatedAt.Value, userTimezone) : null
    }).ToList();

    return Ok(userDtos);
}
```

### 2. En Controladores (Para recibir del usuario)

```csharp
[HttpPost("users")]
public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
{
    // Convertir la fecha del usuario a UTC antes de guardar
    var userTimezone = _timezoneService.GetCurrentUserTimezone();
    var utcDateOfBirth = _timezoneService.ConvertToUtc(request.DateOfBirth, userTimezone);

    var user = new User
    {
        Name = request.Name,
        DateOfBirth = utcDateOfBirth, // EF Core lo manejará automáticamente
        CreatedAt = DateTime.UtcNow
    };

    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();

    return Ok(user);
}
```

### 3. Usando Helper

```csharp
[HttpPost("users")]
public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
{
    // Usar helper para conversión
    var utcDateOfBirth = UserTimezoneHelper.ConvertClientDateTimeToUtc(
        request.DateOfBirth, _timezoneService, HttpContext);

    // ... resto del código
}
```

## Ventajas de esta Implementación

### 1. **Automática en EF Core**

- No necesitas recordar convertir fechas manualmente
- Todas las entidades manejan UTC automáticamente
- Consistencia garantizada

### 2. **Flexible para UI**

- Puedes convertir a la zona horaria del usuario cuando sea necesario
- Header X-Timezone para personalización
- Fallback a UTC si no se especifica zona horaria

### 3. **Performance**

- Conversiones a nivel de base de datos
- No impacto en queries
- Manejo eficiente de memoria

### 4. **Mantenible**

- Configuración centralizada en ApplicationDbContext
- No necesitas cambiar código en cada entidad
- Fácil de extender

## Testing

### 1. Verificar que las fechas se almacenan en UTC

```csharp
[Test]
public async Task Should_Store_DateTime_In_UTC()
{
    // Arrange
    var localTime = DateTime.Now;
    var user = new User { CreatedAt = localTime };

    // Act
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();

    // Assert
    var savedUser = await _context.Users.FirstAsync();
    Assert.AreEqual(DateTimeKind.Utc, savedUser.CreatedAt.Kind);
}
```

### 2. Verificar conversión de zona horaria

```csharp
[Test]
public void Should_Convert_UTC_To_User_Timezone()
{
    // Arrange
    var utcTime = DateTime.UtcNow;
    var userTimezone = "America/Argentina/Buenos_Aires";

    // Act
    var localTime = _timezoneService.ConvertFromUtc(utcTime, userTimezone);

    // Assert
    Assert.AreEqual(DateTimeKind.Unspecified, localTime.Kind);
    Assert.AreNotEqual(utcTime, localTime);
}
```

## Consideraciones Importantes

1. **Siempre UTC en BD**: Todas las fechas en la base de datos están en UTC
2. **Conversión manual para UI**: Solo convierte a zona horaria del usuario para mostrar
3. **Header X-Timezone**: Opcional, fallback a UTC
4. **DateTime.Now vs DateTime.UtcNow**: Ambos funcionan, EF Core los convierte automáticamente
5. **UpdatedAt**: Se actualiza automáticamente en cada SaveChanges

## Migración

Si ya tienes datos en la base de datos, necesitarás una migración para convertir las fechas existentes a UTC:

```sql
-- Ejemplo de migración (ajustar según tu esquema)
UPDATE users SET created_at = created_at AT TIME ZONE 'UTC';
UPDATE users SET updated_at = updated_at AT TIME ZONE 'UTC';
```

¡Este sistema te da lo mejor de ambos mundos: automatización en EF Core y flexibilidad para la UI!
