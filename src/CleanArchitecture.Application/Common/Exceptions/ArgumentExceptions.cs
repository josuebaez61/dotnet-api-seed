using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Common.Exceptions
{
  public class InvalidTestEmailTypeError : ApplicationException
  {
    public InvalidTestEmailTypeError(EmailType emailType)
        : base("INVALID_TEST_EMAIL_TYPE", $"Invalid email type: {emailType}", new { EmailType = emailType })
    {
    }
  }
}