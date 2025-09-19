using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Auth.Commands.ChangeFirstTimePassword
{
  public class ChangeFirstTimePasswordCommandHandler : IRequestHandler<ChangeFirstTimePasswordCommand, AuthDataDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;

    public ChangeFirstTimePasswordCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAuthService authService,
        IEmailService emailService)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _authService = authService;
      _emailService = emailService;
    }

    public async Task<AuthDataDto> Handle(ChangeFirstTimePasswordCommand request, CancellationToken cancellationToken)
    {
      // Find user by ID
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
        throw new UserNotFoundError(request.UserId.ToString());
      }

      // Verify the user must change password
      if (!user.MustChangePassword)
      {
        throw new PasswordChangeNotRequiredError(request.UserId.ToString());
      }

      // Change the password
      var token = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, token, request.Request.NewPassword);

      if (!result.Succeeded)
      {
        throw new PasswordChangeFailedError(string.Join(", ", result.Errors.Select(e => e.Description)));
      }

      // Clear the must change password flag
      user.MustChangePassword = false;

      // User received temporary password via email, so email is confirmed
      user.EmailConfirmed = true;

      await _userManager.UpdateAsync(user);

      // Send password changed confirmation email
      var userName = $"{user.FirstName} {user.LastName}".Trim();
      await _emailService.SendPasswordChangedEmailAsync(user.Email!, userName);

      // Generate new auth response
      return await _authService.GenerateAuthDataAsync(user);
    }
  }
}
