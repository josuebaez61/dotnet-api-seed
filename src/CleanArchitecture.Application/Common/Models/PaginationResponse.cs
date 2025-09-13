namespace CleanArchitecture.Application.Common.Models
{
  public class PaginationResponse<T>
  {
    /// <summary>
    /// Lista de elementos de la página actual
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Información de paginación
    /// </summary>
    public PaginationMetadata Metadata { get; set; } = new();
  }

  public class PaginationMetadata
  {
    /// <summary>
    /// Página actual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Número de elementos por página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros en la base de datos
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas disponibles
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indica si hay una página siguiente
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Indica si hay una página anterior
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Número de elementos en la página actual
    /// </summary>
    public int CurrentPageItemCount { get; set; }

    /// <summary>
    /// Número del primer elemento en la página actual (1-based)
    /// </summary>
    public int FirstItemIndex => TotalCount == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;

    /// <summary>
    /// Número del último elemento en la página actual (1-based)
    /// </summary>
    public int LastItemIndex => Math.Min(CurrentPage * PageSize, TotalCount);

    /// <summary>
    /// Término de búsqueda global aplicado
    /// </summary>
    public string? GlobalSearch { get; set; }

    /// <summary>
    /// Campo por el cual se está ordenando
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Dirección del ordenamiento
    /// </summary>
    public string? SortDirection { get; set; }
  }
}
