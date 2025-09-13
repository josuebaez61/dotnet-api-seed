using System.Linq.Expressions;
using System.Reflection;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Services
{
  public class PaginationService : IPaginationService
  {
    public async Task<PaginationResponse<T>> GetPaginatedAsync<T>(
        IQueryable<T> query,
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
      // Aplicar búsqueda global si existe
      if (!string.IsNullOrEmpty(request.GlobalSearch))
      {
        query = ApplyGlobalSearch(query, request.GlobalSearch);
      }

      // Obtener el total de registros ANTES de aplicar ordenamiento y paginación
      var totalCount = await query.CountAsync(cancellationToken);

      // Aplicar ordenamiento
      query = ApplySorting(query, request);

      // Aplicar paginación
      var items = await query
          .Skip(request.Offset)
          .Take(request.Limit)
          .ToListAsync(cancellationToken);

      // Crear metadata de paginación
      var metadata = new PaginationMetadata
      {
        CurrentPage = request.Page,
        PageSize = request.Limit,
        TotalCount = totalCount,
        CurrentPageItemCount = items.Count,
        GlobalSearch = request.GlobalSearch,
        SortBy = request.SortBy,
        SortDirection = request.SortDirection
      };

      return new PaginationResponse<T>
      {
        Items = items,
        Metadata = metadata
      };
    }

    private IQueryable<T> ApplyGlobalSearch<T>(IQueryable<T> query, string searchTerm)
    {
      var entityType = typeof(T);
      var properties = entityType.GetProperties()
          .Where(p => p.PropertyType == typeof(string) && p.CanRead)
          .ToList();

      if (!properties.Any())
        return query;

      var parameter = Expression.Parameter(typeof(T), "x");
      Expression? searchExpression = null;

      foreach (var property in properties)
      {
        var propertyAccess = Expression.Property(parameter, property);

        // Verificar si la propiedad es nullable string
        Expression propertyValue = property.PropertyType == typeof(string)
            ? propertyAccess
            : Expression.Call(propertyAccess, "ToString", null);

        // Crear expresión: property.ToLower().Contains(searchTerm.ToLower())
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        var toLowerExpression = Expression.Call(propertyValue, toLowerMethod);
        var containsExpression = Expression.Call(
            toLowerExpression,
            containsMethod,
            Expression.Constant(searchTerm.ToLower()));

        searchExpression = searchExpression == null
            ? containsExpression
            : Expression.OrElse(searchExpression, containsExpression);
      }

      if (searchExpression != null)
      {
        var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
        query = query.Where(lambda);
      }

      return query;
    }

    private IQueryable<T> ApplySorting<T>(IQueryable<T> query, PaginationRequest request)
    {
      var entityType = typeof(T);

      // Si no se especifica ordenamiento, usar ordenamiento por defecto (ID si existe, o primer campo)
      string sortBy = request.SortBy;
      bool isDescending = request.IsDescending;

      if (string.IsNullOrEmpty(sortBy))
      {
        // Buscar un campo ID primero, luego el primer campo disponible
        var idProperty = entityType.GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (idProperty != null)
        {
          sortBy = "Id";
          isDescending = false; // Ordenamiento ascendente por defecto para ID
        }
        else
        {
          var firstProperty = entityType.GetProperties()
              .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
              .FirstOrDefault();

          if (firstProperty != null)
          {
            sortBy = firstProperty.Name;
            isDescending = false;
          }
          else
          {
            return query; // No hay propiedades para ordenar
          }
        }
      }

      var property = entityType.GetProperty(
          sortBy,
          BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

      if (property == null)
        return query;

      var parameter = Expression.Parameter(typeof(T), "x");
      var propertyAccess = Expression.Property(parameter, property);

      var orderByExpression = Expression.Lambda(propertyAccess, parameter);

      var methodName = isDescending ? "OrderByDescending" : "OrderBy";

      // Obtener el método genérico de ordenamiento
      var orderByMethod = typeof(Queryable).GetMethods()
          .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
          .First()
          .MakeGenericMethod(typeof(T), property.PropertyType);

      return (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, orderByExpression })!;
    }
  }
}
