using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IPaginationService<T>
  {
    Task<PaginationResponse<T>> GetPaginatedAsync(
        IQueryable<T> query,
        PaginationRequest request,
        CancellationToken cancellationToken = default);
  }

  public interface IPaginationService
  {
    Task<PaginationResponse<T>> GetPaginatedAsync<T>(
        IQueryable<T> query,
        PaginationRequest request,
        CancellationToken cancellationToken = default);
  }
}
