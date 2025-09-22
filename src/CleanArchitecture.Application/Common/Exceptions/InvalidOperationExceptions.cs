namespace CleanArchitecture.Application.Common.Exceptions
{
  public class InvalidOperationError : ApplicationException
  {
    public InvalidOperationError(string message)
        : base("INVALID_OPERATION", message, new { })
    {
    }
  }
  public class EmailCannotBeTheSameAsCurrentError : ApplicationException
  {
    public EmailCannotBeTheSameAsCurrentError(string email)
        : base("EMAIL_CANNOT_BE_THE_SAME_AS_CURRENT", $"The new email '{email}' cannot be the same as the current email.", new { Email = email })
    {
    }
  }

  public class EmailVerificationCodeExpiredError : ApplicationException
  {
    public EmailVerificationCodeExpiredError()
        : base("EMAIL_VERIFICATION_CODE_EXPIRED", "Email verification code has expired")
    {
    }
  }
  public class EmailSendingFailedError : ApplicationException
  {
    public EmailSendingFailedError()
        : base("EMAIL_SENDING_FAILED", "Email sending failed")
    {
    }
  }


}