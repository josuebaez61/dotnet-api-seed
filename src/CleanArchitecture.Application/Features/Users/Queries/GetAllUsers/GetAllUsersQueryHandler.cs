using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Users.Queries.GetAllUsers
{
  public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
  {
    private readonly UserManager<User> _userManager;

    public GetAllUsersQueryHandler(UserManager<User> userManager)
    {
      _userManager = userManager;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
      var users = _userManager.Users.ToList();

      return users.Select(user => new UserDto
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
      }).ToList();
    }
  }
}
