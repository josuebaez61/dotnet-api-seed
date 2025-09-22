using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Auth.Queries.GetAuthUser
{
  public class GetAuthUserQueryHandler : IRequestHandler<GetAuthUserQuery, AuthUserDto>
  {

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    public GetAuthUserQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _mapper = mapper;
    }

    public async Task<AuthUserDto> Handle(GetAuthUserQuery request, CancellationToken cancellationToken)
    {
      var user = await _userManager.Users
          .Where(u => u.Id == request.Id)
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role.RolePermissions)
                  .ThenInclude(rp => rp.Permission)
          .FirstOrDefaultAsync();

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.Id);
      }

      return _mapper.Map<AuthUserDto>(user);
    }

  }
}