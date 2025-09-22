using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRoleById
{
  public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    public GetRoleByIdQueryHandler(RoleManager<Role> roleManager, IMapper mapper)
    {
      _roleManager = roleManager;
      _mapper = mapper;
    }

    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
      var role = await _roleManager.Roles
        .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      return _mapper.Map<RoleDto>(role);
    }
  }
}
