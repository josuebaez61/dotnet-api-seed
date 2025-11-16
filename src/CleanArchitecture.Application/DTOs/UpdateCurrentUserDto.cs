using System;

namespace CleanArchitecture.Application.DTOs
{
  public class UpdateCurrentUserDto
  {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
  }
}


