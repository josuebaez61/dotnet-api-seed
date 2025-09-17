using System;

namespace CleanArchitecture.Application.Common.Interfaces
{
  /// <summary>
  /// Servicio para manejar conversiones de zona horaria del usuario
  /// </summary>
  public interface IUserTimezoneService
  {
    /// <summary>
    /// Convierte una fecha UTC a la zona horaria del usuario
    /// </summary>
    /// <param name="utcDateTime">Fecha en UTC</param>
    /// <param name="userTimezone">Zona horaria del usuario (ej: "America/Argentina/Buenos_Aires")</param>
    /// <returns>Fecha convertida a la zona horaria del usuario</returns>
    DateTime ConvertFromUtc(DateTime utcDateTime, string userTimezone);

    /// <summary>
    /// Convierte una fecha de la zona horaria del usuario a UTC
    /// </summary>
    /// <param name="userDateTime">Fecha en la zona horaria del usuario</param>
    /// <param name="userTimezone">Zona horaria del usuario</param>
    /// <returns>Fecha convertida a UTC</returns>
    DateTime ConvertToUtc(DateTime userDateTime, string userTimezone);

    /// <summary>
    /// Convierte una fecha UTC nullable a la zona horaria del usuario
    /// </summary>
    /// <param name="utcDateTime">Fecha UTC nullable</param>
    /// <param name="userTimezone">Zona horaria del usuario</param>
    /// <returns>Fecha convertida o null si la fecha original era null</returns>
    DateTime? ConvertFromUtc(DateTime? utcDateTime, string userTimezone);

    /// <summary>
    /// Convierte una fecha nullable de la zona horaria del usuario a UTC
    /// </summary>
    /// <param name="userDateTime">Fecha nullable en la zona horaria del usuario</param>
    /// <param name="userTimezone">Zona horaria del usuario</param>
    /// <returns>Fecha convertida a UTC o null si la fecha original era null</returns>
    DateTime? ConvertToUtc(DateTime? userDateTime, string userTimezone);

    /// <summary>
    /// Obtiene la zona horaria actual del usuario desde el contexto HTTP
    /// </summary>
    /// <returns>Zona horaria del usuario o UTC por defecto</returns>
    string GetCurrentUserTimezone();

    /// <summary>
    /// Valida si una zona horaria es válida
    /// </summary>
    /// <param name="timezoneId">ID de la zona horaria</param>
    /// <returns>True si la zona horaria es válida</returns>
    bool IsValidTimezone(string timezoneId);

    /// <summary>
    /// Obtiene la zona horaria por defecto
    /// </summary>
    /// <returns>UTC como zona horaria por defecto</returns>
    string GetDefaultTimezone();
  }
}
