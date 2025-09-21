namespace CleanArchitecture.Application.Common.Constants
{
  /// <summary>
  /// Constantes para los modos de filtrado disponibles en las consultas paginadas
  /// </summary>
  public static class FilterMatchMode
  {
    /// <summary>
    /// Filtro que coincide si el valor comienza con el texto especificado
    /// </summary>
    public const string StartsWith = "startsWith";

    /// <summary>
    /// Filtro que coincide si el valor contiene el texto especificado
    /// </summary>
    public const string Contains = "contains";

    /// <summary>
    /// Filtro que coincide si el valor NO contiene el texto especificado
    /// </summary>
    public const string NotContains = "notContains";

    /// <summary>
    /// Filtro que coincide si el valor termina con el texto especificado
    /// </summary>
    public const string EndsWith = "endsWith";

    /// <summary>
    /// Filtro que coincide si el valor es igual al texto especificado
    /// </summary>
    public const string EqualsValue = "equals";

    /// <summary>
    /// Filtro que coincide si el valor NO es igual al texto especificado
    /// </summary>
    public const string NotEquals = "notEquals";

    /// <summary>
    /// Filtro que coincide si el valor está en la lista de valores especificados
    /// </summary>
    public const string In = "in";

    /// <summary>
    /// Filtro que coincide si el valor es menor que el especificado
    /// </summary>
    public const string LessThan = "lt";

    /// <summary>
    /// Filtro que coincide si el valor es menor o igual que el especificado
    /// </summary>
    public const string LessThanOrEqualTo = "lte";

    /// <summary>
    /// Filtro que coincide si el valor es mayor que el especificado
    /// </summary>
    public const string GreaterThan = "gt";

    /// <summary>
    /// Filtro que coincide si el valor es mayor o igual que el especificado
    /// </summary>
    public const string GreaterThanOrEqualTo = "gte";

    /// <summary>
    /// Filtro que coincide si el valor está entre dos valores especificados
    /// </summary>
    public const string Between = "between";

    /// <summary>
    /// Filtro que coincide si el valor es nulo
    /// </summary>
    public const string Is = "is";

    /// <summary>
    /// Filtro que coincide si el valor NO es nulo
    /// </summary>
    public const string IsNot = "isNot";

    /// <summary>
    /// Filtro que coincide si la fecha es antes de la especificada
    /// </summary>
    public const string Before = "before";

    /// <summary>
    /// Filtro que coincide si la fecha es después de la especificada
    /// </summary>
    public const string After = "after";

    /// <summary>
    /// Filtro que coincide si la fecha es igual a la especificada
    /// </summary>
    public const string DateIs = "dateIs";

    /// <summary>
    /// Filtro que coincide si la fecha NO es igual a la especificada
    /// </summary>
    public const string DateIsNot = "dateIsNot";

    /// <summary>
    /// Filtro que coincide si la fecha es antes de la especificada
    /// </summary>
    public const string DateBefore = "dateBefore";

    /// <summary>
    /// Filtro que coincide si la fecha es después de la especificada
    /// </summary>
    public const string DateAfter = "dateAfter";
  }
}
