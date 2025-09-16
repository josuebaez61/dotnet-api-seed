using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserById
{
  public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
  {
    private readonly UserManager<User> _userManager;

    public GetUserByIdQueryHandler(UserManager<User> userManager)
    {
      _userManager = userManager;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByIdAsync(request.Id.ToString());

      if (user == null)
        return null;

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
        EmailConfirmed = user.EmailConfirmed,
        MustChangePassword = user.MustChangePassword
      };
    }
  }
}
