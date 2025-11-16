using System;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.UpdateCurrentUser
{
  public class UpdateCurrentUserCommand : IRequest<AuthUserDto>
  {
    public Guid UserId { get; set; }
    public UpdateCurrentUserDto Request { get; set; } = new();
  }
}


