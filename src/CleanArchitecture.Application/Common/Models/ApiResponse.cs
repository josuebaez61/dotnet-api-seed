using System;

namespace CleanArchitecture.Application.Common.Models
{
  public class ApiResponse<T>
  {
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RequestId { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "")
    {
      return new ApiResponse<T>
      {
        Success = true,
        Message = message,
        Data = data
      };
    }

    public static ApiResponse<T> ErrorResponse(string message, object? errors = null)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Message = message,
        Errors = errors
      };
    }

    public static ApiResponse<T> ValidationErrorResponse(string message, object errors)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Message = message,
        Errors = errors
      };
    }
  }

  public class ApiResponse : ApiResponse<object>
  {
    public static new ApiResponse SuccessResponse(string message = "")
    {
      return new ApiResponse
      {
        Success = true,
        Message = message
      };
    }

    public static new ApiResponse ErrorResponse(string message, object? errors = null)
    {
      return new ApiResponse
      {
        Success = false,
        Message = message,
        Errors = errors
      };
    }

    public static new ApiResponse ValidationErrorResponse(string message, object errors)
    {
      return new ApiResponse
      {
        Success = false,
        Message = message,
        Errors = errors
      };
    }
  }
}
