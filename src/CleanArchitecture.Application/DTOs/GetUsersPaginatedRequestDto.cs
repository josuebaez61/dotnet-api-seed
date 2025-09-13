using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.DTOs
{
  public class GetUsersPaginatedRequestDto : PaginationRequest
  {
    /// <summary>
    /// Filtrar por nombre de usuario
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Filtrar por email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Filtrar por nombre
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Filtrar por apellido
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Filtrar por estado activo
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filtrar por confirmación de email
    /// </summary>
    public bool? EmailConfirmed { get; set; }

    /// <summary>
    /// Filtrar por fecha de creación (desde)
    /// </summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Filtrar por fecha de creación (hasta)
    /// </summary>
    public DateTime? CreatedTo { get; set; }
  }
}
