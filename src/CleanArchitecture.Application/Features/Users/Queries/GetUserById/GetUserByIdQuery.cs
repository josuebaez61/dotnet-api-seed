using System;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserById
{
  public class GetUserByIdQuery : IRequest<UserDto?>
  {
    public Guid Id { get; set; }
  }
}
