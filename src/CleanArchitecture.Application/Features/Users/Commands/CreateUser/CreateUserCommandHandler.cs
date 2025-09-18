using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser
{
  public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly IPasswordGeneratorService _passwordGeneratorService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    public CreateUserCommandHandler(
        UserManager<User> userManager,
        IPasswordGeneratorService passwordGeneratorService,
        IEmailService emailService,
        IMapper mapper)
    {
      _userManager = userManager;
      _passwordGeneratorService = passwordGeneratorService;
      _emailService = emailService;
      _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
      // Generate a secure temporary password
      var temporaryPassword = _passwordGeneratorService.GenerateSecurePassword();

      var user = new User
      {
        Id = Guid.NewGuid(),
        FirstName = request.User.FirstName,
        LastName = request.User.LastName,
        Email = request.User.Email,
        UserName = request.User.UserName,
        DateOfBirth = request.User.DateOfBirth,
        ProfilePicture = request.User.ProfilePicture,
        CreatedAt = DateTime.UtcNow,
        IsActive = true,
        MustChangePassword = true // Mark that user must change password on first login
      };

      var result = await _userManager.CreateAsync(user, temporaryPassword);

      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors)}");
      }

      // Send temporary password email
      var userName = $"{user.FirstName} {user.LastName}".Trim();
      await _emailService.SendTemporaryPasswordEmailAsync(user.Email!, userName, temporaryPassword);

      return _mapper.Map<UserDto>(user);
    }
  }
}
