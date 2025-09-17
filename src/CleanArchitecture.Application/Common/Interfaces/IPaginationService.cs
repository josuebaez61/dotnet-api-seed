using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IPaginationService
  {
    Task<PaginationResponse<T>> GetPaginatedAsync<T>(
        IQueryable<T> query,
        PaginationRequest request,
        CancellationToken cancellationToken = default);
  }
}
