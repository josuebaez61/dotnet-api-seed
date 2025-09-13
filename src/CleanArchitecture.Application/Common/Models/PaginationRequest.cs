using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.Common.Models
{
  public class PaginationRequest
  {
    private int _page = 1;
    private int _limit = 10;

    /// <summary>
    /// Número de página (empezando desde 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page
    {
      get => _page;
      set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Número de elementos por página
    /// </summary>
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    public int Limit
    {
      get => _limit;
      set => _limit = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// Búsqueda global que se aplicará a todos los campos de texto
    /// </summary>
    public string? GlobalSearch { get; set; }

    /// <summary>
    /// Campo por el cual ordenar
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Dirección del ordenamiento (asc/desc)
    /// </summary>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// Calcula el offset para la consulta SQL
    /// </summary>
    public int Offset => (Page - 1) * Limit;

    /// <summary>
    /// Verifica si el ordenamiento es descendente
    /// </summary>
    public bool IsDescending => !string.IsNullOrEmpty(SortDirection) &&
                               SortDirection.ToLower() == "desc";
  }
}
