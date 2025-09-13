using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser
{
  public class CreateUserCommand : IRequest<UserDto>
  {
    public CreateUserDto User { get; set; } = new();
  }
}
