using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.RequestEmailChange
{
  public class RequestEmailChangeCommand : IRequest<Unit>
  {
    public Guid UserId { get; set; }
    public RequestEmailChangeDto Request { get; set; } = new RequestEmailChangeDto();
  }
}
