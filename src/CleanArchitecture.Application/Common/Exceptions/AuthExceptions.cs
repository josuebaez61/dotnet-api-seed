using System;

namespace CleanArchitecture.Application.Common.Exceptions
{
  public class UserNotFoundError : ApplicationException
  {
    public UserNotFoundError(string emailOrUsername)
        : base("ERROR_USER_NOT_FOUND", $"User not found: {emailOrUsername}", new { EmailOrUsername = emailOrUsername })
    {
    }
  }

  public class UserNotFoundByIdError : ApplicationException
  {
    public UserNotFoundByIdError(Guid userId)
        : base("ERROR_USER_NOT_FOUND", $"User not found with ID: {userId}", new { UserId = userId })
    {
    }
  }

  public class InvalidCredentialsError : ApplicationException
  {
    public InvalidCredentialsError()
        : base("ERROR_INVALID_CREDENTIALS", "Invalid email/username or password")
    {
    }
  }

  public class AccountDeactivatedError : ApplicationException
  {
    public AccountDeactivatedError(string userId)
        : base("ERROR_ACCOUNT_DEACTIVATED", $"Account is deactivated for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class UserAlreadyExistsError : ApplicationException
  {
    public UserAlreadyExistsError(string field, string value)
        : base("ERROR_USER_ALREADY_EXISTS", $"User already exists with {field}: {value}", new { Field = field, Value = value })
    {
    }
  }

  public class InvalidPasswordError : ApplicationException
  {
    public InvalidPasswordError()
        : base("ERROR_INVALID_PASSWORD", "Invalid password format or requirements not met")
    {
    }
  }

  public class InvalidRefreshTokenError : ApplicationException
  {
    public InvalidRefreshTokenError()
        : base("ERROR_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token")
    {
    }
  }

  public class PasswordResetCodeInvalidError : ApplicationException
  {
    public PasswordResetCodeInvalidError(string userId)
        : base("ERROR_PASSWORD_RESET_CODE_INVALID", $"Invalid or expired password reset code for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class PasswordResetCodeExpiredError : ApplicationException
  {
    public PasswordResetCodeExpiredError(string userId)
        : base("ERROR_PASSWORD_RESET_CODE_EXPIRED", $"Password reset code has expired for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class PasswordResetCodeAlreadyUsedError : ApplicationException
  {
    public PasswordResetCodeAlreadyUsedError(string userId)
        : base("ERROR_PASSWORD_RESET_CODE_ALREADY_USED", $"Password reset code has already been used for user: {userId}", new { UserId = userId })
    {
    }
  }

  public class CurrentPasswordIncorrectError : ApplicationException
  {
    public CurrentPasswordIncorrectError()
        : base("ERROR_CURRENT_PASSWORD_INCORRECT", "Current password is incorrect")
    {
    }
  }

  public class PasswordChangeFailedError : ApplicationException
  {
    public PasswordChangeFailedError(string reason)
        : base("ERROR_PASSWORD_CHANGE_FAILED", $"Failed to change password: {reason}", new { Reason = reason })
    {
    }
  }
}
