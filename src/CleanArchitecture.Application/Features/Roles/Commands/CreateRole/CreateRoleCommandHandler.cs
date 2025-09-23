using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Roles.Commands.CreateRole
{
  public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public CreateRoleCommandHandler(RoleManager<Role> roleManager, IMapper mapper)
    {
      _roleManager = roleManager;
      _mapper = mapper;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
      var role = new Role
      {
        Id = Guid.NewGuid(),
        Name = request.Role.Name,
        NormalizedName = request.Role.Name.ToUpper(),
        Description = request.Role.Description,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      var result = await _roleManager.CreateAsync(role);
      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }

      return _mapper.Map<RoleDto>(role);
    }
  }
}
