using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Test.Commands.SendTestEmail
{
  public class SendTestEmailCommandHandler : IRequestHandler<SendTestEmailCommand, Unit>
  {
    private readonly IEmailService _emailService;

    public SendTestEmailCommandHandler(IEmailService emailService)
    {
      _emailService = emailService;
    }

    public async Task<Unit> Handle(SendTestEmailCommand request, CancellationToken cancellationToken)
    {
      var emailType = request.Request.EmailType;
      var email = request.Request.Email;
      var userName = "Test User"; // For testing purposes

      switch (emailType)
      {
        case EmailType.Welcome:
          await _emailService.SendWelcomeEmailAsync(email, userName);
          break;

        case EmailType.PasswordReset:
          var resetCode = "123456"; // Test code
          await _emailService.SendPasswordResetEmailAsync(email, userName, resetCode);
          break;

        case EmailType.PasswordChanged:
          await _emailService.SendPasswordChangedEmailAsync(email, userName);
          break;

        default:
          throw new InvalidTestEmailTypeError(emailType);
      }

      return Unit.Value;
    }
  }
}
