using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser
{
  public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
  {
    private readonly UserManager<User> _userManager;

    public CreateUserCommandHandler(UserManager<User> userManager)
    {
      _userManager = userManager;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
      var user = new User
      {
        Id = Guid.NewGuid(),
        FirstName = request.User.FirstName,
        LastName = request.User.LastName,
        Email = request.User.Email,
        UserName = request.User.Email,
        DateOfBirth = request.User.DateOfBirth,
        ProfilePicture = request.User.ProfilePicture,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
      };

      var result = await _userManager.CreateAsync(user, request.User.Password);

      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors)}");
      }

      return new UserDto
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!,
        UserName = user.UserName,
        DateOfBirth = user.DateOfBirth,
        ProfilePicture = user.ProfilePicture,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        IsActive = user.IsActive,
        EmailConfirmed = user.EmailConfirmed
      };
    }
  }
}
