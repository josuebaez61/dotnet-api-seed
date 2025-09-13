using System;

namespace CleanArchitecture.Application.Common.Exceptions
{
  public abstract class ApplicationException : Exception
  {
    public string ErrorCode { get; }
    public object? Parameters { get; }

    protected ApplicationException(string errorCode, string message, object? parameters = null)
        : base(message)
    {
      ErrorCode = errorCode;
      Parameters = parameters;
    }

    protected ApplicationException(string errorCode, string message, Exception innerException, object? parameters = null)
        : base(message, innerException)
    {
      ErrorCode = errorCode;
      Parameters = parameters;
    }
  }
}
