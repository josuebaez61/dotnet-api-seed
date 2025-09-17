using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Infrastructure.Data.Converters
{
  /// <summary>
  /// Value converter que asegura que todas las fechas DateTime se almacenen y recuperen como UTC
  /// </summary>
  public class UtcDateTimeValueConverter : ValueConverter<DateTime, DateTime>
  {
    public UtcDateTimeValueConverter() : base(
        // Convertir a UTC al guardar en la base de datos
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        // Especificar que el valor recuperado es UTC
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
  }
}
