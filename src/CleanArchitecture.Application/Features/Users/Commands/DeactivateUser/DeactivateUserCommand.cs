using System;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.DeactivateUser
{
  public record DeactivateUserCommand(Guid UserId) : IRequest<bool>;
}
