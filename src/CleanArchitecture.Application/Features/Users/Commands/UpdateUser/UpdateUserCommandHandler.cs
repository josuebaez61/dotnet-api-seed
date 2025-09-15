using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUser
{
  public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
  {
    private readonly UserManager<User> _userManager;

    public UpdateUserCommandHandler(UserManager<User> userManager)
    {
      _userManager = userManager;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByIdAsync(request.User.Id.ToString());

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.User.Id);
      }

      // Update user properties
      user.FirstName = request.User.FirstName;
      user.LastName = request.User.LastName;
      user.Email = request.User.Email;
      user.UserName = request.User.Email; // Keep username in sync with email
      user.DateOfBirth = request.User.DateOfBirth;
      user.ProfilePicture = request.User.ProfilePicture;
      user.IsActive = request.User.IsActive;
      user.UpdatedAt = DateTime.UtcNow;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors)}");
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


