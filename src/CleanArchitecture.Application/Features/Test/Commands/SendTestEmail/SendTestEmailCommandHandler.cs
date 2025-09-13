using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Test.Commands.SendTestEmail
{
  public class SendTestEmailCommandHandler : IRequestHandler<SendTestEmailCommand, ApiResponse>
  {
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public SendTestEmailCommandHandler(
        IEmailService emailService,
        ILocalizationService localizationService)
    {
      _emailService = emailService;
      _localizationService = localizationService;
    }

    public async Task<ApiResponse> Handle(SendTestEmailCommand request, CancellationToken cancellationToken)
    {
      var emailType = request.Request.EmailType;
      var email = request.Request.Email;
      var userName = "Test User"; // For testing purposes

      try
      {
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
            throw new ArgumentException($"Unsupported email type: {emailType}");
        }

        return ApiResponse.SuccessResponse(
            _localizationService.GetSuccessMessage("TestEmailSent", emailType.ToString()));
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(
            _localizationService.GetErrorMessage("TestEmailFailed", ex.Message));
      }
    }
  }
}
