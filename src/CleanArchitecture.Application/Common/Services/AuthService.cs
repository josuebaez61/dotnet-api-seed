using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Common.Constants;
using CleanArchitecture.Domain.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Application.Common.Services
{
  public class RefreshTokenInfo
  {
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
  }

  public class AuthService : IAuthService
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IPermissionService _permissionService;
    private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;
    private readonly Dictionary<string, RefreshTokenInfo> _refreshTokens = new();
    private readonly IMapper _mapper;
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        IPermissionService permissionService,
        IPasswordResetCodeRepository passwordResetCodeRepository,
        IMapper mapper)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _configuration = configuration;
      _permissionService = permissionService;
      _passwordResetCodeRepository = passwordResetCodeRepository;
      _mapper = mapper;
    }

    public async Task<AuthDataDto> LoginAsync(LoginRequestDto request)
    {
      var user = await GetUserByEmailOrUsernameAsync(request.EmailOrUsername);

      if (user == null)
      {
        throw new InvalidCredentialsError();
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
      if (!result.Succeeded)
      {
        throw new InvalidCredentialsError();
      }

      if (!user.IsActive)
      {
        throw new AccountDeactivatedError(user.Id.ToString());
      }

      return await GenerateAuthDataAsync(user);
    }

    public async Task<AuthDataDto> RegisterAsync(RegisterRequestDto request)
    {
      var existingUser = await _userManager.FindByEmailAsync(request.Email);
      if (existingUser != null)
      {
        throw new UserAlreadyExistsError("email", request.Email);
      }

      existingUser = await _userManager.FindByNameAsync(request.UserName);
      if (existingUser != null)
      {
        throw new UserAlreadyExistsError("username", request.UserName);
      }

      var user = new User
      {
        Id = Guid.NewGuid(),
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        UserName = request.UserName,
        DateOfBirth = request.DateOfBirth,
        ProfilePicture = request.ProfilePicture,
        CreatedAt = DateTime.UtcNow,
        IsActive = true,
        EmailConfirmed = true // For simplicity, auto-confirm emails
      };

      var result = await _userManager.CreateAsync(user, request.Password);
      if (!result.Succeeded)
      {
        throw new InvalidPasswordError();
      }

      // Generate tokens for the new user
      return await GenerateAuthDataAsync(user);
    }

    public async Task<AuthDataDto> RefreshTokenAsync(string refreshToken)
    {
      if (!_refreshTokens.TryGetValue(refreshToken, out var tokenInfo))
      {
        throw new InvalidRefreshTokenError();
      }

      if (tokenInfo.ExpiresAt < DateTime.UtcNow)
      {
        _refreshTokens.Remove(refreshToken);
        throw new InvalidRefreshTokenError();
      }

      var userId = tokenInfo.UserId;

      var user = await _userManager.Users
          .Where(u => u.Id == userId)
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role.RolePermissions)
                  .ThenInclude(rp => rp.Permission)
          .FirstOrDefaultAsync();

      if (user == null || !user.IsActive)
      {
        throw new UserNotFoundError(userId.ToString());
      }

      // Remove old refresh token
      _refreshTokens.Remove(refreshToken);

      // Generate new tokens
      return await GenerateAuthDataAsync(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
    {
      var user = await _userManager.FindByIdAsync(userId.ToString());
      if (user == null)
      {
        throw new UserNotFoundError(userId.ToString());
      }

      var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
      if (!result.Succeeded)
      {
        throw new CurrentPasswordIncorrectError();
      }

      return true;
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
      if (_refreshTokens.ContainsKey(refreshToken))
      {
        _refreshTokens.Remove(refreshToken);
        return true;
      }
      return false;
    }

    public async Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername)
    {
      return await _userManager.Users
                .Where(u => u.NormalizedEmail == emailOrUsername.ToUpper() || u.UserName == emailOrUsername)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync();
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
      var jwtSettings = _configuration.GetSection("JwtSettings");
      var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
      var issuer = jwtSettings["Issuer"] ?? "CleanArchitecture";
      var audience = jwtSettings["Audience"] ?? "CleanArchitectureUsers";
      var expiryHours = int.Parse(jwtSettings["ExpiryHours"] ?? "1");

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      // Get user permissions
      var userPermissions = await _permissionService.GetUserPermissionsAsync(user.Id);

      var claims = new List<Claim>
      {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("isActive", user.IsActive.ToString())
            };

      // Add permission claims
      foreach (var permission in userPermissions)
      {
        claims.Add(new Claim("permission", permission.Name));
      }

      var token = new JwtSecurityToken(
          issuer: issuer,
          audience: audience,
          claims: claims,
          expires: DateTime.UtcNow.AddHours(expiryHours),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateJwtToken(User user)
    {
      // For backward compatibility, but this should not be used
      return GenerateJwtTokenAsync(user).GetAwaiter().GetResult();
    }

    public string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }

    public async Task<AuthDataDto> GenerateAuthDataAsync(User user)
    {
      var token = await GenerateJwtTokenAsync(user);
      var refreshToken = GenerateRefreshToken();

      // Store refresh token (in production, store in database)
      _refreshTokens[refreshToken] = new RefreshTokenInfo
      {
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7)
      };

      return new AuthDataDto
      {
        Token = token,
        RefreshToken = refreshToken,
        ExpiresAt = DateTime.UtcNow.AddHours(1),
        User = _mapper.Map<AuthUserDto>(user)
      };
    }
    public async Task<string> GeneratePasswordResetCodeAsync(Guid userId)
    {
      // Generar código de 8 dígitos separados por guión medio (formato: 1234-5678)
      var random = new Random();
      var firstPart = random.Next(1000, 9999); // 4 dígitos
      var secondPart = random.Next(1000, 9999); // 4 dígitos
      var code = $"{firstPart}-{secondPart}";

      // Limpiar códigos expirados y usados del usuario
      var expiredCodes = await _passwordResetCodeRepository.FindAsync(prc => 
          prc.UserId == userId && (prc.ExpiresAt <= DateTime.UtcNow || prc.IsUsed));

      if (expiredCodes.Any())
      {
        foreach (var expiredCode in expiredCodes)
        {
          await _passwordResetCodeRepository.DeleteAsync(expiredCode);
        }
      }

      var resetCode = new PasswordResetCode
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Code = code,
        ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        IsUsed = false,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      // Agregar nuevo código a la base de datos
      await _passwordResetCodeRepository.AddAsync(resetCode);

      return code;
    }

    public async Task<bool> ValidatePasswordResetCodeAsync(Guid userId, string code)
    {
      var resetCode = await _passwordResetCodeRepository.FirstOrDefaultAsync(prc => 
          prc.UserId == userId && prc.Code == code);

      if (resetCode == null)
      {
        throw new PasswordResetCodeInvalidError(userId.ToString());
      }

      if (resetCode.ExpiresAt <= DateTime.UtcNow)
      {
        throw new PasswordResetCodeExpiredError(userId.ToString());
      }

      if (resetCode.IsUsed)
      {
        throw new PasswordResetCodeAlreadyUsedError(userId.ToString());
      }

      return true;
    }

    public async Task<Guid> ValidatePasswordResetCodeAndGetUserIdAsync(string code)
    {
      var resetCode = await _passwordResetCodeRepository.GetByCodeAsync(code);

      if (resetCode == null)
      {
        throw new PasswordResetCodeInvalidError("Invalid reset code");
      }

      if (resetCode.ExpiresAt <= DateTime.UtcNow)
      {
        throw new PasswordResetCodeExpiredError(resetCode.UserId.ToString());
      }

      if (resetCode.IsUsed)
      {
        throw new PasswordResetCodeAlreadyUsedError(resetCode.UserId.ToString());
      }

      return resetCode.UserId;
    }

    public async Task MarkPasswordResetCodeAsUsedAsync(Guid userId, string code)
    {
      var resetCode = await _passwordResetCodeRepository.FirstOrDefaultAsync(prc => 
          prc.UserId == userId && prc.Code == code);

      if (resetCode != null)
      {
        await _passwordResetCodeRepository.MarkAsUsedAsync(resetCode.Id);
      }
    }
  }
}
