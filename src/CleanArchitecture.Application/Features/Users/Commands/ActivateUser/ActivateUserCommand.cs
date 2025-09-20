using System;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.ActivateUser
{
  public record ActivateUserCommand(Guid UserId) : IRequest<bool>;
}
