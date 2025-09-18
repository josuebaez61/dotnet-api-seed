using System;

namespace CleanArchitecture.Application.Common.Exceptions
{
  public class UserNotFoundError : ApplicationException
  {
    public UserNotFoundError(string emailOrUsername)
        : base("USER_NOT_FOUND", $"User not found: {emailOrUsername}", new { EmailOrUsername = emailOrUsername })
    {
    }
  }

  public class UserNotFoundByIdError : ApplicationException
  {
    public UserNotFoundByIdError(Guid userId)
        : base("USER_NOT_FOUND", $"User not found with ID: {userId}", new { UserId = userId })
    {
    }
  }

  public class UserAlreadyExistsError : ApplicationException
  {
    public UserAlreadyExistsError(string field, string value)
        : base("USER_ALREADY_EXISTS", $"User already exists with {field}: {value}", new { Field = field, Value = value })
    {
    }
  }

  public class AccountDeactivatedError : ApplicationException
  {
    public AccountDeactivatedError(string userId)
        : base("ACCOUNT_DEACTIVATED", $"Account is deactivated for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class UserNotActiveError : ApplicationException
  {
    public UserNotActiveError(string emailOrUsername)
        : base("USER_NOT_ACTIVE", $"User is not active: {emailOrUsername}", new { EmailOrUsername = emailOrUsername })
    {
    }
  }

  public class UserEmailNotConfirmedError : ApplicationException
  {
    public UserEmailNotConfirmedError(string email)
        : base("USER_EMAIL_NOT_CONFIRMED", $"User email is not confirmed: {email}", new { Email = email })
    {
    }
  }

  public class UserPasswordResetCodeExpiredError : ApplicationException
  {
    public UserPasswordResetCodeExpiredError()
        : base("PASSWORD_RESET_CODE_EXPIRED", "Password reset code has expired")
    {
    }
  }

  public class UserPasswordResetCodeUsedError : ApplicationException
  {
    public UserPasswordResetCodeUsedError()
        : base("PASSWORD_RESET_CODE_USED", "Password reset code has already been used")
    {
    }
  }

  public class UserEmailVerificationCodeExpiredError : ApplicationException
  {
    public UserEmailVerificationCodeExpiredError()
        : base("EMAIL_VERIFICATION_CODE_EXPIRED", "Email verification code has expired")
    {
    }
  }

  public class UserEmailVerificationCodeUsedError : ApplicationException
  {
    public UserEmailVerificationCodeUsedError()
        : base("EMAIL_VERIFICATION_CODE_USED", "Email verification code has already been used")
    {
    }
  }

  public class UserMustChangePasswordError : ApplicationException
  {
    public UserMustChangePasswordError()
        : base("USER_MUST_CHANGE_PASSWORD", "User must change password before accessing the system")
    {
    }
  }
}
