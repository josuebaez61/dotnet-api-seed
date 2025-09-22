using System;

namespace CleanArchitecture.Application.Common.Exceptions
{
  public class ValidationError : ApplicationException
  {
    public ValidationError(string errorCode, string message, object? parameters = null)
        : base(errorCode, message, parameters)
    {
    }
  }

  public class RequiredFieldError : ValidationError
  {
    public RequiredFieldError(string fieldName)
        : base("REQUIRED_FIELD", $"Field '{fieldName}' is required", new { FieldName = fieldName })
    {
    }
  }

  public class InvalidEmailFormatError : ValidationError
  {
    public InvalidEmailFormatError(string email)
        : base("INVALID_EMAIL_FORMAT", $"Invalid email format: {email}", new { Email = email })
    {
    }
  }

  public class PasswordTooWeakError : ValidationError
  {
    public PasswordTooWeakError()
        : base("PASSWORD_TOO_WEAK", "Password does not meet security requirements")
    {
    }
  }

  public class InvalidDateOfBirthError : ValidationError
  {
    public InvalidDateOfBirthError(DateTime dateOfBirth)
        : base("INVALID_DATE_OF_BIRTH", $"Invalid date of birth: {dateOfBirth:yyyy-MM-dd}", new { DateOfBirth = dateOfBirth })
    {
    }
  }

  public class InvalidAgeError : ValidationError
  {
    public InvalidAgeError(int age)
        : base("INVALID_AGE", $"Invalid age: {age}. Must be between 13 and 120 years old", new { Age = age })
    {
    }
  }

  public class UsernameTooShortError : ValidationError
  {
    public UsernameTooShortError(int minLength)
        : base("USERNAME_TOO_SHORT", $"Username must be at least {minLength} characters long", new { MinLength = minLength })
    {
    }
  }

  public class UsernameTooLongError : ValidationError
  {
    public UsernameTooLongError(int maxLength)
        : base("USERNAME_TOO_LONG", $"Username must be at most {maxLength} characters long", new { MaxLength = maxLength })
    {
    }
  }

  public class InvalidUsernameFormatError : ValidationError
  {
    public InvalidUsernameFormatError(string username)
        : base("INVALID_USERNAME_FORMAT", $"Invalid username format: {username}. Only letters, numbers, and underscores are allowed", new { Username = username })
    {
    }
  }
}

