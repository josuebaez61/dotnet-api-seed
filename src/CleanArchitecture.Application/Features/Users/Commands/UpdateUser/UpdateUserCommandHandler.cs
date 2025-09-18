using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMapper _mapper;
    public UpdateUserCommandHandler(UserManager<User> userManager, IMapper mapper)
    {
      _userManager = userManager;
      _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByIdAsync(request.User.Id.ToString()) ?? throw new UserNotFoundByIdError(request.User.Id);

      // Update user properties
      user.FirstName = request.User.FirstName ?? user.FirstName;
      user.LastName = request.User.LastName ?? user.LastName;
      user.Email = request.User.Email ?? user.Email;
      user.UserName = request.User.UserName ?? user.UserName; //  Keep username in ?? user. sync with email

      if (request.User.DateOfBirth != default(DateTime))
      {
        user.DateOfBirth = request.User.DateOfBirth;
      }

      user.ProfilePicture = request.User.ProfilePicture ?? user.ProfilePicture;

      if (request.User.IsActive != user.IsActive)
      {
        user.IsActive = request.User.IsActive;
      }

      user.UpdatedAt = DateTime.UtcNow;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors)}");
      }

      return _mapper.Map<UserDto>(user);
    }
  }
}


