using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Test.Commands.SendTestEmail
{
  public class SendTestEmailCommand : IRequest<Unit>
  {
    public TestEmailRequestDto Request { get; set; } = new();
  }
}
