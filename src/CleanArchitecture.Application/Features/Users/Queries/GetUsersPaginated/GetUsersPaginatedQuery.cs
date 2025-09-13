using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUsersPaginated
{
  public class GetUsersPaginatedQuery : IRequest<PaginationResponse<UserDto>>
  {
    public GetUsersPaginatedRequestDto Request { get; set; } = new();
  }
}
