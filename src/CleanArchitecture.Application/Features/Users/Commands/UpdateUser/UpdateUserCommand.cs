using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUser
{
  public class UpdateUserCommand : IRequest<UserDto>
  {
    public UpdateUserDto User { get; set; } = new();
  }
}


