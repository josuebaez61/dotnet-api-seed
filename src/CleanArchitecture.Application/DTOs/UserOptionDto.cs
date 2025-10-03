using System;

namespace CleanArchitecture.Application.DTOs
{
  public class UserOptionDto
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
  }
}
