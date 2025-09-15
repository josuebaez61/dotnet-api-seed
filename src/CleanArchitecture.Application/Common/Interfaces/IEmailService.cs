using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IEmailService
  {
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendPasswordResetEmailAsync(string to, string userName, string resetCode);
    Task SendWelcomeEmailAsync(string to, string userName);
    Task SendPasswordChangedEmailAsync(string to, string userName);
    Task SendEmailChangeVerificationEmailAsync(string to, string userName, string verificationCode);
    Task SendEmailChangeConfirmationEmailAsync(string to, string userName, string oldEmail);
    Task SendTemporaryPasswordEmailAsync(string to, string userName, string temporaryPassword);
  }
}
