using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Queries.GetAuthUser
{
  public class GetAuthUserQuery : IRequest<AuthUserDto>
  {
    public Guid Id { get; set; }
  }
}
