using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Auth.Commands.UpdateCurrentUser
{
  public class UpdateCurrentUserCommandHandler : IRequestHandler<UpdateCurrentUserCommand, AuthUserDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UpdateCurrentUserCommandHandler(UserManager<User> userManager, IMapper mapper)
    {
      _userManager = userManager;
      _mapper = mapper;
    }

    public async Task<AuthUserDto> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _userManager.Users
          .Where(u => u.Id == request.UserId)
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role.RolePermissions)
                  .ThenInclude(rp => rp.Permission)
          .FirstOrDefaultAsync();

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Validate username uniqueness if it's being updated
      if (!string.IsNullOrWhiteSpace(request.Request.UserName) && request.Request.UserName != user.UserName)
      {
        var existingUser = await _userManager.FindByNameAsync(request.Request.UserName);
        if (existingUser != null && existingUser.Id != user.Id)
        {
          throw new UserAlreadyExistsError("username", request.Request.UserName);
        }
      }

      // Allowed fields to update for current user (email changes use a dedicated flow)
      if (!string.IsNullOrWhiteSpace(request.Request.FirstName))
      {
        user.FirstName = request.Request.FirstName;
      }
      if (!string.IsNullOrWhiteSpace(request.Request.LastName))
      {
        user.LastName = request.Request.LastName;
      }
      if (!string.IsNullOrWhiteSpace(request.Request.UserName))
      {
        user.UserName = request.Request.UserName;
      }

      if (request.Request?.DateOfBirth.HasValue == true)
      {
        user.DateOfBirth = request.Request.DateOfBirth.Value;
      }

      if (request.Request?.ProfilePicture != null)
      {
        user.ProfilePicture = request.Request.ProfilePicture;
      }
      user.UpdatedAt = DateTime.UtcNow;

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors)}");
      }

      return _mapper.Map<AuthUserDto>(user);
    }
  }
}


