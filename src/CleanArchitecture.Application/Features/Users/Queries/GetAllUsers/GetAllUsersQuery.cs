using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetAllUsers
{
  public class GetAllUsersQuery : IRequest<List<UserDto>>
  {
  }
}
