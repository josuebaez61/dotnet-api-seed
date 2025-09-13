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

  public class InvalidCredentialsError : ApplicationException
  {
    public InvalidCredentialsError()
        : base("INVALID_CREDENTIALS", "Invalid email/username or password")
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

  public class UserAlreadyExistsError : ApplicationException
  {
    public UserAlreadyExistsError(string field, string value)
        : base("USER_ALREADY_EXISTS", $"User already exists with {field}: {value}", new { Field = field, Value = value })
    {
    }
  }

  public class InvalidPasswordError : ApplicationException
  {
    public InvalidPasswordError()
        : base("INVALID_PASSWORD", "Invalid password format or requirements not met")
    {
    }
  }

  public class InvalidRefreshTokenError : ApplicationException
  {
    public InvalidRefreshTokenError()
        : base("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token")
    {
    }
  }

  public class PasswordResetCodeInvalidError : ApplicationException
  {
    public PasswordResetCodeInvalidError(string userId)
        : base("PASSWORD_RESET_CODE_INVALID", $"Invalid or expired password reset code for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class PasswordResetCodeExpiredError : ApplicationException
  {
    public PasswordResetCodeExpiredError(string userId)
        : base("PASSWORD_RESET_CODE_EXPIRED", $"Password reset code has expired for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class PasswordResetCodeAlreadyUsedError : ApplicationException
  {
    public PasswordResetCodeAlreadyUsedError(string userId)
        : base("PASSWORD_RESET_CODE_ALREADY_USED", $"Password reset code has already been used for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class CurrentPasswordIncorrectError : ApplicationException
  {
    public CurrentPasswordIncorrectError()
        : base("CURRENT_PASSWORD_INCORRECT", "Current password is incorrect")
    {
    }
  }

  public class PasswordChangeFailedError : ApplicationException
  {
    public PasswordChangeFailedError(string reason)
        : base("PASSWORD_CHANGE_FAILED", $"Failed to change password: {reason}", new { Reason = reason })
    {
    }
  }
}
